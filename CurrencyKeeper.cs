using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk {
    internal class CurrencyKeeper {
        static double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 };
        int denomLen = denoms.Length;

        public void MakeChange(double totalPrice) {
            List<double> totalChange = new List<double>();

            for (int i = denomLen - 1; i >= 0; i--) {
                while (totalPrice >= denoms[i]) {
                    totalPrice -= denoms[i];
                    totalChange.Add(denoms[i]);
                }
            }

            for (int i = 0; i < totalChange.Count; i++) {
                Console.WriteLine(totalChange[i]);
            }
        }
    }
}
