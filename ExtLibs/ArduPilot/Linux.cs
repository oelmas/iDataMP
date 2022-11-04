using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities
{
    public class Linux
    {
        public static List<DeviceInfo> GetAllCOMPorts()
        {
            var ans = new List<DeviceInfo>();
            //var proc = System.Diagnostics.Process.Start("bash", @"-c ""lsusb -tv > /tmp/lsusb.list""");
            //proc.WaitForExit();


            try
            {
                var proc = Process.Start("/usr/bin/bash",
                    @"-c ""/usr/bin/find /sys/bus/usb/devices/usb*/ -name dev | /usr/bin/grep tty > /tmp/usb.list""");
                proc.WaitForExit();

                var data = File.ReadAllLines("/tmp/usb.list");
                Console.WriteLine(data);
                foreach (var device in data)
                    try
                    {
                        var pth = Path.Combine(Path.GetDirectoryName(device), "../../../");
                        var product = File.ReadAllText(pth + "product").TrimEnd();

                        ans.Add(new DeviceInfo
                        {
                            board = product,
                            description = product,
                            name = product,
                            hardwareid =
                                $"USB\\VID_{File.ReadAllText(pth + "idVendor").TrimEnd()}&PID_{File.ReadAllText(pth + "idProduct").TrimEnd()}"
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return ans;
        }
    }
}