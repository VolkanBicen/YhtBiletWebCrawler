using System;
using System.Collections.Generic;
using System.Text;

namespace TcddBiletBot.Helper
{
    public class EmptySeatsCount
    {
        public int GetEmptySeatsCount(string seatsValue)
        {
            string[] seatsValueSplit = seatsValue.Split(" ");
        
            int count = Convert.ToInt32(seatsValueSplit[seatsValueSplit.Length - 1].Trim( new char[] { '(', ')' }));
            
            return count;
        }
    }
}
