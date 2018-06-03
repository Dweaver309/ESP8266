
using Microsoft.VisualBasic;

public class WebPages
{
    public static string DefaultPage()
    {
        string t = string.Empty;
        t += Constants.vbCrLf + "<html>";
        t += Constants.vbCrLf + "<head>";
        t += Constants.vbCrLf + "<meta http-equiv=\"Content-Language\" content=\"en-us\">";
        t += Constants.vbCrLf + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\">";
        t += Constants.vbCrLf + "<meta name=\"theme-color\" content=\"#ffffff\">";
        t += Constants.vbCrLf + "<span style=\"color:Gray;background-color:Transparent;border-color:White;font-family:Arial;font-size:Medium; left: 32px; position: absolute; top: 8px\">";
        t += Constants.vbCrLf + "</span>";
        t += Constants.vbCrLf + "<title>Irrigation Controler</title>";
        t += Constants.vbCrLf + "<meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0; maximum-scale=1.0;\">";
        t += Constants.vbCrLf + "</head>";
        t += Constants.vbCrLf + "<body>";
        t += Constants.vbCrLf + "<form method=\"GET\" action=\"default.html\">";
        t += Constants.vbCrLf + "	<p align=\"left\"><font face=\"Arial\" size=\"4\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ";
        t += Constants.vbCrLf + "	Tiny Rain Maker</font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Zone 1:&nbsp;&nbsp; </font><input type=\"submit\" value=\"On\" name=\"Zone1On\">&nbsp;&nbsp;&nbsp;";
        t += Constants.vbCrLf + "	<font face=\"Arial\">";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Off\" name=\"Zone1Off\">&nbsp;&nbsp; </font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; Zone 2:&nbsp;&nbsp; </font>";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"On\" name=\"Zone2On\">&nbsp; &nbsp;";
        t += Constants.vbCrLf + "	<font face=\"Arial\">";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Off\" name=\"Zone2Off\">&nbsp;&nbsp;</font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Zone 3:&nbsp;&nbsp; </font>";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"On\" name=\"Zone3On\">&nbsp;&nbsp;&nbsp;";
        t += Constants.vbCrLf + "	<font face=\"Arial\">";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Off\" name=\"Zone3Off\">&nbsp;&nbsp; </font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Zone 4:&nbsp;&nbsp; </font>";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"On\" name=\"Zone4On\">&nbsp;&nbsp;&nbsp;";
        t += Constants.vbCrLf + "	<font face=\"Arial\">";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Off\" name=\"Zone4Off\">&nbsp;&nbsp; </font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp; Program:&nbsp;&nbsp; </font>";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"On\" name=\"ProgramOn\">&nbsp;&nbsp;&nbsp;";
        t += Constants.vbCrLf + "	<font face=\"Arial\">";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Off\" name=\"ProgramOff\"></font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Settings\" name=\"Settings\"></font></p>";
        t += Constants.vbCrLf + "	<p><font face=\"Arial\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;";
        t += Constants.vbCrLf + "	<input type=\"submit\" value=\"Awning \" name=\"Awning\"></font></p>";
        t += Constants.vbCrLf + "	</form>";
        t += Constants.vbCrLf + "<p>&nbsp;</p>";
        t += Constants.vbCrLf + "</body>";
        t += Constants.vbCrLf + "</html>";

        return t;
    }

    public static string StatusPage(string Status)
    {
        string t = string.Empty;
        t = t + "<html>";
        t = t + "<head>";
        t = t + "<meta http-equiv=\"Content-Language\" content=\"en-us\">";
        t = t + "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1252\">";
        t = t + "<title>Irrigation Controler</title>";
        t = t + "<meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0; maximum-scale=1.0;\">";
        t = t + "</head>";
        t = t + "<body>";
        t = t + "<h1>" + Status + "</h1>";
        t = t + "</body>";
        t = t + "</html>";


        return t;
    }
}
