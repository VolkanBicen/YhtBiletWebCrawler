using System;
using System.Collections.Generic;
using System.Text;

namespace TcddBiletBot.Helper
{
    public class VoyageControl
    {
        public bool Check(string fromStation, string toStation)
        {
            List<string> stationList = new List<string>() { "Ankara Gar","Eskişehir", "Konya", "İstanbul(Bakırköy)", "İstanbul(Bostancı)", "İstanbul(Halkalı)", "İstanbul(Pendik)", "İstanbul(Söğütlü Ç.)" };
            int state = 0;

            foreach (var item in stationList)
            {
                if (string.Equals(item, fromStation))
                {
                    state++;
                }
                if (string.Equals(item, toStation))
                {
                    state++;
                }
            }
            if (state == 2)
            {
                return true;
            }
            return false;
        }
    }
}
