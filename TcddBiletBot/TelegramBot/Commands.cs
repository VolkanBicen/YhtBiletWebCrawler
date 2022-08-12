using System;
using System.Threading.Tasks;
using TcddBiletBot.Helper;
using TcddBiletBot.Model;
using TcddBiletBot.Ticket;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace TcddBiletBot.TelegramBot
{
    public class Commands
    {
        public async Task Run(Update update, ITelegramBotClient client)
        {
            Send send = new Send();
            try
            {
                System.IO.File.AppendAllText("Work/requestList.txt", DateTime.Now + " - Id: " + update.Message.Chat.Id + " - Name: " + update.Message.Chat.FirstName + update.Message.Chat.LastName + " - Username: " + update.Message.Chat.Username + "- Message Text: " + update.Message.Text + Environment.NewLine);

                var instance = TicketListSingleton.Instance;

                if (/*!new CheckUser().Check(update.Message.Chat.Id)*/ false)
                {
                    await send.Message(client, update, "Uygulamayı Kullanmak İçin Yetkiniz Yok. Lütfen  @TcddBiletBotAdmin İle " + update.Message.Chat.Id + " Kodunu Göndererek İletişime Geçiniz.");
                    return;
                }

                string commands = "/nasilcalisir" + Environment.NewLine +
                                   "/biletAra" + Environment.NewLine +
                                   "/iptal" + Environment.NewLine +
                                   "/komutlar" + Environment.NewLine +
                                   "/istasyonlistesi";

                string nasilCalisir = "/biletAra komutunu yazdıktan sonra aralarında virgül olacak şekilde" + Environment.NewLine + Environment.NewLine +
                                       "Kalkış istaysonu(Ankara Gar), Varış istasyonu(Eskişehir),Gidis tarihi(01.01.2022),Minumum Gidiş Saati(09:00),Maximum Gidiş Saati(23:00)," +
                                       "Dönüş tarihi(01.01.2022),Minumum Dönüş Saati(09:00),Maxiumum Dönüş Saati(23:00)" + Environment.NewLine + Environment.NewLine + " Yazılır ve mesaj gönderilir!" + Environment.NewLine + Environment.NewLine +
                                       "ÖRN:" + Environment.NewLine + "/biletAra Ankara Gar,Eskişehir,01.01.2022,08:00,09:30,01.01.2022,15:00,19:00" + Environment.NewLine + Environment.NewLine + "İşleminiz başarılı bir şekilde tamamlandıysa /iptal komutuyla botu durdurmayı unutmayınız!";

                string description = "TCDD YHT Biletleri için Alarm oluşturabilirsiniz. Seçtiğiniz güzergahta seçtiğiniz saatler arasında ki tren biletlerini kontrol eder ve bilet bulunduğunda size mesaj atar." + Environment.NewLine + Environment.NewLine + "Önerilerinizi iletmek için @TcddBiletBotAdmin ile iletişime geçebilirsiniz." + Environment.NewLine + Environment.NewLine;

                string stationList = "Ankara" + " " + "Gar" + Environment.NewLine + "Eskişehir" + Environment.NewLine + " Konya" + Environment.NewLine + "İstanbul(Bakırköy)" + Environment.NewLine + "İstanbul(Bostancı)" + Environment.NewLine + "İstanbul(Halkalı)" + Environment.NewLine + " İstanbul(Pendik)" + Environment.NewLine + "İstanbul(Söğütlü Ç.)";

                string[] commandParameters = update.Message.Text.Split(" ");

                if (commandParameters.Length == 1)
                {
                    switch (commandParameters[0].ToLowerInvariant())
                    {
                        case "/start":
                        case "/nasilcalisir":
                            await send.Message(client, update, description + nasilCalisir);
                            await send.Message(client, update, "/biletAra Ankara Gar,Eskişehir,01.01.2022,08:00,09:30,01.01.2022,15:00,19:00");
                            return;
                        case "/biletara":
                            await send.Message(client, update, "Eksik Parametre Girdiniz! " + Environment.NewLine + Environment.NewLine + nasilCalisir);
                            return;
                        case "/komutlar":
                            await send.Message(client, update, commands);
                            return;
                        case "/istasyonlistesi":
                            await send.Message(client, update, stationList);
                            return;
                        case "/iptal":
                            instance.TicketList.RemoveAll(x => x.Update.Message.Chat.Id == update.Message.Chat.Id);
                            await send.Message(client, update, "Tüm İşlemleriniz İptal Edildi!");
                            return;
                        case "/ornek":
                            await send.Message(client, update, "/biletAra Ankara Gar,Eskişehir,01.01.2022,08:00,09:30,01.01.2022,15:00,19:00");
                            return;
                        default:
                            await send.Message(client, update, "Geçersiz Komut!" + Environment.NewLine + commands);
                            return;
                    }
                }

                if (String.Equals(commandParameters[0].ToLowerInvariant(), "/admin"))
                {
                    var split = update.Message.Text.Split(",");
                    update.Message.Chat.Id = Convert.ToInt64(split[split.Length - 1]);
                }

                var result = instance.TicketList.Find(x => x.Update.Message.Chat.Id == update.Message.Chat.Id);
                if (result != null)
                {
                    await send.Message(client, update, "Hala Devam Eden İşleminiz Var! Yeni Bir İşlem İçin Önceki İşlemi İptal Et !");
                    return;
                }

                update.Message.Text = update.Message.Text.Replace(commandParameters[0], "").Trim();
                string[] seferBilgileri = update.Message.Text.Split(",");

                if (!new VoyageControl().Check(seferBilgileri[0], seferBilgileri[1]))
                {
                    await send.Message(client, update, "Biniş-İniş İstasyonunu Kontrol Ediniz! /istasyonlistesi");
                    return;
                }

                if (DateTime.Now.Date.AddDays(10) < DateTime.Parse(seferBilgileri[2]) || DateTime.Now.Date.AddDays(10) < DateTime.Parse(seferBilgileri[5]))
                {
                    await send.Message(client, update, "Sefer saatlerini kontrol edin! Maksimum 10 gün sonrasında ki biletler için botu çalıştırabilirsiniz!");
                    return;
                }

                if (DateTime.Parse(seferBilgileri[2]) < DateTime.Now || DateTime.Parse(seferBilgileri[5]) < DateTime.Now || DateTime.Parse(seferBilgileri[5]) < DateTime.Parse(seferBilgileri[2]))
                {
                    await send.Message(client, update, "Sefer saatlerini kontrol edin! Geçmiş zamanı veya dönüş tarihi gidiş tarihinden önce olamaz!");
                    return;
                }

                Sefer seferModel = new Sefer();
                seferModel.Kalkis = seferBilgileri[0];
                seferModel.Varis = seferBilgileri[1];
                seferModel.GidisTarihi = seferBilgileri[2];
                seferModel.MinGidisSaat = seferBilgileri[3];
                seferModel.MaxGidisSaat = seferBilgileri[4];
                seferModel.DonusTarihi = seferBilgileri[5];
                seferModel.MinDonusSaat = seferBilgileri[6];
                seferModel.MaxDonusSaat = seferBilgileri[7];

                Bilet biletModel = new Bilet();
                biletModel.Client = client;
                biletModel.Sefer = seferModel;
                biletModel.Update = update;

                await send.Message(client, update, "Bot başlatıldı. Her Dakika Bilet Olup Olmadığı Kontrol Edilecek. Biletiniz Bulunduğu Taktirde Size Bilgi Verilecektir.");
                instance.TicketList.Add(biletModel);

                TCDDTicket ticket = new TCDDTicket();
                await ticket.Search(biletModel);

            }
            catch (Exception ex)
            {
                await send.Message(client, update, "Bir Hata Oluştu Sefer Bilgilerini Kontrol Et.");
                Console.WriteLine("Hata => " + ex.Message);
            }

        }
    }
}