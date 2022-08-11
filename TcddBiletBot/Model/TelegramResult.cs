using System;
using System.Collections.Generic;
using System.Text;

namespace TcddBiletBot.Model
{
    public class TelegramResult
    {
        public bool IsResultNull { get; set; } = false;
        public string Message { get; set; }
    }
}
