using System;
using Microsoft.SPOT;
// AT Command Reference 
// https://github.com/espressif/ESP8266C_AT/wiki/


using Microsoft.VisualBasic;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace ESP8266WiFI

{
    public class Program
    {
        // Pin connected to ESP8266C GPIO0 pin
        public const Cpu.Pin PA13 = (Cpu.Pin)((0 * 16) + 13);

        // Pin connected to ESP8266C CH_PD pin
        public const Cpu.Pin PA14 = (Cpu.Pin)((0 * 16) + 14);

        public static void Main()
        {
                       
            // Debug.Print(Resources.GetString(Resources.StringResources.String1));
            ESP8266.DataReceivedEvent += DataReceivedHandler;

            ESP8266.ServerRequestEvent += ServerRequestHandler;

            ESP8266.APServerRequestEvent += APServerRequestHandler;

            // Default is 6000 MS
            ESP8266.CommandDelay = 3000;

            // Default is -5 for US central time offset from GMT time
            ESP8266.LocalTimeOffSet = -5;

            ESP8266.Initialize(PA13, PA14, "COM2");

            ESP8266.GetVersion();

            ESP8266.Connect("weaver", "3098280065");

            ESP8266.SetTime();

            ESP8266.PrintData(DateTime.Now.ToString("dd MMM HH:mm"));

            ESP8266.StartServer();

            ESP8266.PrintData("IP Address: " + ESP8266.IPAddress);

            // ********* Uncomment to start Access Point mode
            // ESP8266C.StartAPMode()

            while ((true))

                Thread.Sleep(1000);
        }
        /// <summary>
        ///         ''' Called from ServerRequestHandler
        ///         ''' </summary>
        public static string ProcessServerRequest(string Request)
        {

            // ****** Parse Request here

            string ReturnString = WebPages.DefaultPage();


            return ReturnString + Constants.vbCrLf + Constants.vbCrLf;
        }

        /// <summary>
        ///         ''' Called from APServerRequestHandler
        ///         ''' </summary>
        public static string ProcessAPServerRequest(string Request)
        {

            // ****** Parse Request here

            string ReturnString = WebPages.StatusPage("ESP8266C in AP Mode");

            return ReturnString + Constants.vbCrLf + Constants.vbCrLf;
        }

        /// <summary>
        ///         ''' Get the AP server request
        ///         ''' Process the request
        ///         ''' Send the AP server request
        ///         ''' </summary>
        public static void APServerRequestHandler()
        {
            ESP8266.PrintData("AP Server Request");

            string ReturnString = ProcessAPServerRequest(ESP8266.ServerRequest);

            ESP8266.SendServerRequest(ReturnString);
        }

        /// <summary>
        ///         ''' Get the server request
        ///         ''' Process the request
        ///         ''' Send the server request
        ///         ''' </summary>
        public static void ServerRequestHandler()
        {
            ESP8266.PrintData("Server Request");

            string ReturnString = ProcessServerRequest(ESP8266.ServerRequest);

            ESP8266.SendServerRequest(ReturnString);
        }

        /// <summary>
        ///         ''' Get the serial port received data from the ESP8266C device
        ///         ''' Print to the console
        ///         ''' </summary>
        public static void DataReceivedHandler()
        {
            ESP8266.PrintData("Received: " + ESP8266.Data);
        }
    }
}


