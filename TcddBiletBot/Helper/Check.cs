using System;
using System.Collections.Generic;
using System.Text;

namespace TcddBiletBot.Helper
{
    public class CheckUser
    {
        public bool Check(long Id)
        {
            string[] accessList =  System.IO.File.ReadAllLines("Work/accessList.txt");
            foreach (var item in accessList)
            {
                if (Id == Convert.ToInt64(item))
                {
                    return true;
                }
            }
        
            return false;
           
        }
    }
}
