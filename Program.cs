using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Kiosk {
    internal class Program {
        public static List<double> items = new List<double>();
        public static int i = 1;
        private static void Main(string[] args) {
            bool continueTransactions = true;

            while (continueTransactions) {
                Console.Clear();
                DateTime dateTime = DateTime.Now;
                Console.WriteLine(dateTime.ToString());
                int transActs = 1;
                Console.WriteLine($"Transaction: {transActs}");
                InputRequest();

                double totalSpent = items.Sum();
                totalSpent = Math.Round(totalSpent, 2);

                MakePayment(totalSpent);

                //ProcessStartInfo startInfo = new ProcessStartInfo();
                //startInfo.FileName = "ProgramToExecute.exe";
                //startInfo.Arguments = $"{transActs} {dateTime}";
                //Process.Start(startInfo);

                string newTrans = Prompt("\nFinished with Your Transactions? Would You Like to Exit? [Y]");
                newTrans = newTrans.ToLower();

                if (newTrans == "y") {
                    continueTransactions = false;
                    break;
                }
                transActs++;
                i = 1;
                items = new List<double>();
            }
        }
        public static void MakePayment(double totalSpent) {
            bool validInput = true;

            string option = Prompt($"Total Due: {totalSpent:C}\nPlease Choose A Payment Option: Cash | Credit\n");
            option = option.ToLower();

            while (validInput) {
                if (option == "cash") {
                    CurrencyKeeper.MakePayment(totalSpent);
                    validInput = false;
                } else if (option == "credit") {
                    CurrencyKeeper.ValidEntry(totalSpent);
                    validInput = false;
                } else {
                    Console.Write("Invalid Entry. Please Reenter.");
                    option = Prompt("Please Choose A Payment Option: Cash | Credit\n");
                    option = option.ToLower();
                }
            }
        }
        private static void InputRequest() {
            bool keepScanning = true;
            Console.WriteLine("\n-- Scan Items and Press Enter When Done --");
            while (keepScanning) {
                bool isValid = ItemInput();
                if (isValid == false) {
                    keepScanning = false;
                }
            }
        }
        private static bool ItemInput() {
            double price = 0;
            double totalSpent = 0;
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
                                items.Add(price);
                                i++;
                            }
                        }
                    } else {
                        isValid = true;
                        price = double.Parse(priceStr);
                        price = Math.Round(price, 2);
                        totalSpent += price;
                        totalSpent = Math.Round(totalSpent, 2);
                        items.Add(price);
                    }
                }
            }
            i++;
            return hasValue;
        }

        #region PROMPT FUNCTIONS

        private static void ColorText(string message, ConsoleColor color, bool isWriteLine = true) {
            Console.ForegroundColor = color;
            if (isWriteLine) {
                Console.WriteLine(message);
            } else {
                Console.Write(message);
            }
            Console.ResetColor();
        }

        public static string Prompt(string prompt) {
            Console.Write(prompt);
            string answer = Console.ReadLine();
            return answer;
        }

        public static int PromptInt(string prompt) {
            int screenWidth = (Console.WindowWidth / 2);
            int value = 0;
            string input = Prompt(prompt);

            while (!int.TryParse(input, out value)) {
                ColorText("Error: Incorrect value type. Please enter a number:", ConsoleColor.DarkRed, false);
                input = Prompt(" ");
            }

            return value;
        }
        public static double PromptDubs(string prompt) {
            int screenWidth = (Console.WindowWidth / 2);
            double value = 0;
            string input = Prompt(prompt);

            while (!double.TryParse(input, out value)) {
                ColorText("Error: Incorrect value type. Please enter a number:", ConsoleColor.DarkRed, false);
                input = Prompt(" ");
            }

            return value;
        }

        #endregion PROMPT FUNCTIONS
    }
}