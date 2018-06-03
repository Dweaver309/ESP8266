
Imports System.Text
Imports Microsoft.SPOT.Hardware
Imports Microsoft.VisualBasic
Imports System.Threading
Imports System.IO.Ports
Imports Microsoft.VisualBasic.Constants

Public Class ESP8266

    Private Shared Port As SerialPort

    ' Get the debugging data in the DataReceivedEvent
    Public Shared Data As String = String.Empty

    ' Raised when data is received from serial port
    Public Shared Event DataReceivedEvent()

    ' Returned in the ServerRequestEvent Handler
    Public Shared ServerRequest As String = String.Empty

    ' Raised when server request is made
    Public Shared Event ServerRequestEvent()

    ' Raised when AP server request is made
    Public Shared Event APServerRequestEvent()

    ' Used to start polling the serial port in Initialize  
    Private Shared PollThread As Thread = New Thread(AddressOf PollSerialPort)

    ' Controls the bytes send with a server request
    Const MAX_BUFF As Integer = 1024

    ' Used when polling serial port 
    Private Shared CurrentMode As Integer = Mode.Connect

    Enum Mode
        Connect
        TCPServer
        TCPClient
        AP
    End Enum

    ' Used to Parse date string in GetTimeFromString
    Structure StrDate
        Public Shared Year As Integer
        Public Shared Month As Integer
        Public Shared Day As Integer
        Public Shared Hour As Integer
        Public Shared Minute As Integer
        Public Shared Second As Integer
    End Structure

    ' Set to false to stop sending text to the console
    Private Shared Debugging As Boolean = True

    ' Set after StartServer 
    Public Shared IPAddress As String = "Connecting!"

    ' Hours to adjust GMT time
    Public Shared LocalTimeOffSet As Integer = -5

    ' Pause time in MS
    Public Shared CommandDelay As Integer = 6000

    ' Updated in GetLinkedID
    Private Shared LinkedID As String = "0"

    ''' <summary>
    ''' GH_PD must be pulled high to enable the signal
    ''' GPIO0 must be pulled high, if set to low the chip will be in flash boot mode 
    ''' Toggle the pins to reset the chip
    ''' Open the serial port and start polling for data
    ''' Set chip to Station mode 
    ''' </summary>
    ''' <param name="GIO0CpuPin"></param>
    ''' <param name="CH_PDCpuPin"></param>
    ''' <param name="PortToOpen</param> ' Example COM1 or COM2
    Public Shared Sub Initialize(GIO0CpuPin As Cpu.Pin, CH_PDCpuPin As Cpu.Pin, PortToOpen As String)

        Dim GPIO0 As OutputPort = New OutputPort(GIO0CpuPin, False)

        Dim CH_PD As OutputPort = New OutputPort(CH_PDCpuPin, False)

        Thread.Sleep(1000)

        GPIO0.Write(True)

        CH_PD.Write(True)

        Thread.Sleep(1000)

        Port = New SerialPort(PortToOpen, 115200, Parity.None, 8, StopBits.One)

        Port.Open()

        Port.Flush()

        PollThread.Start()

        ' Set to station to connect to router
        SendData("AT+CWMODE=1")

        Thread.Sleep(CommandDelay)


    End Sub

    ''' <summary>
    ''' Set chip to TCP mode 
    ''' Pause for the response from Google 
    ''' Close the connection
    ''' </summary>
    Public Shared Sub SetTime()

        CurrentMode = Mode.TCPClient

        Dim StringToSend As String = "HEAD / HTTP/1.1" & vbCrLf &
             "Host: www.google.com" & vbCrLf &
             "Accept */*" & vbCrLf &
             "User-Agent: Mozilla/4.0 (compatible; esp8266 Lua;)" & vbCrLf & vbCrLf

        SendData("AT+CIPMUX=1")

        Thread.Sleep(CommandDelay)

        SendData("AT+CIPSTART=0,""TCP"",""www.google.com"",80")

        Thread.Sleep(CommandDelay)

        SendData("AT+CIPSENDBUF=0," & StringToSend.Length + 1)

        Thread.Sleep(2000)

        SendData(StringToSend)

        Thread.Sleep(CommandDelay)

        SendData("AT+CIPCLOSE=0")

    End Sub

    ''' <summary>
    ''' Parse the string and set time and date
    ''' Uses the LocalTimeOffSet property to set the local time
    ''' </summary>
    ''' <param name="IPDString"></param>  
    ''' Called from PollingSerialPort
    Private Shared Sub GetTimeFromString(IPDString As String)

        Try

            Dim DateString As String = String.Empty

            Dim Lines() As String

            Lines = IPDString.Split(CType(Constants.vbCrLf, Char()))

            If Lines.Length > 0 Then

                For i = 0 To Lines.Length - 1

                    'Debug.Print("Line " & i.ToString & " " & Lines(i))
                    PrintData("Line " & i.ToString & " " & Lines(i).ToString)

                    If InString(Lines(i), "Date:") Then

                        DateString = Lines(i)

                        Exit For

                    End If

                Next

                If DateString.Length > 1 Then

                    PrintData("The Date is " & DateString)

                End If

                Dim Space As Char = Strings.ChrW(32)

                Dim Colon As Char = Strings.ChrW(58)

                Dim DateLines() As String

                Dim HMSString As String = String.Empty

                DateLines = DateString.Split(Space)

                For i = 0 To DateLines.Length - 1

                    If i = 2 Then StrDate.Day = CInt(DateLines(i))

                    If i = 3 Then StrDate.Month = GetMonthFromString(DateLines(i))

                    If i = 4 Then StrDate.Year = CInt(DateLines(i))

                    If i = 5 Then

                        HMSString = DateLines(i)

                    End If

                Next

                DateLines = HMSString.Split(Colon)

                If DateLines.Length > 2 Then

                    StrDate.Hour = CInt(DateLines(0))

                    StrDate.Minute = CInt(DateLines(1))

                    StrDate.Second = CInt(DateLines(2))

                End If

                Dim UTCdate As New DateTime(StrDate.Year, StrDate.Month, StrDate.Day, StrDate.Hour, StrDate.Minute, StrDate.Second)

                Dim LocalDate As DateTime = UTCdate

                LocalDate = LocalDate.AddHours(LocalTimeOffSet)

                Utility.SetLocalTime(LocalDate)

                PrintData(DateTime.Now.ToString)

            End If

        Catch ex As Exception

            PrintData("Error: GetTimeFromString: " & ex.ToString)

        End Try

    End Sub

    ''' <summary>
    ''' Called from GetTimeFromString
    ''' </summary>
    Private Shared Function GetMonthFromString(CurrentMonth As String) As Integer

        If InString(CurrentMonth, "Jan") Then
            Return 1
        End If

        If InString(CurrentMonth, "Feb") Then
            Return 2
        End If

        If InString(CurrentMonth, "Mar") Then
            Return 3
        End If

        If InString(CurrentMonth, "Apr") Then
            Return 4
        End If

        If InString(CurrentMonth, "May") Then
            Return 5
        End If

        If InString(CurrentMonth, "Jun") Then
            Return 6
        End If

        If InString(CurrentMonth, "Jul") Then
            Return 7
        End If

        If InString(CurrentMonth, "Aug") Then
            Return 8
        End If

        If InString(CurrentMonth, "Sep") Then
            Return 9
        End If

        If InString(CurrentMonth, "Oct") Then
            Return 10
        End If

        If InString(CurrentMonth, "Nov") Then
            Return 11
        End If

        If InString(CurrentMonth, "Dec") Then
            Return 12
        End If

        Return 1

    End Function

    ''' <summary>
    ''' Esp8266 can have up to 4 connections
    ''' Get the current connection
    ''' Called from PollSerialPort
    ''' </summary>
    Private Shared Function GetLinkID(IPDString As String) As String

        If InString(IPDString, "+IPD,0") Then Return "0"

        If InString(IPDString, "+IPD,1") Then Return "1"

        If InString(IPDString, "+IPD,3") Then Return "2"

        If InString(IPDString, "+IPD,4") Then Return "3"

        Return "0"

    End Function

    ''' <summary>
    ''' Gets IP Address from string
    ''' Called from PollSerialPort
    ''' </summary>
    ''' <returns>Parsed IP Address</returns>
    Private Shared Function ParseIPAddres(Data As String) As String

        Try

            Dim Str As String = "+CIFSR:STAIP,"

            Dim IpStart As Integer = Data.LastIndexOf(Str) + Str.Length

            IPAddress = Data.Substring(IpStart)

            Dim IPEnd As Integer = IPAddress.IndexOf(vbCrLf)

            IPAddress = IPAddress.Substring(1, IPEnd - 2)

        Catch ex As Exception

            PrintData("Error: ParseIPAddress: " & ex.ToString)

        End Try

        Return IPAddress

    End Function

    ''' <summary>
    ''' Polling is started in Initialize
    ''' </summary>
    Private Shared Sub PollSerialPort()

        Try

            Dim Buffer() As Byte

            While True

                If Port.BytesToRead > 1 Then

                    Thread.Sleep(20)

                    ReDim Buffer(Port.BytesToRead)

                    Port.Read(Buffer, 0, Buffer.Length)

                    Dim Chars() As Char = System.Text.Encoding.UTF8.GetChars(Buffer)

                    For i = 0 To Chars.Length - 1

                        Data += Chars(i)

                    Next


                    Select Case CurrentMode

                        Case Mode.Connect

                            ' Get IP 
                            If InString(Data, "+CIFSR:STAIP,") Then

                                IPAddress = ParseIPAddres(Data)

                            End If

                        Case Mode.TCPServer, Mode.AP

                            If InString(Data, "+IPD") Then

                                ServerRequest = Data

                                ' Get the current connection linked ID
                                ' Used to send the requested data in SendServerRequest
                                LinkedID = GetLinkID(ServerRequest)

                                If CurrentMode = Mode.TCPServer Then

                                    RaiseEvent ServerRequestEvent()

                                End If

                                If CurrentMode = Mode.AP Then

                                    RaiseEvent APServerRequestEvent()

                                End If

                            End If

                        Case Mode.TCPClient

                            'Returned request from Google
                            If InString(Data, "+IPD") Then

                                GetTimeFromString(Data)

                            End If


                    End Select

                    RaiseEvent DataReceivedEvent()

                    Data = String.Empty

                End If

                Thread.Sleep(100)

            End While

        Catch ex As Exception

            PrintData("Error: PollSerialPort: " & ex.ToString)

        End Try

    End Sub

    ''' <summary>
    ''' Sends the data requested 
    ''' </summary>
    Public Shared Sub SendServerRequest(ServerRequestedData As String)

        Try

            Dim FileLength As Integer = ServerRequestedData.Length

            Dim BytesSent As Integer = 0

            While BytesSent < FileLength

                Dim BytesToRead As Integer = FileLength - BytesSent

                If BytesToRead > MAX_BUFF Then

                    BytesToRead = MAX_BUFF

                End If

                ESP8266.SendData("AT+CIPSENDBUF=" & LinkedID & "," & BytesToRead)

                Thread.Sleep(20)

                Dim DataToSend As String = ServerRequestedData.Substring(BytesSent, BytesToRead)

                ESP8266.SendData(DataToSend)

                BytesSent += BytesToRead

            End While

            Thread.Sleep(20)

            SendData("AT+CIPCLOSE=" & LinkedID)

        Catch ex As Exception

            PrintData("Error: SendServerRequest: " & ex.ToString)

        End Try

    End Sub

    ''' <summary>
    ''' Declared Public use to send ANY AT commands
    ''' Pause after each AT command is sent
    ''' Converts the string to bytes
    ''' Sends the data to the serial port
    ''' </summary>
    Public Shared Sub SendData(StrData As String)

        Try

            ' Clean it out
            Port.Flush()

            Dim Buffer As Byte() = Encoding.UTF8.GetBytes(StrData & vbCrLf)

            Port.Write(Buffer, 0, Buffer.Length)

            PrintData("Sent: " & StrData)

        Catch ex As Exception

            PrintData("Error: SendData: " & ex.ToString)

        End Try


    End Sub

    ''' <summary>
    ''' Displays string to console when Debugging is set to True
    ''' </summary>
    Public Shared Sub PrintData(Str As String)

        If Debugging Then

            Debug.Print(Str)

        End If

    End Sub

    ''' <summary>
    ''' Chip information
    ''' </summary>
    Public Shared Sub GetVersion()

        SendData("AT+GMR")

        Thread.Sleep(CommandDelay)

    End Sub

    ''' <summary>
    ''' Display the routers that can be connected to
    ''' </summary>
    Public Shared Sub ListAccessPoints()

        SendData("AT+CWLAP")

        Thread.Sleep(CommandDelay)

    End Sub

    ''' <summary>
    ''' Connect to the router and get the IP address
    ''' IPAddress is parsed from PollSerialPort after connecting to router
    ''' </summary>
    Public Shared Sub Connect(SSID As String, Password As String)

        SendData("AT+CWJAP=""" & SSID & """,""" & Password & """")

        ' Need 6 seconds after connecting for IP 
        Thread.Sleep(6000)

        ' Get IP 
        SendData("AT+CIFSR")

        Thread.Sleep(CommandDelay)

    End Sub

    ''' <summary>
    ''' Put Esp8266 in AP mode
    ''' </summary>
    Public Shared Sub StartAPMode()

        CurrentMode = Mode.AP

        Thread.Sleep(CommandDelay)

        ' Puts chip in AP Mode with no security
        ' Default IP address is 192.168.4.1
        ' Default SSID varies with chip 
        ' To set SSID, Password etc. 
        ' https://github.com/espressif/ESP8266_AT/wiki/CWSAP

        SendData("AT+CWMODE=3")

        Thread.Sleep(6000)

        ' Get the AP IP address
        ESP8266.SendData("AT+CIPAP?")

        Thread.Sleep(CommandDelay)

    End Sub

    ''' <summary>
    ''' Put ESP8266 in server mode
    ''' </summary>
    Public Shared Sub StartServer()

        Try

            CurrentMode = Mode.TCPServer

            Thread.Sleep(3000)

            ' Multiple connections
            SendData("AT+CIPMUX=1")

            Thread.Sleep(CommandDelay)

            SendData("AT+CIPSERVER=1,80")

            Thread.Sleep(CommandDelay)

        Catch ex As Exception

            PrintData("Error: StartServer: " & ex.ToString)

        End Try

    End Sub

    ''' <summary>
    ''' Returns True if a string is part of another
    ''' </summary>
    Private Shared Function InString(ByVal String1 As String, ByVal StringToFind As String) As Boolean

        Try

            If String1 = String.Empty Then Return False

            If StringToFind = String.Empty Then Return False

            String1 = String1.ToUpper

            StringToFind = StringToFind.ToUpper

            If String1.IndexOf(StringToFind) = -1 Then

                Return False

            Else

                Return True

            End If

        Catch

            Return False

        End Try

    End Function

End Class