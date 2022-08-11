using Newtonsoft.Json;
using System;
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
                        instance.TicketList[i].Sefer.GidisTarihi = "10.08.2022";
                        instance.TicketList[i].Sefer.DonusTarihi = "12.08.2022";

                        System.IO.File.AppendAllText("Work/queue.txt", DateTime.Now + " - " + instance.TicketList[i].Update.Message.Chat.Id + " - " + instance.TicketList[i].Update.Message.Chat.Username + " - " + instance.TicketList[i].Update.Message.Text + Environment.NewLine);

                        if (DateTime.Parse(instance.TicketList[i].Sefer.GidisTarihi) > DateTime.Now && DateTime.Parse(instance.TicketList[i].Sefer.DonusTarihi) > DateTime.Now)
                        {
                            await ticket.Search(instance.TicketList[i]);
                        }
                        else
                        {
                            instance.TicketList.Remove(instance.TicketList[i]);
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
