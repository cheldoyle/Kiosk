using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Kiosk {
    internal class Program {
        public static List<double> items = new List<double>(); // LIST TO COLLECT ITEM PRICE INPUTS
        public static int i = 1; // TO CHECK WHAT ITEM YOU ARE IN - SET OUTSIDE FUNCTION TO BE ABLE TO RESET ON NEW TRANSACTIONS
        private static void Main(string[] args) {
            /*

            DateTime timeOfDay = DateTime.Now;

            string month = timeOfDay.Month.ToString();
            string day = timeOfDay.Day.ToString();
            string year = timeOfDay.Year.ToString();

            StreamWriter writer = new StreamWriter($"C:\\Users\\Chels\\Desktop\\{month}-{day}-{year}-Transactions.txt", true);

            for (int i = 0; i < args.Length; i++) {
                writer.WriteLine(args[i]);
            }

            writer.WriteLine();

            writer.Close();

            */

            int transActs = 1; // COUNTS WHICH TRANSACTION KIOSK IS AT IN CASE OF MULTIPLE TRANSACTIONS
            bool continueTransactions = true; // BOOL TO KEEP TRANSACTIONS GOING SO LONG AS USER NEEDS
            string[] receiptInfo = new string[4]; // COLLECTS INFORMATION FOR RECEIPT

            while (continueTransactions) {
                Console.Clear(); // CLEARS CONSOLE UPON NEW TRANSACTIONS
                DateTime dateTime = DateTime.Now; // GRABS CURRENT DATE FOR DISPLAY AND RECEIPT
                Console.WriteLine(dateTime.ToString()); // DISPLAYS CURRENT DATE AND TIME
                Console.WriteLine($"Transaction: {transActs}"); // DISPLAYS TRANSACTION NUMBER
                InputRequest(); // BEGINS USER REQUEST FOR ITEM INPUT - ALL INFO STORED IN ITEMS LIST

                double totalSpent = items.Sum(); // TAKES SUM OF EVERY INPUT IN LIST
                totalSpent = Math.Round(totalSpent, 2); // SETS TOTAL TO XX.XX FORMAT FOR MATH

                if (totalSpent > 0) {
                    receiptInfo = MakePayment(totalSpent); // GOES INTO MAKEPAYMENT FUNCTION TO BEGIN CASH OR CREDIT - STORES RESULT IN RECEIPT LIST
                }

                string transactionNum = Convert.ToString(transActs); // TRANSACTION NUM FOR RECEIPT
                string dateDay = Convert.ToString(dateTime); // DATE INFO FOR RECEIPT

                string cardDisplay = "";
                string changeDisplay = "";

                if (receiptInfo[0] != null) { // CHECKS IF CARD WAS USED
                    cardDisplay = "Card:";
                }
                if (receiptInfo[2] != null) { // CHECKS IF CHANGE GIVEN
                    changeDisplay = "Change:";
                    double moneyfyChange = Convert.ToDouble(receiptInfo[2]); // TURNS STRING TO DOUBLE
                    receiptInfo[2] = Convert.ToString($"{moneyfyChange:C}"); // TURNS BACK TO STRING WITH $XX.XX FORMAT
                }

                double moneyfyPaid = Convert.ToDouble(receiptInfo[1]); // STRING TO DOUBLE
                receiptInfo[1] = Convert.ToString($"{moneyfyPaid:C}"); // BACK TO STRING WITH $XX.XX FORMAT

                Random rnd = new Random();
                int transID = rnd.Next(10000, 100000); // CREATES RANDOM NUM FOR ID (AESTHETIC ONLY)

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Users\starl\OneDrive\Documents\GitHub\ReceiptBuilder\bin\Debug\net8.0\ReceiptBuilder.exe";
                startInfo.Arguments = $"Transaction:{transactionNum} ID:{transID} Date:{dateDay} Total:{totalSpent:C} {cardDisplay}{receiptInfo[0]} Paid:{receiptInfo[1]} {changeDisplay}{receiptInfo[2]}";
                Process.Start(startInfo);

                // OPTION TO DO A FOLLOWUP TRANSACTION
                Console.Write("\nFinished with Your Transactions? Would You Like to Exit? [Y]");
                string newTrans = Console.ReadLine();
                newTrans = newTrans.ToLower();
                // WILL ONLY EXIT IF Y IS ENTERED INTO THE CONSOLE
                if (newTrans == "y") {
                    continueTransactions = false;
                    break;
                }
                transActs++; // UPS TRANSACTS NUM IF TRANSACTIONS ARE CONTINUING
                i = 1; // RESETS I TO 1 SO THAT ITEM NUMBERS DO NOT CONTINUE FROM LAST TRANSACTION
                items = new List<double>(); // RESETS LIST TO CLEAR PREVIOUS ITEMS
            }
        }
        public static string[] MakePayment(double totalSpent) {
            bool validInput = true; // BOOL TO ENSURE VALID INPUT FOR CASH OR CREDIT
            string[] receiptInfo = new string[4]; // STRING TO HOLD RECEIPT INFO

            string option = Prompt($"Total Due: {totalSpent:C}\nPlease Choose A Payment Option: Cash | Credit\n");
            option = option.ToLower();

            while (validInput) {
                if (option == "cash") {
                    receiptInfo = CurrencyKeeper.MakePayment(totalSpent); // GOES INTO CASH SECTION OF CURRENCY CLASS
                    validInput = false;
                } else if (option == "credit") {
                    receiptInfo = CurrencyKeeper.ValidEntry(totalSpent); // GOES INTO CREDIT SECTION OF CURRENCY CLASS
                    validInput = false;
                } else {
                    Console.WriteLine("Invalid Entry. Please Reenter.");
                    option = Prompt("Please Choose A Payment Option: Cash | Credit\n");
                    option = option.ToLower();
                }
            }
            return receiptInfo;
        }
        private static void InputRequest() {
            bool keepScanning = true; // KEEPS SCANNING FOR ITEMS AS LONG AS TRUE
            Console.WriteLine("\n-- Scan Items and Press Enter When Done --");
            while (keepScanning) {
                bool isValid = ItemInput(); // CHECKS IF INPUT IS A PRICE OR EMPTY
                if (isValid == false) {
                    keepScanning = false; // HOPS OUT OF SCANNING IF BOOL IS FALSE (EMPTY INPUT)
                }
            }
        }
        private static bool PriceWriter(string priceStr) {
            double price = 0;
            bool validation = true;

            validation = double.TryParse(priceStr, out price); // CHECKS IF INPUT CAN BE PARSED

            if (validation) { // IF VALID DOUBLE
                if (price < 0) { // DOESN'T ALLOW NEGATIVE
                    Console.WriteLine("\nError: Cannot Accept Amounts Under $.01\n");
                    validation = false;
                } else {
                    items.Add(price); // ADDS VALID ENTRIES INTO LIST
                }
            }

            return validation;
        }
        private static bool ItemInput() {
            double price = 0; // TO CONVERT STRING TO DOUBLE IF INPUT IS NOT EMPTY
            bool hasValue = true; // CHECKS IF INPUT CONTAINS VALUE OR IS EMPTY
            bool isValid = false; // CHECKS FOR VALID INPUT TO FILTER OUT UNDESIRED INPUTS

            while (!isValid) {
                Console.Write($"Item {i} Price: "); // USES I TO DISPLAY ITEM NUM
                string priceStr = Console.ReadLine(); // TAKES INITIAL INPUT AS STRING
                if (priceStr == "") { // CHECKS IF INPUT IS EMPTY
                    hasValue = false; // SETS BOOL TO FALSE
                    return hasValue; // RETURNS FALSE TO INPUTREQUEST
                }  else if (priceStr == "kiosk") {
                    CurrencyKeeper.DisplayKioskInfo();
                } else {
                    isValid = PriceWriter(priceStr); // CHECKS IF INPUT IS IN PROPER DOUBLE FORMAT
                    if (isValid == false) { // IF IT ISN'T
                        while (isValid == false) {
                            ColorText("\nError: Incorrect Value Type. Please Enter A Number.\n", ConsoleColor.DarkRed, false);
                            Console.Write($"Item {i} Price:");
                            priceStr = Prompt(" ");
                            isValid = PriceWriter(priceStr); // CONTINUES TO CHECK AND UPDATE UNTIL MEETS CORRECT FORMAT
                        }
                    }
                }
            }
            i++; // INCREASES I VALUE AS ITEM NUMBER GOES UP
            return hasValue; // RETURNS TRUE OR FALSE TO DETERMINE WHETHER TO KEEP SCANNING
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
                ColorText("\nError: Incorrect Value Type. Please Enter A Valid Number.", ConsoleColor.DarkRed, false);
                input = Prompt(" ");
            }

            return value;
        }

        #endregion PROMPT FUNCTIONS
    }
}