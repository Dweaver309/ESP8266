Imports System
Imports Microsoft.SPOT
Imports Microsoft.VisualBasic.Constants
Public Class WebPages

    Public Shared Function DefaultPage() As String

        Dim t As String = String.Empty
        t += vbCrLf & "<html>"
        t += vbCrLf & "<head>"
        t += vbCrLf & "<meta http-equiv=""Content-Language"" content=""en-us"">"
        t += vbCrLf & "<meta http-equiv=""Content-Type"" content=""text/html; charset=windows-1252"">"
        t += vbCrLf & "<meta name=""theme-color"" content=""#ffffff"">"
        t += vbCrLf & "<span style=""color:Gray;background-color:Transparent;border-color:White;font-family:Arial;font-size:Medium; left: 32px; position: absolute; top: 8px"">"
        t += vbCrLf & "</span>"
        t += vbCrLf & "<title>Irrigation Controler</title>"
        t += vbCrLf & "<meta name=""viewport"" content=""width=device-width; initial-scale=1.0; maximum-scale=1.0;"">"
        t += vbCrLf & "</head>"
        t += vbCrLf & "<body>"
        t += vbCrLf & "<form method=""GET"" action=""default.html"">"
        t += vbCrLf & "	<p align=""left""><font face=""Arial"" size=""4"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; "
        t += vbCrLf & "	Tiny Rain Maker</font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Zone 1:&nbsp;&nbsp; </font><input type=""submit"" value=""On"" name=""Zone1On"">&nbsp;&nbsp;&nbsp;"
        t += vbCrLf & "	<font face=""Arial"">"
        t += vbCrLf & "	<input type=""submit"" value=""Off"" name=""Zone1Off"">&nbsp;&nbsp; </font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; Zone 2:&nbsp;&nbsp; </font>"
        t += vbCrLf & "	<input type=""submit"" value=""On"" name=""Zone2On"">&nbsp; &nbsp;"
        t += vbCrLf & "	<font face=""Arial"">"
        t += vbCrLf & "	<input type=""submit"" value=""Off"" name=""Zone2Off"">&nbsp;&nbsp;</font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Zone 3:&nbsp;&nbsp; </font>"
        t += vbCrLf & "	<input type=""submit"" value=""On"" name=""Zone3On"">&nbsp;&nbsp;&nbsp;"
        t += vbCrLf & "	<font face=""Arial"">"
        t += vbCrLf & "	<input type=""submit"" value=""Off"" name=""Zone3Off"">&nbsp;&nbsp; </font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Zone 4:&nbsp;&nbsp; </font>"
        t += vbCrLf & "	<input type=""submit"" value=""On"" name=""Zone4On"">&nbsp;&nbsp;&nbsp;"
        t += vbCrLf & "	<font face=""Arial"">"
        t += vbCrLf & "	<input type=""submit"" value=""Off"" name=""Zone4Off"">&nbsp;&nbsp; </font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp; Program:&nbsp;&nbsp; </font>"
        t += vbCrLf & "	<input type=""submit"" value=""On"" name=""ProgramOn"">&nbsp;&nbsp;&nbsp;"
        t += vbCrLf & "	<font face=""Arial"">"
        t += vbCrLf & "	<input type=""submit"" value=""Off"" name=""ProgramOff""></font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;"
        t += vbCrLf & "	<input type=""submit"" value=""Settings"" name=""Settings""></font></p>"
        t += vbCrLf & "	<p><font face=""Arial"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;"
        t += vbCrLf & "	<input type=""submit"" value=""Awning "" name=""Awning""></font></p>"
        t += vbCrLf & "	</form>"
        t += vbCrLf & "<p>&nbsp;</p>"
        t += vbCrLf & "</body>"
        t += vbCrLf & "</html>"

        Return t

    End Function

    Public Shared Function StatusPage(Status As String) As String

        Dim t As String = String.Empty
        t = t & "<html>"
        t = t & "<head>"
        t = t & "<meta http-equiv=""Content-Language"" content=""en-us"">"
        t = t & "<meta http-equiv=""Content-Type"" content=""text/html; charset=windows-1252"">"
        t = t & "<title>Irrigation Controler</title>"
        t = t & "<meta name=""viewport"" content=""width=device-width; initial-scale=1.0; maximum-scale=1.0;"">"
        t = t & "</head>"
        t = t & "<body>"
        t = t & "<h1>" & Status & "</h1>"
        t = t & "</body>"
        t = t & "</html>"


        Return t

    End Function


End Class
