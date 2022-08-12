using Newtonsoft.Json;
using System;
using TcddBiletBot.TelegramBot;
using TcddBiletBot.Ticket;

namespace TcddBiletBot.Helper
{
    public static class TimerTask
    {
        public static async void SearchTicket(object state)
        {
            try
            {
                System.IO.File.Delete("Work/queue.txt");

                System.IO.File.Create("Work/queue.txt").Close();

                var instance = TicketListSingleton.Instance;
                TCDDTicket ticket = new TCDDTicket();

                if (instance.TicketList.Count > 0)
                {
                    for (int i = 0; i < instance.TicketList.Count; i++)
                    {
                        System.IO.File.AppendAllText("Work/queue.txt", DateTime.Now + " - " + instance.TicketList[i].Update.Message.Chat.Id + " - " + instance.TicketList[i].Update.Message.Chat.Username + " - " + instance.TicketList[i].Update.Message.Text + Environment.NewLine);

                        if (DateTime.Parse(instance.TicketList[i].Sefer.GidisTarihi) > DateTime.Now && DateTime.Parse(instance.TicketList[i].Sefer.DonusTarihi) > DateTime.Now)
                        {
                            await ticket.Search(instance.TicketList[i]);
                        }
                        else
                        {
                            Send send = new Send();
                         
                            instance.TicketList.Remove(instance.TicketList[i]);
                          await send.MessageOnlyClientId(instance.TicketList[i].Update.Message.Chat.Id,"Tarih Formatı yanlış! İşleminiz durduruldu!");
                        }
                    }

                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata => " + ex.Message);
            }
        }

    }
}
