using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurbExcel
{
    public class ConverterAlphabetNumbers
    {
        public static string ToAlphabetNumber(int numb)
        {
            if (numb < 0)
            {
                return String.Empty;
            }
                

            int tnumb = numb;
            List<char> alfNumb = new List<char>();
            int iter = 0;
            while (true)
            {
                    int remDiv = tnumb % 26;
                    alfNumb.Add((char)(65 + remDiv - iter));
                    tnumb /= 26;
                if (tnumb == 0) break;
                    iter += 1;
            }
            alfNumb.Reverse();
            return new string(alfNumb.ToArray());
        }
        public static int ToDecimalNumber()
        {
            return 0;
        }
    }
}
