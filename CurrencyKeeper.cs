using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk {
    internal class CurrencyKeeper {

        // CASH FUNCTIONS
        public static void MakePayment(double totalSpent) {
            double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 };
            double paid = 0;
            double paidAmnt = 0;
            double inputTotal = totalSpent;
            double leftOver = 0;
            bool paymentSuccess = false;
            int i = 1;

            Console.Write("--- Valid Currency Amounts (Coins and Dollars) Only ---");
            while (totalSpent > 0) {
                paidAmnt = Program.PromptDubs($"\nCurrent total: {totalSpent:C}\nPayment {i}: ");
                paymentSuccess = CurrencyCheck(paidAmnt);
                while (paymentSuccess == false) {
                    paidAmnt = Program.PromptDubs($"\nError: Invalid Payment Amount\nCurrent total: {totalSpent:C}\nEnter how much you want to pay for payment {i}: ");
                    paymentSuccess = CurrencyCheck(paidAmnt);
                }
                i++;
                paid += paidAmnt;
                totalSpent -= paidAmnt;
                totalSpent = Math.Round(totalSpent, 2);
            }
            if (totalSpent < 0) {
                leftOver = paid - inputTotal;
                leftOver = Math.Round(leftOver, 2);
                Console.WriteLine($"\nTotal Paid: {paid:C}\nChange Due: {leftOver:C}");
                MakeChange(leftOver);
            }
        }
        public static bool CurrencyCheck(double price) {
            double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 };
            bool paymentSuccess = false;

            for (int i = 0; i < denoms.Length; i++) {
                if (price == denoms[i]) {
                    paymentSuccess = true;
                }
            }

            return paymentSuccess;
        }

        public static void MakeChange(double totalPrice) {
            double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 };
            int[] denomsAmnts = { 50, 50, 50, 50, 5, 20, 5, 20, 20, 20, 10, 10 };
            int denomLen = denoms.Length;
            List<double> totalChange = new List<double>();
            bool kioskEmpty = false;

            for (int i = denomLen - 1; i >= 0; i--) {
                while (totalPrice >= denoms[i]) {
                    if (denomsAmnts[i] == 0) {
                        denoms[i] = 0;
                    } else {
                        denomsAmnts[i]--;
                        totalPrice -= denoms[i];
                        totalChange.Add(denoms[i]);
                    }
                    for (int j = 0; j < denomLen - 1; j++) {
                        if (denoms[i] != 0) {
                            kioskEmpty = false;
                            break;
                        } else {
                            kioskEmpty = true;
                        }
                    }
                }
                if (kioskEmpty) {
                    string noMoney = Program.Prompt("Kiosk is Currently Out of Money. Return to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit");
                    noMoney = noMoney.ToLower();
                    while (noMoney != "return" ||  noMoney != "exit") {
                        if (noMoney == "return") {
                            Program.MakePayment(totalPrice);
                        } else if (noMoney == "exit") {

                        } else {
                            Console.WriteLine("Error: Invalid Input. Please Try Again.");
                            noMoney = Program.Prompt("Kiosk is Currently Out of Money. Return to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit");
                        }
                    }
                }
            }
            Console.WriteLine("\n--- Thank You For Your Patronage! Please Take Your Change ---");
            for (int i = 0; i < totalChange.Count; i++) {
                Console.WriteLine($"{totalChange[i]:C} dispensed");
            }
        }

        // CREDIT CARD FUNCTIONS

        public static void ValidEntry(double amountSpent) {
            bool validIn = true;
            string[] cardValid = new string[2];
            string cardType = "";
            double paidAmnt = 0;
            double cashAmnt = 0;
            bool cashBackBool = true;
            bool gotCash = false;

            string cashBack = Program.Prompt("Cash Back? Y | N\n");
            cashBack = cashBack.ToLower();

            while (cashBackBool) {
                if (cashBack == "y") {
                    cashAmnt = Program.PromptDubs("Cash Back Amount: [Max $100.00]");
                    if (cashAmnt <= 100) {
                        cashBackBool = false;
                        gotCash = true;
                    } else {
                        Console.WriteLine($"Error: Max Cash Back Amount is $100.00");
                    }
                } else if (cashBack != "y" && cashBack != "n") {
                    Console.WriteLine("Error: Invalid Response. Please Try Again.");
                    cashBack = Program.Prompt("Cash Back? Y | N\n");
                } else if (cashBack == "n") {
                    cashBackBool = false;
                }
            }


            while (validIn) {
                string cardNo = Program.Prompt("Enter Your Credit Card Number: ");
                if (checkLuhn(cardNo) == true) {
                    cardValid = MoneyRequest(cardNo, amountSpent);
                    cardType = CardType(cardNo);
                    validIn = false;
                } else {
                    Console.WriteLine("Invalid Input. Please Try Again.");
                }
            }
            Console.WriteLine($"Card Number: {cardType} {cardValid[0]}");
            if (cardValid[1] != "declined") {
                paidAmnt = double.Parse(cardValid[1]);
                paidAmnt = Math.Round(paidAmnt, 2);
                Console.WriteLine($"Amount Paid: {paidAmnt:C}");
                if (gotCash) {
                    Console.WriteLine($"Cash Back: {cashAmnt:C}");
                    MakeChange(cashAmnt);
                }
            } else {
                Console.WriteLine($"Payment: {cardValid[1]}");
            }

            if (cardValid[1] == "declined" && cashAmnt == 0) {
                string noMoney = Program.Prompt($"Card Has Been Declined.\nBalance Owed: {amountSpent:C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                noMoney = noMoney.ToLower();
                while (noMoney != "return" || noMoney != "exit") {
                    if (noMoney == "return") {
                        Program.MakePayment(amountSpent);
                        break;
                    } else if (noMoney == "exit") {
                        break;
                    } else {
                        Console.WriteLine("Error: Invalid Input. Please Try Again.");
                        noMoney = Program.Prompt($"Card Has Been Declined.\nBalance Owed: {amountSpent:C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                    }
                }
            }
        }
        static string CardType(string cardType) {
            string type = "";

            if (cardType[0] == '3' && (cardType[1] == 4 || cardType[1] == 7)) {
                type = "American Express";
            } else if (cardType[0] == '6') {
                type = "Discover";
            } else if (cardType[0] == '5') {
                type = "MasterCard";
            } else if (cardType[0] == '4') {
                type = "Visa";
            } else {
                type = "Unknown";
            }

            return type;
        }
        static bool checkLuhn(String cardNo) {
            int nDigits = cardNo.Length;

            int nSum = 0;
            bool isSecond = false;
            for (int i = nDigits - 1; i >= 0; i--) {

                int d = cardNo[i] - '0';

                if (isSecond == true)
                    d = d * 2;
                nSum += d / 10;
                nSum += d % 10;

                isSecond = !isSecond;
            }
            return (nSum % 10 == 0);
        }
        static string[] MoneyRequest(string account_number, double amount) {
            Random rnd = new Random();
            bool pass = rnd.Next(100) < 50;
            bool declined = rnd.Next(100) < 50;

            if (pass) {
                return new string[] { account_number, amount.ToString() };
            } else {
                if (!declined) {
                    return new string[] { account_number, (amount / rnd.Next(2, 6)).ToString() };
                } else {
                    return new string[] { account_number, "declined" };
                }
            }
        }
    }
}
