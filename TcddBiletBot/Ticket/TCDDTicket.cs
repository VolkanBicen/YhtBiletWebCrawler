using TcddBiletBot.Model;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using TcddBiletBot.TelegramBot;
using TcddBiletBot.Helper;

namespace TcddBiletBot.Ticket
{
    public class TCDDTicket
    {
        public async Task Search(Bilet model)
        {
            try
            {
                Cookies cookies = new Cookies();
                TelegramResult telegramResult = new TelegramResult();

                GetRequest(cookies);
                PostRequest(cookies, model.Sefer);
                PostRequest(cookies, model.Sefer);
                var result = Result(cookies);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(result);

                var outgoingCount = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[1]/div/div/div/table/tbody")[0].ChildNodes.Count;

                for (int i = 0; i < outgoingCount; i++)
                {
                    string seferSaati = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[1]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[1]/span")[0].InnerHtml;
                    if (DateTime.Parse(seferSaati) >= DateTime.Parse(model.Sefer.MinGidisSaat) && DateTime.Parse(seferSaati) <= DateTime.Parse(model.Sefer.MaxGidisSaat))
                    {
                        var economy = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[1]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[5]/div/div[1]/select/option[1]")[0].InnerHtml;
                        var busieness = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[1]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[5]/div/div[1]/select/option[2]")[0].InnerHtml;

                        int economyCount = new EmptySeatsCount().GetEmptySeatsCount(economy);
                        int busienessCount = new EmptySeatsCount().GetEmptySeatsCount(busieness);

                        if (economyCount > 2 || busienessCount > 0)
                        {
                            telegramResult.IsResultNull = true;
                            telegramResult.Message = telegramResult.Message + model.Sefer.GidisTarihi + " Tarihinde " + model.Sefer.Kalkis + " Yönünden " + model.Sefer.Varis + " Yönüne Saat: " + seferSaati + "'da boş koltuk var." + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }

                var incomingCount = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[2]/div/div/div/table/tbody")[0].ChildNodes.Count;

                if (telegramResult.IsResultNull)
                {
                    telegramResult.Message = telegramResult.Message + Environment.NewLine + "**********************************************************" + Environment.NewLine + Environment.NewLine;
                }

                for (int i = 0; i < incomingCount; i++)
                {
                    string seferSaati = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[2]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[1]/span")[0].InnerHtml;
                    if (DateTime.Parse(seferSaati) >= DateTime.Parse(model.Sefer.MinDonusSaat) && DateTime.Parse(seferSaati) <= DateTime.Parse(model.Sefer.MaxDonusSaat))
                    {
                        string economy = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[2]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[5]/div/div[1]/select/option[1]")[0].InnerHtml;
                        string busieness = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[2]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[5]/div/div[1]/select/option[2]")[0].InnerHtml;

                        int economyCount = new EmptySeatsCount().GetEmptySeatsCount(economy);
                        int busienessCount = new EmptySeatsCount().GetEmptySeatsCount(busieness);

                        if (economyCount > 2 || busienessCount > 0)
                        {
                            telegramResult.IsResultNull = true;
                            telegramResult.Message = telegramResult.Message + model.Sefer.DonusTarihi + " Tarihinde " + model.Sefer.Varis + " Yönünden " + model.Sefer.Kalkis + " Yönüne Saat: " + seferSaati + "'da boş koltuk var." + Environment.NewLine + Environment.NewLine + "İşleminiz başarılı bir şekilde tamamlandıysa /iptal komutuyla botu durdurmayı unutmayınız!";
                        }
                    }
                }
                Send send = new Send();
                if (telegramResult.IsResultNull)
                {
                    System.IO.File.AppendAllText("Work/success.txt", DateTime.Now + " - Id: " + model.Update.Message.Chat.Id + " - Name: " + model.Update.Message.Chat.FirstName + model.Update.Message.Chat.Username + "- BİLETİ BULDUK! " + Environment.NewLine);
                    await send.Message(model.Client, model.Update, telegramResult.Message + Environment.NewLine);
                }
                System.IO.File.AppendAllText("Work/unsuccess.txt", DateTime.Now + " - Id: " + model.Update.Message.Chat.Id + " - Name: " + model.Update.Message.Chat.FirstName + model.Update.Message.Chat.Username + "- BİLETİ BULAMADIK! " + Environment.NewLine);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("Work/error.txt", DateTime.Now + " - Id: " + model.Update.Message.Chat.Id + " - Name: " + model.Update.Message.Chat.FirstName + model.Update.Message.Chat.Username + "- Error Text: " + ex.Message + Environment.NewLine);

                await Search(model);
            }
        }
        public string Result(Cookies cookies)
        {

            var baseAddressGet = new Uri("https://ebilet.tcddtasimacilik.gov.tr/view/eybis/tnmGenel/int_sat_001.jsf");
            var cookieContainerGet = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainerGet })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddressGet })
            {
                Cookie JSESSIONID = new Cookie("JSESSIONID", cookies.Jsession);
                Cookie NSC_WJQ = new Cookie("NSC_WJQ_UTN_FZCJT-HJTF_TTM", cookies.NscWjo);
                Cookie NSC_ESNS = new Cookie("NSC_ESNS", cookies.NscEsns);
                Cookie YolcuTabId = new Cookie("yolcuTabId", cookies.YolcuTabId);

                cookieContainerGet.Add(baseAddressGet, JSESSIONID);
                cookieContainerGet.Add(baseAddressGet, YolcuTabId);
                cookieContainerGet.Add(baseAddressGet, NSC_WJQ);
                cookieContainerGet.Add(baseAddressGet, NSC_ESNS);
                var result = client.GetAsync(baseAddressGet).Result.Content.ReadAsStringAsync();

                return result.Result;
            }
        }
        public Cookies GetRequest(Cookies cookies)
        {
            HttpWebRequest requestCookie = (HttpWebRequest)WebRequest.Create("https://ebilet.tcddtasimacilik.gov.tr/view/eybis/tnmGenel/tcddWebContent.jsf");
            requestCookie.CookieContainer = new CookieContainer();
            var response = (HttpWebResponse)requestCookie.GetResponse();

            string source;

            using (response)
            {
                cookies.Jsession = response.Cookies["JSESSIONID"].Value;
                cookies.NscEsns = response.Cookies["NSC_ESNS"].Value;
                cookies.NscWjo = response.Cookies["NSC_WJQ_UTN_FZCJT-HJTF_TTM"].Value;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                source = reader.ReadToEnd();

            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(source);

            cookies.ViewState = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div/div[2]/ul/li[2]/div/form/input[2]")[0].Attributes["value"].Value;
            cookies.YolcuTabId = "yolcuTabId" + DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            return cookies;
        }
        public Cookies PostRequest(Cookies cookies, Sefer model)
        {
            var baseAddress = new Uri("https://ebilet.tcddtasimacilik.gov.tr/view/eybis/tnmGenel/tcddWebContent.jsf");
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");

                var content = new FormUrlEncodedContent(new[]
                {

                new KeyValuePair<string, string>("javax.faces.partial.ajax", "true"),
                new KeyValuePair<string, string>("javax.faces.source", "btnSeferSorgula"),
                new KeyValuePair<string, string>("javax.faces.partial.execute", "btnSeferSorgula biletAramaForm"),

                new KeyValuePair<string, string>("javax.faces.partial.render", "msg biletAramaForm"),
                new KeyValuePair<string, string>("btnSeferSorgula", "btnSeferSorgula"),
                new KeyValuePair<string, string>("biletAramaForm", "biletAramaForm"),

                new KeyValuePair<string, string>("tipradioIslemTipi", "0"),
                new KeyValuePair<string, string>("nereden", model.Kalkis),
                new KeyValuePair<string, string>("trCalGid_input", model.GidisTarihi),

                new KeyValuePair<string, string>("tipradioSeyehatTuru", "0"),
                new KeyValuePair<string, string>("nereye", model.Varis),
                new KeyValuePair<string, string>("trCalDon_input", model.DonusTarihi),

                new KeyValuePair<string, string>("syolcuSayisi_input", "1"),
                new KeyValuePair<string, string>("javax.faces.ViewState", cookies.ViewState),
                });

                content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";
                content.Headers.ContentType.CharSet = "UTF-8";

                Cookie JSESSIONID = new Cookie("JSESSIONID", cookies.Jsession);
                Cookie NSC_WJQ = new Cookie("NSC_WJQ_UTN_FZCJT-HJTF_TTM", cookies.NscWjo);
                Cookie NSC_ESNS = new Cookie("NSC_ESNS", cookies.NscEsns);
                Cookie YolcuTabId = new Cookie("yolcuTabId", cookies.YolcuTabId);

                cookieContainer.Add(baseAddress, JSESSIONID);
                cookieContainer.Add(baseAddress, YolcuTabId);
                cookieContainer.Add(baseAddress, NSC_WJQ);
                cookieContainer.Add(baseAddress, NSC_ESNS);

                var httpResponse = client.PostAsync(baseAddress, content).Result;
                cookies.NscEsns = cookieContainer.GetCookies(baseAddress).Cast<Cookie>().LastOrDefault(x => x.Name == "NSC_ESNS").Value.ToString();
            }

            return cookies;
        }
    }
}