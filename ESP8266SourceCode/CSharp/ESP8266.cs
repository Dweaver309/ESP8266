using System;
using System.Text;
using Microsoft.SPOT;
using Microsoft.VisualBasic;
using Microsoft.SPOT.Hardware;
using System.Threading;
using System.IO.Ports;


public class ESP8266
{
    private static SerialPort Port;

    // Get the debugging data in the DataReceivedEvent
    public static string Data = string.Empty;

    // Raised when data is received from serial port
    public static event DataReceivedEventEventHandler DataReceivedEvent;

    public  delegate void DataReceivedEventEventHandler();

    // Returned in the ServerRequestEvent Handler
    public static string ServerRequest = string.Empty;

    // Raised when server request is made
    public static  event ServerRequestEventEventHandler ServerRequestEvent;

    public  delegate void ServerRequestEventEventHandler();

    // Raised when AP server request is made
    public static event APServerRequestEventEventHandler APServerRequestEvent;

    public  delegate void APServerRequestEventEventHandler();

    // Used to start polling the serial port in Initialize  
    private static Thread PollThread = new Thread(PollSerialPort);

    // Controls the bytes send with a server request
    const int MAX_BUFF = 1024;

    // Used when polling serial port 
    private static int CurrentMode = (int)Mode.Connect;

     enum  Mode
    {
        Connect = 1,
        TCPServer = 2,
        TCPClient = 3,
        AP =4
    }

    // Used to Parse date string in GetTimeFromString
    struct StrDate
    {
        public static int Year;
        public static int Month;
        public static int Day;
        public static int Hour;
        public static int Minute;
        public static int Second;
    }

    // Set to false to stop sending text to the console
    private static bool Debugging = true;

    // Set after StartServer 
    public static string IPAddress = "Connecting!";

    // Hours to adjust GMT time
    public static int LocalTimeOffSet = -5;

    // Pause time in MS
    public static int CommandDelay = 6000;

    // Updated in GetLinkedID
    private static string LinkedID = "0";

    /// <summary>
    ///     ''' GH_PD must be pulled high to enable the signal
    ///     ''' GPIO0 must be pulled high, if set to low the chip will be in flash boot mode 
    ///     ''' Toggle the pins to reset the chip
    ///     ''' Open the serial port and start polling for data
    ///     ''' Set chip to Station mode 
    ///     ''' </summary>
    ///     ''' <param name="GIO0CpuPin"></param>
    ///     ''' <param name="CH_PDCpuPin"></param>
    ///     ''' <param name="PortToOpen</param> ' Example COM1 or COM2
    public static void Initialize(Cpu.Pin GIO0CpuPin, Cpu.Pin CH_PDCpuPin, string PortToOpen)
    {
        OutputPort GPIO0 = new OutputPort(GIO0CpuPin, false);

        OutputPort CH_PD = new OutputPort(CH_PDCpuPin, false);

        Thread.Sleep(1000);

        GPIO0.Write(true);

        CH_PD.Write(true);

        Thread.Sleep(1000);

        Port = new SerialPort(PortToOpen, 115200, Parity.None, 8, StopBits.One);

        Port.Open();

        Port.Flush();

        PollThread.Start();

        // Set to station to connect to router
        SendData("AT+CWMODE=1");

        Thread.Sleep(CommandDelay);
    }

    /// <summary>
    ///     ''' Set chip to TCP mode 
    ///     ''' Pause for the response from Google 
    ///     ''' Close the connection
    ///     ''' </summary>
    public static void SetTime()
    {
        CurrentMode = (int)Mode.TCPClient;

        string StringToSend = "HEAD / HTTP/1.1" + Constants.vbCrLf + "Host: www.google.com" + Constants.vbCrLf + "Accept */*" + Constants.vbCrLf + "User-Agent: Mozilla/4.0 (compatible; esp8266 Lua;)" + Constants.vbCrLf + Constants.vbCrLf;

        SendData("AT+CIPMUX=1");

        Thread.Sleep(CommandDelay);

        SendData("AT+CIPSTART=0,\"TCP\",\"www.google.com\",80");

        Thread.Sleep(CommandDelay);

        int Length = StringToSend.Length;

        Length += 1;

        PrintData("Length " + Length.ToString());

        SendData("AT+CIPSENDBUF=0," + Length);

        Thread.Sleep(2000);

        SendData(StringToSend);

        Thread.Sleep(CommandDelay);

        SendData("AT+CIPCLOSE=0");
    }

    /// <summary>
    ///     ''' Parse the string and set time and date
    ///     ''' Uses the LocalTimeOffSet property to set the local time
    ///     ''' </summary>
    ///     ''' <param name="IPDString"></param>  
    ///     ''' Called from PollingSerialPort
    private static void GetTimeFromString(string IPDString)
    {
        try
        {
            string DateString = string.Empty;

            string[] Lines;

            // Lines = IPDString.Split((char[])Constants.vbCrLf);
            Lines = IPDString.Split(Constants.vbLf.ToCharArray());


            if (Lines.Length > 0)
            {
                for (var i = 0; i <= Lines.Length - 1; i++)
                {
                    
                   PrintData("Line " + i.ToString() + " " + Lines[1] );
                   
                    if (InString(Lines[i], "Date:"))
                    {
                        DateString = Lines[i];

                        break;
                    }
                }

                if (DateString.Length > 1)
                    PrintData("The Date is " + DateString);

                char Space = Strings.ChrW(32);

                char Colon = Strings.ChrW(58);

                string[] DateLines;

                string HMSString = string.Empty;

                DateLines = DateString.Split(Space);

                for (var i = 0; i <= DateLines.Length - 1; i++)
                {
                    if (i == 2)
                        StrDate.Day = System.Convert.ToInt32(DateLines[i]);

                    if (i == 3)
                        StrDate.Month = GetMonthFromString(DateLines[i]);

                    if (i == 4)
                        StrDate.Year = System.Convert.ToInt32(DateLines[i]);

                    if (i == 5)
                        HMSString = DateLines[i];
                }

                DateLines = HMSString.Split(Colon);

                if (DateLines.Length > 2)
                {
                    StrDate.Hour = System.Convert.ToInt32(DateLines[0]);
                    
                    StrDate.Minute = System.Convert.ToInt32(DateLines[1]);

                    StrDate.Second = System.Convert.ToInt32(DateLines[2]);
                }

                DateTime UTCdate = new DateTime(StrDate.Year, StrDate.Month, StrDate.Day, StrDate.Hour, StrDate.Minute, StrDate.Second);

                DateTime LocalDate = UTCdate;

                LocalDate = LocalDate.AddHours(LocalTimeOffSet);

                Utility.SetLocalTime(LocalDate);

                PrintData(DateTime.Now.ToString());
            }
        }
        catch (Exception ex)
        {
            PrintData("Error: GetTimeFromString: " + ex.ToString());
        }
    }

    /// <summary>
    ///     ''' Called from GetTimeFromString
    ///     ''' </summary>
    private static int GetMonthFromString(string CurrentMonth)
    {
        if (InString(CurrentMonth, "Jan"))
            return 1;

        if (InString(CurrentMonth, "Feb"))
            return 2;

        if (InString(CurrentMonth, "Mar"))
            return 3;

        if (InString(CurrentMonth, "Apr"))
            return 4;

        if (InString(CurrentMonth, "May"))
            return 5;

        if (InString(CurrentMonth, "Jun"))
            return 6;

        if (InString(CurrentMonth, "Jul"))
            return 7;

        if (InString(CurrentMonth, "Aug"))
            return 8;

        if (InString(CurrentMonth, "Sep"))
            return 9;

        if (InString(CurrentMonth, "Oct"))
            return 10;

        if (InString(CurrentMonth, "Nov"))
            return 11;

        if (InString(CurrentMonth, "Dec"))
            return 12;

        return 1;
    }

    /// <summary>
    ///     ''' Esp8266 can have up to 4 connections
    ///     ''' Get the current connection
    ///     ''' Called from PollSerialPort
    ///     ''' </summary>
    private static string GetLinkID(string IPDString)
    {
        if (InString(IPDString, "+IPD,0"))
            return "0";

        if (InString(IPDString, "+IPD,1"))
            return "1";

        if (InString(IPDString, "+IPD,3"))
            return "2";

        if (InString(IPDString, "+IPD,4"))
            return "3";

        return "0";
    }

    /// <summary>
    ///     ''' Gets IP Address from string
    ///     ''' Called from PollSerialPort
    ///     ''' </summary>
    ///     ''' <returns>Parsed IP Address</returns>
    private static string ParseIPAddres(string Data)
    {
        try
        {
            string Str = "+CIFSR:STAIP,";

            int IpStart = Data.LastIndexOf(Str) + Str.Length;

            IPAddress = Data.Substring(IpStart);

            int IPEnd = IPAddress.IndexOf(Constants.vbCrLf);

            IPAddress = IPAddress.Substring(1, IPEnd - 2);
        }
        catch (Exception ex)
        {
            PrintData("Error: ParseIPAddress: " + ex.ToString());
        }

        return IPAddress;
    }

    /// <summary>
    ///     ''' Polling is started in Initialize
    ///     ''' </summary>
    private static void PollSerialPort()
    {
        try
        {
            byte[] Buffer;

            while (true)
            {
                if (Port.BytesToRead > 1)
                {
                    Thread.Sleep(20);

                    Buffer = new byte[Port.BytesToRead + 1];

                    Port.Read(Buffer, 0, Buffer.Length);

                    char[] Chars = System.Text.Encoding.UTF8.GetChars(Buffer);

                    for (var i = 0; i <= Chars.Length - 1; i++)

                        Data += Chars[i];


                    switch (CurrentMode)
                    {
                        case (int)Mode.Connect:
                            {

                                // Get IP 
                                if (InString(Data, "+CIFSR:STAIP,"))
                                    IPAddress = ParseIPAddres(Data);
                                break;
                            }

                        case (int)Mode.TCPServer:
                        case (int)Mode.AP:
                            {
                                if (InString(Data, "+IPD"))
                                {
                                    ServerRequest = Data;

                                    // Get the current connection linked ID
                                    // Used to send the requested data in SendServerRequest
                                    LinkedID = GetLinkID(ServerRequest);

                                    if (CurrentMode == (int)Mode.TCPServer)
                                        ServerRequestEvent.Invoke();

                                    if (CurrentMode == (int)Mode.AP)
                                        APServerRequestEvent.Invoke();
                                }

                                break;
                            }

                        case (int)Mode.TCPClient:
                            {

                                // Returned request from Google
                                if (InString(Data, "+IPD"))
                                    GetTimeFromString(Data);
                                break;
                            }
                    }

                    DataReceivedEvent.Invoke();

                    Data = string.Empty;
                }

                Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            PrintData("Error: PollSerialPort: " + ex.ToString());
        }
    }

    /// <summary>
    ///     ''' Sends the data requested 
    ///     ''' </summary>
    public static void SendServerRequest(string ServerRequestedData)
    {
        try
        {
            int FileLength = ServerRequestedData.Length;

            int BytesSent = 0;

            while (BytesSent < FileLength)
            {
                int BytesToRead = FileLength - BytesSent;

                if (BytesToRead > MAX_BUFF)
                    BytesToRead = MAX_BUFF;

                ESP8266.SendData("AT+CIPSENDBUF=" + LinkedID + "," + BytesToRead);

                Thread.Sleep(20);

                string DataToSend = ServerRequestedData.Substring(BytesSent, BytesToRead);

                ESP8266.SendData(DataToSend);

                BytesSent += BytesToRead;
            }

            Thread.Sleep(20);

            SendData("AT+CIPCLOSE=" + LinkedID);
        }
        catch (Exception ex)
        {
            PrintData("Error: SendServerRequest: " + ex.ToString());
        }
    }

    /// <summary>
    ///     ''' Declared Public use to send ANY AT commands
    ///     ''' Pause after each AT command is sent
    ///     ''' Converts the string to bytes
    ///     ''' Sends the data to the serial port
    ///     ''' </summary>
    public static void SendData(string StrData)
    {
        try
        {

            // Clean it out
            Port.Flush();

            byte[] Buffer = Encoding.UTF8.GetBytes(StrData + Constants.vbCrLf);

            Port.Write(Buffer, 0, Buffer.Length);

            PrintData("Sent: " + StrData);
        }
        catch (Exception ex)
        {
            PrintData("Error: SendData: " + ex.ToString());
        }
    }

    /// <summary>
    ///     ''' Displays string to console when Debugging is set to True
    ///     ''' </summary>
    public static void PrintData(string Str)
    {
        if (Debugging)

            Debug.Print(Str);
    }

    /// <summary>
    ///     ''' Chip information
    ///     ''' </summary>
    public static void GetVersion()
    {
        SendData("AT+GMR");

        Thread.Sleep(CommandDelay);
    }

    /// <summary>
    ///     ''' Display the routers that can be connected to
    ///     ''' </summary>
    public static void ListAccessPoints()
    {
        SendData("AT+CWLAP");

        Thread.Sleep(CommandDelay);
    }

    /// <summary>
    ///     ''' Connect to the router and get the IP address
    ///     ''' IPAddress is parsed from PollSerialPort after connecting to router
    ///     ''' </summary>
    public static void Connect(string SSID, string Password)
    {
        SendData("AT+CWJAP=\"" + SSID + "\",\"" + Password + "\"");

        // Need 6 seconds after connecting for IP 
        Thread.Sleep(6000);

        // Get IP 
        SendData("AT+CIFSR");

        Thread.Sleep(CommandDelay);
    }

    /// <summary>
    ///     ''' Put Esp8266 in AP mode
    ///     ''' </summary>
    public static void StartAPMode()
    {
        CurrentMode =(int) Mode.AP;

        Thread.Sleep(CommandDelay);

        // Puts chip in AP Mode with no security
        // Default IP address is 192.168.4.1
        // Default SSID varies with chip 
        // To set SSID, Password etc. 
        // https://github.com/espressif/ESP8266_AT/wiki/CWSAP

        SendData("AT+CWMODE=3");

        Thread.Sleep(6000);

        // Get the AP IP address
        ESP8266.SendData("AT+CIPAP?");

        Thread.Sleep(CommandDelay);
    }

    /// <summary>
    ///     ''' Put ESP8266 in server mode
    ///     ''' </summary>
    public static void StartServer()
    {
        try
        {
            CurrentMode = (int)Mode.TCPServer;

            Thread.Sleep(3000);

            // Multiple connections
            SendData("AT+CIPMUX=1");

            Thread.Sleep(CommandDelay);

            SendData("AT+CIPSERVER=1,80");

            Thread.Sleep(CommandDelay);
        }
        catch (Exception ex)
        {
            PrintData("Error: StartServer: " + ex.ToString());
        }
    }

    /// <summary>
    ///     ''' Returns True if a string is part of another
    ///     ''' </summary>
    private static bool InString(string String1, string StringToFind)
    {
        try
        {
            if (String1 == string.Empty)
                return false;

            if (StringToFind == string.Empty)
                return false;

            String1 = String1.ToUpper();

            StringToFind = StringToFind.ToUpper();

            if (String1.IndexOf(StringToFind) == -1)
                return false;
            else
                return true;
        }
        catch
        {
            return false;
        }
    }
}
