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

            StreamWriter writer = new StreamWriter($"C:\\Users\\starl\\OneDrive\\Desktop\\{month}-{day}-{year}-Transactions.txt");

            for (int i = 0; i < args.Length; i++) {
                writer.WriteLine(args[i]);
            }

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

                receiptInfo = MakePayment(totalSpent); // GOES INTO MAKEPAYMENT FUNCTION TO BEGIN CASH OR CREDIT - STORES RESULT IN RECEIPT LIST

                string transactionNum = Convert.ToString(transActs); // TRANSACTION NUM FOR RECEIPT
                string dateDay = Convert.ToString(dateTime); // DATE INFO FOR RECEIPT
                string receiptOne = Convert.ToString(receiptInfo[0]); // CONVERTS FIRST ITEM FOR RECEIPT (PAID AMOUNT FOR CASH, CARD TYPE FOR CARD)
                string receiptTwo = Convert.ToString(receiptInfo[1]); // CONVERTS SECOND ITEM FOR RECEIPT (CHANGE FOR CASH, PAID AMOUNT FOR CARD)
                string receiptThree = Convert.ToString(receiptInfo[2]); // CONVERTS THIRD ITEM FOR CARD TRANSACTIONS - CHANGE (IF CASH BACK)

                if (receiptInfo[3] == "cash") { // CHECKS IF LAST ELEMENT OF RECEIPT IS CASH
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"C:\Users\starl\OneDrive\Documents\GitHub\ReceiptBuilder\bin\Debug\net8.0\ReceiptBuilder.exe";
                    startInfo.Arguments = $"{transactionNum} {dateDay} Total:{totalSpent:C} Paid:{receiptInfo[0]} Change:{receiptInfo[1]}";
                    Process.Start(startInfo);
                } else if (receiptInfo[3] == "credit") { // CHECKS IF LAST ELEMENT IS CREDIT
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"C:\Users\starl\OneDrive\Documents\GitHub\ReceiptBuilder\bin\Debug\net8.0\ReceiptBuilder.exe";
                    startInfo.Arguments = $"{transactionNum} {dateDay} Total:{totalSpent:C} Card:{receiptInfo[0]} Paid:{receiptInfo[1]} Change:{receiptInfo[2]}";
                    Process.Start(startInfo);
                }
                // OPTION TO DO A FOLLOWUP TRANSACTION
                string newTrans = Prompt("\nFinished with Your Transactions? Would You Like to Exit? [Y]");
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
            string payType = ""; // PAYTYPE FOR DETERMINING WHAT TYPE OF RECEIPT TO DISPLAY

            string option = Prompt($"Total Due: {totalSpent:C}\nPlease Choose A Payment Option: Cash | Credit\n");
            option = option.ToLower();

            while (validInput) {
                if (option == "cash") {
                    receiptInfo = CurrencyKeeper.MakePayment(totalSpent); // GOES INTO CASH SECTION OF CURRENCY CLASS
                    payType = "cash";
                    validInput = false;
                } else if (option == "credit") {
                    receiptInfo = CurrencyKeeper.ValidEntry(totalSpent); // GOES INTO CREDIT SECTION OF CURRENCY CLASS
                    payType = "credit";
                    validInput = false;
                } else {
                    Console.WriteLine("Invalid Entry. Please Reenter.");
                    option = Prompt("Please Choose A Payment Option: Cash | Credit\n");
                    option = option.ToLower();
                }
            }
            receiptInfo[3] = payType;
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
        private static bool ItemInput() {
            double price = 0; // TO CONVERT STRING TO DOUBLE IF INPUT IS NOT EMPTY
            bool hasValue = true; // CHECKS IF INPUT CONTAINS VALUE OR IS EMPTY
            bool isValid = false; // CHECKS FOR VALID INPUT TO FILTER OUT UNDESIRED INPUTS

            while (isValid == false) {
                Console.Write($"Item {i} Price: "); // USES I TO DISPLAY ITEM NUM
                string priceStr = Console.ReadLine(); // TAKES INITIAL INPUT AS STRING
                if (priceStr == "") { // CHECKS IF INPUT IS EMPTY
                    hasValue = false; // SETS BOOL TO FALSE
                    return hasValue; // RETURNS FALSE TO INPUTREQUEST
                } else {
                    int count = Regex.Count(priceStr, "[0-9.-.]"); // CREATES AND SETS COUNT TO NUMBER OF NUMS AND DECIMALS IN INPUT
                    int decCount = Regex.Count(priceStr, "[.-.]"); // CHECKS HOW MANY DECIMALS IN INPUT TO AVOID TOO MANY
                    if (count != priceStr.Length && decCount <= 1) { // CHECKS IF COUNT == LENGTH OF PRICESTR TO FILTER OUT LETTERS OR OTHER SYMBOLS AND AVOID TOO MANY DECIMALS
                        while (count != priceStr.Length && decCount <= 1) { // KEEPS IN LOOP TO MAKE SURE ONLY NUMBERS AND DECIMALS APPEAR
                            Console.Write($"Error: Invalid input. Please enter a valid price amount for item {i}: ");
                            priceStr = Console.ReadLine();
                            if (priceStr == "") { // CHECKS AGAIN FOR EMPTY INPUT IF INTENTION WAS TO EXIT
                                hasValue = false;
                                return hasValue;
                            }
                            count = Regex.Count(priceStr, "[0-9.-.]"); // CHECKS AGAIN FOR COUNTS
                            decCount = Regex.Count(priceStr, "[.-.]");
                            if (count == priceStr.Length && decCount <= 1) {
                                price = double.Parse(priceStr); // SETS TO DOUBLE FROM STRING
                                price = Math.Round(price, 2); // ROUNDS TO XX.XX FORMAT TO AVOID MATH ERRORS
                                items.Add(price); // ADDS PRICE TO ITEMS LIST
                                i++;
                            }
                        }
                    } else { // GOES HERE IF INITIAL INPUT CORRECT
                        isValid = true; // SETS TO TRUE TO RETURN TO INPUT REQUEST
                        price = double.Parse(priceStr); // SETS PRICE TO DOUBLE FROM STRING
                        price = Math.Round(price, 2); // ROUNDS TO AVOID MATH ERRORS
                        items.Add(price); // ADDS TO ITEMS LIST
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
                ColorText("Error: Incorrect value type. Please enter a number:", ConsoleColor.DarkRed, false);
                input = Prompt(" ");
            }

            return value;
        }

        #endregion PROMPT FUNCTIONS
    }
}