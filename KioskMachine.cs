using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kiosk {

    public class KioskMachine {
        List<double> items = new List<double>();
        private double _startingMoney = 0;
        private int i = 1;
        double totalSpent = 0;

        public KioskMachine(double startingMoney) {
            _startingMoney = startingMoney;
        }

        public void InputRequest() {
            bool keepScanning = true;
            Console.WriteLine("-- Scan Items and Press Enter When Done --");
            while (keepScanning) {
                bool isValid = ItemInput();
                if (isValid == false) {
                    keepScanning = false;
                }
            }
            for (int i = 0; i < items.Count; i++) {
                Console.WriteLine($"{items[i]:C}");
            }
            MakePayment();
        }

        private bool ItemInput() {
            double price = 0;
            bool hasValue = true;
            bool isValid = false;

            while (isValid == false) {
                Console.Write($"Item {i} Price: ");
                string priceStr = Console.ReadLine();
                if (priceStr == "") {
                    hasValue = false;
                    return hasValue;
                } else {
                    int count = Regex.Count(priceStr, "[0-9.-.]");
                    if (count != priceStr.Length) {
                        while (count != priceStr.Length) {
                            Console.Write($"Error: Invalid input. Please enter a valid price amount for item {i}: ");
                            priceStr = Console.ReadLine();
                            if (priceStr == "") {
                                hasValue = false;
                                return hasValue;
                            }
                            count = Regex.Count(priceStr, "[0-9.-.]");
                            if (count == priceStr.Length) {
                                price = double.Parse(priceStr);
                                price = Math.Round(price, 2);
                                totalSpent += price;
                                totalSpent = Math.Round(totalSpent, 2);
                                ItemCollection(price);
                                i++;
                            }
                        }
                    } else {
                        isValid = true;
                        price = double.Parse(priceStr);
                        price = Math.Round(price, 2);
                        totalSpent += price;
                        totalSpent = Math.Round(totalSpent, 2);
                        ItemCollection(price);
                    }
                }
            }
            i++;
            return hasValue;
        }

        private void ItemCollection(double price) {
            items.Add(price);
        }


        public void MakePayment() {
            double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 };
            double paid = 0;
            double paidAmnt = 0;
            bool paymentSuccess = false;

            while (paid < totalSpent) {
                paidAmnt = Program.PromptDubs($"Current total: {totalSpent}\nEnter how much you want to pay: ");
                paymentSuccess = CurrencyCheck(paidAmnt);
                while (paymentSuccess == false) {
                    paidAmnt = Program.PromptDubs($"Error: Invalid Payment Amount\nCurrent total: {totalSpent}\nEnter how much you want to pay: ");
                    paymentSuccess = CurrencyCheck(paidAmnt);
                }
                paid += paidAmnt;
                totalSpent -= paidAmnt;
            }
        }
        public bool CurrencyCheck(double price) {
            double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 };
            bool paymentSuccess = false;

            for (int i = 0; i < denoms.Length; i++) {
                if (price == denoms[i]) {
                    paymentSuccess = true;
                }
            }

            return paymentSuccess;
        }
    }
}