using System;

namespace XArch.AppShell.Controls.Console
{
    public class ConsoleEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public string Formatted => $"[{Timestamp:HH:mm:ss}] {Message}";
    }
}
