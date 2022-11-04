using System.Text;
using Newtonsoft.Json;

public static class Extension
{
    public static string ToJSON(this MAVLink.MAVLinkMessage msg)
    {
        return JsonConvert.SerializeObject(msg);
    }

    public static MAVLink.MAVLinkMessage FromJSON(this string msg)
    {
        return JsonConvert.DeserializeObject<MAVLink.MAVLinkMessage>(msg);
    }


    public static string WrapText(this string msg, int length, char[] spliton)
    {
        var ans = new StringBuilder();
        var linecha = 0;
        for (var i = 0; i < msg.Length; i++)
        {
            var splitline = false;
            if (linecha > length)
                foreach (var cha in spliton)
                    if (msg[i] == cha)
                    {
                        ans.Append(msg[i]);
                        ans.Append("\n");
                        splitline = true;
                        linecha = -1;
                        break;
                    }

            if (!splitline)
                ans.Append(msg[i]);

            linecha++;
        }

        return ans.ToString();
    }
}