using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUIApp.Comm;

namespace GUIApp
{
    internal class Utils
    {
        public static string FormatStatusMessage(SimulatorStatus status)
        {
            if (status == null)
            {
                return "Status: <null>";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Simulator Status ===");
            sb.AppendLine($"Uptime:     {status.UptimeSeconds} seconds ({FormatUptime(status.UptimeSeconds)})");
            sb.AppendLine($"IP Address: {status.IpAddress}");
            sb.AppendLine($"Port:       {status.Port}");
            sb.AppendLine("========================");

            return sb.ToString();
        }

        private static string FormatUptime(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            if (timeSpan.TotalDays >= 1)
            {
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            else if (timeSpan.TotalHours >= 1)
            {
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            else if (timeSpan.TotalMinutes >= 1)
            {
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            else
            {
                return $"{timeSpan.Seconds}s";
            }
        }
    }
}
