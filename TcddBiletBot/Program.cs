using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TcddBiletBot.TelegramBot;
using TcddBiletBot.Ticket;
using TcddBiletBot.Model;
using System.Runtime.InteropServices;
namespace TcddBiletBot
{
    public class Program
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {

            var bot = new HandleBot();
            CancellationTokenSource cts = new CancellationTokenSource();
#if DEBUG
            IntPtr hWnd = GetConsoleWindow();
            ShowWindow(hWnd, 1);

            var botClient = new TelegramBotClient("5571258832:AAFXNVv5O6DVZGBRIRthPM47mt_AFpotVaU");
#else
IntPtr hWnd = GetConsoleWindow();
            ShowWindow(hWnd, 0);

var botClient = new TelegramBotClient("5194916816:AAHFHM-qULbrsZvsAokjgkbKVwgLgiQ8rhM");
#endif

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
                ThrowPendingUpdates = true
            };

            botClient.StartReceiving(
            bot.HandleUpdateAsync,
            bot.HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);

            Timer timer = new Timer(Helper.TimerTask.SearchTicket, null, 0, 60000);

            Console.WriteLine("Bot Başladı." + Environment.NewLine + "Durdurmak İçin Bir Tuşa Bas");
            Console.ReadLine();
        }
    }
}
