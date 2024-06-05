using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk {
    internal class CurrencyKeeper {

        static double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 }; // EACH DENOM
        static double amountInKiosk = 300;
        static bool kioskEmpty = false;

        public static void DisplayKioskInfo() {
            Console.WriteLine($"Amount in Kiosk: {amountInKiosk:C}");
        }

        // CASH FUNCTIONS
        public static string[] MakePayment(double totalSpent) {
            string[] receipts = new string[3]; // CREATES NEW LIST FOR RETURNING RECEIPT INFO
            double paid = 0; // TOTAL AMOUNT PAID IN CASH
            double paidAmnt = 0; // INDIVIDUAL PAYMENTS
            double inputTotal = totalSpent; // DUPLICATES INPUT FOR MATH
            double leftOver = 0; // FOR DETERMINING CHANGE NEEDED
            bool paymentSuccess = false; // CHECKS IF AMOUNT ENTERED IS VALID
            int i = 1; // COUNTER FOR PAYMENT NUMBER
            double displayAmount = totalSpent - paidAmnt;
            bool noMoneyInKiosk = false;

            Console.Write("\n--- Valid Currency Amounts (Coins and Dollars) Only ---\nAllowed Amounts: ");
            for (int c = 0; c < denoms.Length; c++) {
                Console.Write($"{denoms[c]:C} "); // DISPLAYS ALLOWED PAYMENT AMOUNT INPUTS
            }
            while (inputTotal > 0) { // CONTINUES WHILE TOTAL TO PAY IS ABOVE 0
                paidAmnt = Program.PromptDubs($"\nCurrent total: {displayAmount:C}\nPayment {i}: "); // ASKS FOR PAYMENT
                paymentSuccess = CurrencyCheck(paidAmnt); // CHECKS IF PAYMENT == ONE OF ALLOWED PAYMENT AMOUNTS
                while (paymentSuccess == false) { // CHECKS IF PAYMENT AMOUNT INVALID
                    paidAmnt = Program.PromptDubs($"\nError: Invalid Payment Amount\nCurrent total: {(totalSpent - paid):C}\nEnter how much you want to pay for payment {i}: "); // REQUESTS NEW PAYMENT INPUT
                    paymentSuccess = CurrencyCheck(paidAmnt);
                }
                i++; // PAYMENT NUMBER GOES UP
                paid += paidAmnt; // ADDS EACH PAID AMOUNT TO TOTAL PAID
                displayAmount = totalSpent - paid;
                inputTotal -= paidAmnt; // REMOVES AMOUNT PAID FROM ORIGINAL TOTAL
                inputTotal = Math.Round(inputTotal, 2); // SETS TO XX.XX FORMAT FOR MATH
            }
            if (inputTotal < 0) { // CHECKS IF PAID TOTAL IS BELOW 0
                leftOver = paid - totalSpent; // DETERMINES HOW MUCH CHANGED NEEDED
                leftOver = Math.Round(leftOver, 2); // XX.XX FORMAT FOR MATH
                receipts[2] = Convert.ToString(leftOver); // ADDS TO RECEIPT INFO
                Console.WriteLine($"Cash Paid: {paid:C}\nChange Due: {leftOver:C}"); // DISPLAYS HOW MUCH PAID AND CHANGE DUE
                noMoneyInKiosk = MakeChange(leftOver); // GOES INTO FUNCTION TO MAKE CHANGE
                if (noMoneyInKiosk) {
                    receipts[1] = Convert.ToString(paid); // CONVERTS PAID TO STRING FOR RECEIPT
                } else {
                    Console.WriteLine("\nKiosk Error Processing Your Change - Please Switch to Credit\n");
                    receipts = ValidEntry(totalSpent);
                }
            } else if (inputTotal == 0) { // IN CASE AMOUNT PAID IS EXACTLY HOW MUCH OWED
                Console.WriteLine($"Cash Paid: {paid:C}\nChange Due: $0.00\n\n--- Thank You for Your Patronage ---\n");
            }
            return receipts; // RETURN RECEIPT INFO TO PROGRAM
        }
        public static bool CurrencyCheck(double price) { // CHECKS IF VALID CURRENCY
            bool paymentSuccess = false;

            for (int i = 0; i < denoms.Length; i++) {
                if (price == denoms[i]) { // IF PRICE IS EQUAL TO ANYTHING IN DENOMS, RETURNS TRUE
                    paymentSuccess = true;
                }
            }

            return paymentSuccess;
        }

        public static bool MakeChange(double totalPrice) { // CHANGE MAKER
            int denomLen = denoms.Length; // LENGTH OF HOW MANY DENOMINATIONS THERE ARE
            List<double> totalChange = new List<double>(); // LIST FOR EACH CHANGE AMOUNT
            double amountToPay = totalPrice;
            bool kioskMadeChange = false;

            for (int i = denomLen - 1; i >= 0; i--) { // GOES THROUGH DENOMS BY LENGTH
                while (amountToPay >= denoms[i]) { // CONTINUES SORTING THROUGH CHANGE WHILE TOTAL IS ABOVE DENOM AMOUNT
                    amountInKiosk -= denoms[i];
                    if (amountInKiosk <= 0) {
                        kioskEmpty = true;
                        break;
                    }
                    amountToPay -= denoms[i];
                    totalChange.Add(denoms[i]);
                }
            }
            if (kioskEmpty) { // CHECKS IF NOTHING IN KIOSK
                Console.WriteLine("\nKiosk is Currently Out of Money. Return to Payment Screen to Choose a Different Method\n"); // ALLOWS USER TO RETURN TO CHOOSE CREDIT
            }
            if (kioskEmpty == false) {
                Console.WriteLine("\n--- Thank You For Your Patronage! Please Take Your Change ---");
                for (int i = 0; i < totalChange.Count; i++) { // DISPLAYS EACH CHANGE DISPENSED
                    Console.WriteLine($"{totalChange[i]:C} dispensed");
                }
                kioskMadeChange = true;
            } else {
                kioskMadeChange = false;
            }
            return kioskMadeChange;
        }


        // CREDIT CARD FUNCTIONS

        public static string[] ValidEntry(double amountSpent) {
            string[] receipts = new string[3]; // RECEIPT INFO TO GO TO PROGRAM
            string cardType = ""; // CARD TYPE (VISA, MC, DISCOVER, AE)
            string cardDisplay = "";
            string[] cardValid = new string[2]; // STRING ARRAY FOR RETURN PASS OR DECLINED STATE
            double paidAmnt = 0; // AMOUNT PAID BY CARD - AMOUNTSPENT
            double cashAmnt = 0; // CASH REQUESTED
            bool cashBackBool = true; // CHECKS IF USER WANTS CASH BACK
            bool gotCash = false; // GOES TO TRUE IF USER WANTS CASH
            bool partial = false; // BOOL FOR CHECKING PARTIAL PAYMENTS
            double partialPay = 0;
            double totalCreditAndCash = 0;

            if (!kioskEmpty) { // CHECKS IF THERE'S MONEY IN KIOSK BEFORE ASKING FOR CASH BACK
                string cashBack = Program.Prompt("\nCash Back? Y | N\n"); // ASKS FOR CASH BACK FIRST
                cashBack = cashBack.ToLower();

                while (cashBackBool) {
                    if (cashBack == "y") { // IF Y
                        cashAmnt = Program.PromptDubs("\n[Max $100.00] Cash Back Amount: "); // SETS MAX TO AVOID TOO HIGH NUMS
                        if (cashAmnt <= 100 && cashAmnt > 0) { // CHECKS FOR LESS THAN OR EQUAL TO MAX
                            cashAmnt = CashBack(cashAmnt);
                            cashBackBool = false;
                            gotCash = true;
                        } else {
                            Console.WriteLine($"\nError: Max Cash Back Amount is $100.00\n");
                        }
                    } else if (cashBack != "y" && cashBack != "n") {
                        Console.WriteLine("\nError: Invalid Response. Please Try Again.");
                        cashBack = Program.Prompt("Cash Back? Y | N\n");
                    } else if (cashBack == "n") { // IMMEDIATELY EXITS IF NO
                        cashBackBool = false;
                    }
                }
            }

            cardValid = NotDeclined(amountSpent); // GRABS CC RUN RESULTS
            cardType = CardType(cardValid[0]); // GRABS CARDTYPE
            cardDisplay = DisplayCardNum(cardValid[0]); // SETS DISPLAY TO LAST 4 DIGITS
            receipts[0] = $"{cardType}-{cardDisplay}"; // SETS FIRST ITEM IN LIST TO CARD TYPE

            bool paidInFull = false;
            while (paidInFull == false) {
                partialPay = double.Parse(cardValid[1]); // GRABS AMOUNT PAID BY CARD
                if (partialPay < amountSpent) { // CHECKS IF AMOUNT IS LESS THAN TOTAL OWED
                    partial = true;
                }
                if (partial) { // IF PAID IS LESS THAN TOTAL
                    while (partial) { // KEEPS IN LOOP WHILE AMOUNT IS BELOW OWED
                        Console.Write($"\nPartial Paid: {partialPay:C}. \nPlease Choose Another Payment Option for the Remaining Balance [{(amountSpent - partialPay):C}]: ");
                        if (gotCash && cashAmnt != 0) { // CHECKS IF CASH BACK - CAN ONLY USED CARDS IF GOT CASH BACK
                            cardValid = NotDeclined((amountSpent - partialPay)); // SENDS AMOUNT LEFT INTO DECLINE, PAY FUNCTION
                            double amntCharged = double.Parse(cardValid[1]); // GRABS AMOUNT CHARGED THIS ROUND
                            Console.WriteLine($"Amount Charged: {amntCharged:C}"); // DISPLAYS AMOUNT CHARGED
                            partialPay = partialPay + amntCharged; // ADDS AMOUNT CHARGED TO PARTIAL PAY
                            partialPay = Math.Round(partialPay, 2); // MATH ROUND FOR MATH
                            Console.WriteLine($"\nCard: {cardType} {cardDisplay} \nTotal Paid: {partialPay:C}\n"); // DISPLAYS CARD TYPE AND PAID AMOUNT
                            partial = partialPay < amountSpent; // SETS PARTIAL BOOL TO WHETHER PARTIAL PAY STILL LOWER
                            if (partial == false) { // IF NOT LOWER
                                Console.WriteLine($"Cash Back: {cashAmnt:C}");
                                paidInFull = true; // SETS TO PAID IN FULL
                            }
                        } else { // GOES HERE IF NO CASH BACK
                            Console.Write("\nWould You Like to Use Cash or Credit? ");
                            string paymentOption = Console.ReadLine(); // REQUESTS PAYMENT OPTION BETWEEN CARD AND CASH
                            paymentOption = paymentOption.ToLower();
                            bool paymentChosen = false;
                            while (paymentChosen == false) {
                                if (paymentOption == "credit") { // CHECKS FOR CREDIT
                                    cardValid = NotDeclined((amountSpent - partialPay)); // GOES THROUGH DECLINE PAYMENT FUNCTION
                                    double amntCharged = double.Parse(cardValid[1]); // GRABS AMOUNT PAID FROM THIS ROUND
                                    Console.WriteLine($"Amount Charged: {amntCharged:C}"); // DISPLAYS AMOUNT CHARGED
                                    partialPay = partialPay + amntCharged; // UPDATES PARTIAL PAY
                                    partialPay = Math.Round(partialPay, 2);
                                    Console.WriteLine($"\nCard: {cardType} {cardDisplay} \nTotal Paid: {partialPay:C}\n"); // DISPLAYS CARD TYPE AND PAID AMOUNT
                                    partial = partialPay < amountSpent; // CHECKS BOOL FOR UPDATES
                                    if (partial == false) {
                                        paidInFull = true; // UPDATES TO PAID IN FULL IF PARTIAL IS FALSE
                                        receipts[1] = Convert.ToString(partialPay); // UPDATES RECEIPT INFO TO HOW MUCH PAID
                                    }
                                    paymentChosen = true;
                                } else if (paymentOption == "cash") {
                                    receipts = MakePayment((amountSpent - partialPay)); // SENDS LEFTOVER TO PAY TO CASH FUNCTION
                                    totalCreditAndCash = partialPay + double.Parse(receipts[1]); // GRABS TOTAL PAID
                                    receipts[1] = Convert.ToString(totalCreditAndCash); // UPDATES RECEIPT TO CREDIT + CASH PAID TOTAL
                                    receipts[0] = $"{cardType}-{cardDisplay}"; // ADDS CARD INFO
                                    Console.WriteLine($"\nTotal Paid: {totalCreditAndCash:C}"); // DISPLAYS TOTAL CREDIT + CASH PAID
                                    paymentChosen = true;
                                    partial = false;
                                    paidInFull = true;
                                } else {
                                    Console.Write("Error: Please Choose Cash or Credit: ");
                                    paymentOption = Console.ReadLine();
                                    paymentOption = paymentOption.ToLower();
                                }
                            }
                        }
                    }
                } else {
                    paidAmnt = double.Parse(cardValid[1]); // DOUBLE PARSE TOTAL PAID
                    paidAmnt = Math.Round(paidAmnt, 2); // FOR MATH XX.XX FORMAT
                    receipts[1] = Convert.ToString(paidAmnt); // SETS SECOND ITEM TO PAID AMOUNT FOR CREDIT
                    Console.WriteLine($"Amount Paid: {paidAmnt:C}");
                    paidInFull = true;
                    if (gotCash && cashAmnt != 0) { // CHECKS IF CASH REQUESTED
                        Console.WriteLine($"Cash Back: {cashAmnt:C}"); // DISPLAYS CASH BACK
                        receipts[2] = Convert.ToString(cashAmnt); // ADDS CASH BACK TO RECEIPT
                        MakeChange(cashAmnt); // GOES THROUGH CHANGE MAKER TO DISPLAY CASH BACK
                    }
                }
            }
            return receipts;
        }
        static string[] NotDeclined (double amount) { // CHECKS FOR DECLINED
            bool validIn = true;
            bool notEntered = true;
            bool declined = true;
            string cardNo = "";
            string cardDisplay;
            string[] cardValid = new string[2];

            while (declined) {
                validIn = true;
                while (validIn) { // KEEPS USER IN LOOP FOR VALID CC INPUT
                    cardNo = Program.Prompt("\nEnter Your Credit Card Number: ");
                    if (cardNo.Length < 15) { // CHECKS FOR APPROPRIATE CC LENGTH
                        while (notEntered) {
                            cardNo = Program.Prompt("\nEntry Error: Enter Your Credit Card Number: ");
                            if (cardNo.Length >= 15) { // EXITS WHEN 15+ CC ENTRY
                                notEntered = false;
                            }
                        }
                    }
                    if (checkLuhn(cardNo) == true) { // CHECKS FOR VALID CC INPUT
                        string cardType = CardType(cardNo);
                        cardDisplay = DisplayCardNum(cardNo);
                        Console.WriteLine($"\nCard: {cardType} {cardDisplay}");
                        cardValid = MoneyRequest(cardNo, amount); // GRABS AMOUNT AND PAYMENT STATE FROM RANDOM DECLINE FUNCTION
                        if (cardValid[1] != "declined") {
                            declined = false;
                        } else {
                            Console.WriteLine("\nCard Declined: Please Reenter\n");
                        }
                        validIn = false; // EXITS LOOP
                    } else {
                        Console.WriteLine("\nInvalid Input. Please Try Again.");
                    }
                }
            }
            return cardValid;
        }
        static double CashBack(double cashWanted) {
            double returnCash = 0; // HOW MUCH TO RETURN FOR CASH BACK
            int denomLen = denoms.Length;
            double totalWanted = cashWanted;
            // GOES THROUGH KIOSK SIMILAR TO MAKING CHANGE - CHECKS IF KIOSK CAN MAKE CHANGE

            for (int i = denomLen - 1; i >= 0; i--) { // GOES THROUGH DENOMS BY LENGTH
                while (totalWanted >= denoms[i]) { // CONTINUES SORTING THROUGH CHANGE WHILE TOTAL IS ABOVE DENOM AMOUNT
                    amountInKiosk -= denoms[i];
                    if (amountInKiosk <= 0) {
                        kioskEmpty = true;
                        break;
                    }
                    returnCash += denoms[i];
                    totalWanted -= denoms[i];
                }
            }
            if (kioskEmpty) { // CHECKS IF NOTHING IN KIOSK
                Console.WriteLine("\nKiosk is Currently Out of Money. Cannot Supply Cash Back. Please Return\n");
            }

            return returnCash;
        }

        static string DisplayCardNum(string cardNum) {
            string hiddenNums = "";

            for (int i = cardNum.Length - 4; i < cardNum.Length; i++) {
                hiddenNums += cardNum[i];
            }

            return hiddenNums;
        }
        static string CardType(string cardType) {
            string type = "";
            // GOES THROUGH CARDTYPE STRING TO DETERMINE COMPANY
            if (cardType[0] == '3' && (cardType[1] == 4 || cardType[1] == 7)) {
                type = "American Express";
            } else if (cardType[0] == '6') {
                type = "Discover";
            } else if (cardType[0] == '5') {
                type = "MasterCard";
            } else if (cardType[0] == '4') {
                type = "Visa";
            } else {
                type = "Unknown"; // AUTOS TO UNKNOWN IF TYPE NOT ONE OF ABOVE
            }

            return type;
        }
        static bool checkLuhn(String cardNo) {
            int nDigits = cardNo.Length;
            // CHECKS FOR VALID CC NUMBER
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
            // PASS OR DECLINE CHANCE CALCULATOR
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
