'  AT Command Reference 
' https://github.com/espressif/ESP8266_AT/wiki/

Option Explicit On
Option Strict On

Imports Microsoft.SPOT.Hardware
Imports System.Threading
Imports Microsoft.VisualBasic.Constants

Namespace ESP8266WiFi

    Public Module Module1

        ' Pin connected to ESP8266 GPIO0 pin
        Public Const PA13 As Cpu.Pin = CType(((0 * 16) + 13), Cpu.Pin)

        ' Pin connected to ESP8266 CH_PD pin
        Public Const PA14 As Cpu.Pin = CType(((0 * 16) + 14), Cpu.Pin)

        Sub Main()

            AddHandler ESP8266.DataReceivedEvent, AddressOf DataReceivedHandler

            AddHandler ESP8266.ServerRequestEvent, AddressOf ServerRequestHandler

            AddHandler ESP8266.APServerRequestEvent, AddressOf APServerRequestHandler

            ' Default is 6000 MS
            ESP8266.CommandDelay = 3000

            ' Default is -5 for US central time offset from GMT time
            ESP8266.LocalTimeOffSet = -5

            ESP8266.Initialize(PA13, PA14, "COM2")

            ESP8266.GetVersion()

            ESP8266.Connect("weaver", "3098280065")

            ESP8266.SetTime()

            ESP8266.PrintData(DateTime.Now.ToString("dd MMM HH:mm"))

            ESP8266.StartServer()

            ESP8266.PrintData("IP Address: " & ESP8266.IPAddress)

            '********* Uncomment to start Access Point mode
            'ESP8266.StartAPMode()

            While (True)

                Thread.Sleep(1000)

            End While

        End Sub

        ''' <summary>
        ''' Called from ServerRequestHandler
        ''' </summary>
        Function ProcessServerRequest(Request As String) As String

            ' ****** Parse Request here

            Dim ReturnString As String = WebPages.DefaultPage

            Return ReturnString & vbCrLf & vbCrLf

        End Function

        ''' <summary>
        ''' Called from APServerRequestHandler
        ''' </summary>
        Function ProcessAPServerRequest(Request As String) As String

            ' ****** Parse Request here

            Dim ReturnString As String = WebPages.StatusPage("ESP8266 in AP Mode")

            Return ReturnString & vbCrLf & vbCrLf

        End Function

        ''' <summary>
        ''' Get the AP server request
        ''' Process the request
        ''' Send the AP server request
        ''' </summary>
        Sub APServerRequestHandler()

            ESP8266.PrintData("AP Server Request")

            Dim ReturnString As String = ProcessAPServerRequest(ESP8266.ServerRequest)

            ESP8266.SendServerRequest(ReturnString)

        End Sub

        ''' <summary>
        ''' Get the server request
        ''' Process the request
        ''' Send the server request
        ''' </summary>
        Sub ServerRequestHandler()

            ESP8266.PrintData("Server Request")

            Dim ReturnString As String = ProcessServerRequest(ESP8266.ServerRequest)

            ESP8266.SendServerRequest(ReturnString)

        End Sub

        ''' <summary>
        ''' Get the serial port received data from the ESP8266 device
        ''' Print to the console
        ''' </summary>
        Sub DataReceivedHandler()

            ESP8266.PrintData("Received: " & ESP8266.Data)

        End Sub

    End Module

End Namespace