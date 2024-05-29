using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Kiosk {
    internal class CurrencyKeeper {

        static double[] denoms = { .01, .05, .10, .25, .50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 }; // EACH DENOM
        static int[] denomsAmnts = { 50, 50, 50, 50, 5, 20, 5, 20, 20, 20, 10, 10 }; // AMOUNT OF EACH DENOM

        // CASH FUNCTIONS
        public static string[] MakePayment(double totalSpent) {
            string[] receipts = new string[4]; // CREATES NEW LIST FOR RETURNING RECEIPT INFO
            double paid = 0; // TOTAL AMOUNT PAID IN CASH
            double paidAmnt = 0; // INDIVIDUAL PAYMENTS
            double inputTotal = totalSpent; // DUPLICATES INPUT FOR MATH
            double leftOver = 0; // FOR DETERMINING CHANGE NEEDED
            bool paymentSuccess = false; // CHECKS IF AMOUNT ENTERED IS VALID
            int i = 1; // COUNTER FOR PAYMENT NUMBER

            Console.Write("\n--- Valid Currency Amounts (Coins and Dollars) Only ---\nAllowed Amounts: ");
            for (int c = 0; c < denoms.Length; c++) {
                Console.Write($"{denoms[c]:C} "); // DISPLAYS ALLOWED PAYMENT AMOUNT INPUTS
            }
            while (inputTotal > 0) { // CONTINUES WHILE TOTAL TO PAY IS ABOVE 0
                paidAmnt = Program.PromptDubs($"\nCurrent total: {totalSpent:C}\nPayment {i}: "); // ASKS FOR PAYMENT
                paymentSuccess = CurrencyCheck(paidAmnt); // CHECKS IF PAYMENT == ONE OF ALLOWED PAYMENT AMOUNTS
                while (paymentSuccess == false) { // CHECKS IF PAYMENT AMOUNT INVALID
                    paidAmnt = Program.PromptDubs($"\nError: Invalid Payment Amount\nCurrent total: {totalSpent:C}\nEnter how much you want to pay for payment {i}: "); // REQUESTS NEW PAYMENT INPUT
                    paymentSuccess = CurrencyCheck(paidAmnt);
                }
                i++; // PAYMENT NUMBER GOES UP
                paid += paidAmnt; // ADDS EACH PAID AMOUNT TO TOTAL PAID
                inputTotal -= paidAmnt; // REMOVES AMOUNT PAID FROM ORIGINAL TOTAL
                inputTotal = Math.Round(inputTotal, 2); // SETS TO XX.XX FORMAT FOR MATH
            }
            if (inputTotal < 0) { // CHECKS IF PAID TOTAL IS BELOW 0
                leftOver = paid - totalSpent; // DETERMINES HOW MUCH CHANGED NEEDED
                leftOver = Math.Round(leftOver, 2); // XX.XX FORMAT FOR MATH
                receipts[1] = Convert.ToString($"{leftOver:C}"); // ADDS TO RECEIPT INFO
                Console.WriteLine($"\nTotal Paid: {paid:C}\nChange Due: {leftOver:C}"); // DISPLAYS HOW MUCH PAID AND CHANGE DUE
                MakeChange(leftOver); // GOES INTO FUNCTION TO MAKE CHANGE
            } else if (inputTotal == 0) { // IN CASE AMOUNT PAID IS EXACTLY HOW MUCH OWED
                Console.WriteLine($"\nTotal Paid: {paid:C}\nChange Due: $0.00\n\n--- Thank You for Your Patronage ---\n");
            }
            receipts[0] = Convert.ToString($"{paid:C}"); // CONVERTS PAID TO STRING FOR RECEIPT
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

        public static void MakeChange(double totalPrice) { // CHANGE MAKER
            int denomLen = denoms.Length; // LENGTH OF HOW MANY DENOMINATIONS THERE ARE
            List<double> totalChange = new List<double>(); // LIST FOR EACH CHANGE AMOUNT
            bool kioskEmpty = false; // CHECKS IF NO CASH LEFT IN KIOSK

            for (int i = denomLen - 1; i >= 0; i--) { // GOES THROUGH DENOMS BY LENGTH
                while (totalPrice >= denoms[i]) { // CONTINUES SORTING THROUGH CHANGE WHILE TOTAL IS ABOVE DENOM AMOUNT
                    if (denomsAmnts[i] == 0) { // CHECKS IF AMOUNT IN KIOSK IS EMPTY
                        denoms[i] = 0; // SETS THAT CURRENCY TO 0 IF NONE IN KIOSK
                        break; // BREAKS OUT OF LOOP
                    } else {
                        denomsAmnts[i]--; // OTHERWISE, IT REMOVES ONE FROM THE AMOUNTS
                        totalPrice -= denoms[i]; // REMOVES DENOM AMOUNT FROM TOTALPRICE
                        totalChange.Add(denoms[i]); // ADDS CHANGE CHOSEN TO LIST
                    }
                }
                for (int j = 0; j < denomLen; j++) {
                    if (denoms[i] != 0) { // CHECKS IF THERE'S STILL MONEY IN KIOSK
                        kioskEmpty = false; // THE MOMENT IT HITS SOMETHING THAT ISN'T 0
                        break; // BREAK
                    } else {
                        kioskEmpty = true; // OTHERWISE, MAKE KIOSK EMPTY = TRUE
                    }
                }
                if (kioskEmpty) { // CHECKS IF NOTHING IN KIOSK
                    string noMoney = Program.Prompt("\nKiosk is Currently Out of Money. Return to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n"); // ALLOWS USER TO RETURN TO CHOOSE CREDIT
                    noMoney = noMoney.ToLower();
                    while (noMoney != "return" ||  noMoney != "exit") { // KEEPS IN LOOP WHILE INPUT ISN'T ONE OF ALLOWED OPTIONS
                        if (noMoney == "return") { // IF USER CHOOSES RETURN
                            Program.MakePayment(totalPrice); // PROGRAM RETURNS TO ORIGINAL SCREEN
                        } else if (noMoney == "exit") { // IF USER DECIDES TO EXIT TRANSACTION
                            return; // LEAVE ENTIRELY
                        } else { // CHECKS IF VALID INPUT
                            Console.WriteLine("\nError: Invalid Input. Please Try Again.");
                            noMoney = Program.Prompt("\nKiosk is Currently Out of Money. Return to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                        }
                    }
                }
            }
            Console.WriteLine("\n--- Thank You For Your Patronage! Please Take Your Change ---");
            for (int i = 0; i < totalChange.Count; i++) { // DISPLAYS EACH CHANGE DISPENSED
                Console.WriteLine($"{totalChange[i]:C} dispensed");
            }
        }

        // CREDIT CARD FUNCTIONS

        public static string[] ValidEntry(double amountSpent) {
            string[] receipts = new string[4]; // RECEIPT INFO TO GO TO PROGRAM
            string cardNo = ""; // CARD NUMBER
            string cardType = ""; // CARD TYPE (VISA, MC, DISCOVER, AE)
            bool validIn = true; // CHECKS FOR VALID INPUT FOR CREDIT CARD ENTRY
            string[] cardValid = new string[2]; // STRING ARRAY FOR RETURN PASS OR DECLINED STATE
            double paidAmnt = 0; // AMOUNT PAID BY CARD - AMOUNTSPENT
            double cashAmnt = 0; // CASH REQUESTED
            bool cashBackBool = true; // CHECKS IF USER WANTS CASH BACK
            bool gotCash = false; // GOES TO TRUE IF USER WANTS CASH
            bool notEntered = true; // CHECKS FOR ISSUES IN USER INPUT FOR CREDIT CARD
            bool partial = false;
            double partialPay = 0;

            string cashBack = Program.Prompt("\nCash Back? Y | N\n"); // ASKS FOR CASH BACK FIRST
            cashBack = cashBack.ToLower();

            while (cashBackBool) {
                if (cashBack == "y") { // IF Y
                    cashAmnt = Program.PromptDubs("\n[Max $100.00] Cash Back Amount: "); // SETS MAX TO AVOID TOO HIGH NUMS
                    if (cashAmnt <= 100) { // CHECKS FOR LESS THAN OR EQUAL TO MAX
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


            while (validIn) { // KEEPS USER IN LOOP FOR VALID CC INPUT
                cardNo = Program.Prompt("\nEnter Your Credit Card Number: ");
                if (cardNo.Length < 15) { // CHECKS FOR APPROPRIATE CC LENGTH
                    while (notEntered) {
                        cardNo = Program.Prompt("\nEntry Error: Enter Your Credit Card Number: ");
                        if (cardNo.Length !< 15) { // EXITS WHEN 15+ CC ENTRY
                            notEntered = false;
                        }
                    }
                }
                if (checkLuhn(cardNo) == true) { // CHECKS FOR VALID CC INPUT
                    cardValid = MoneyRequest(cardNo, amountSpent); // GRABS AMOUNT AND PAYMENT STATE FROM RANDOM DECLINE FUNCTION
                    cardType = CardType(cardNo); // GRABS CARDTYPE
                    receipts[0] = cardType; // SETS FIRST ITEM IN LIST TO CARD TYPE
                    validIn = false; // EXITS LOOP
                } else {
                    Console.WriteLine("\nInvalid Input. Please Try Again.");
                }
            }
            Console.WriteLine($"\nCard Number: {cardType} {cardValid[0]}"); // DISPLAYS CARD TYPE AND PAID AMOUNT

            if (cardValid[1] != "declined") {
                partialPay = double.Parse(cardValid[1]);
            }

            if (partialPay < amountSpent) {
                partial = true;
            }

            if (cardValid[1] != "declined" && partial == false) { // CHECKS IF SECOND ENTRY IS NOT DECLINED
                paidAmnt = double.Parse(cardValid[1]); // DOUBLE PARSE TOTAL PAID
                paidAmnt = Math.Round(paidAmnt, 2); // FOR MATH XX.XX FORMAT
                receipts[1] = Convert.ToString($"{paidAmnt:C}"); // SETS SECOND ITEM TO PAID AMOUNT FOR CREDIT
                Console.WriteLine($"Amount Paid: {paidAmnt:C}");
                if (gotCash && cashAmnt != 0) { // CHECKS IF CASH REQUESTED
                    Console.WriteLine($"Cash Back: {cashAmnt:C}"); // DISPLAYS CASH BACK
                    receipts[2] = Convert.ToString(cashAmnt); // ADDS CASH BACK TO RECEIPT
                    MakeChange(cashAmnt); // GOES THROUGH CHANGE MAKER TO DISPLAY CASH BACK
                }
            }
            else if (cardValid[1] == "declined" && cashAmnt == 0) { // CHECKS IF DECLINED AND NO CASH BACK
                string noMoney = Program.Prompt($"\nCard Has Been Declined.\nBalance Owed: {amountSpent:C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                noMoney = noMoney.ToLower();
                while (noMoney != "return" || noMoney != "exit") {
                    if (noMoney == "return") {
                        Program.MakePayment(amountSpent); // GOES BACK TO MAKE PAYMENT IN PROGRAM TO CHOOSE ANOTHER METHOD
                        break;
                    } else if (noMoney == "exit") {
                        break; // BREAKS FROM PROGRAM IF EXITING
                    } else {
                        Console.WriteLine("\nError: Invalid Input. Please Try Again.");
                        noMoney = Program.Prompt($"\nCard Has Been Declined.\nBalance Owed: {amountSpent:C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                    }
                }
            }
            else if (cardValid[1] == "declined" && cashAmnt != 0) { // CHECKS IF DECLINED AND CASH BACK
                string noMoney = Program.Prompt($"\nCard Has Been Declined.\nBalance Owed: {amountSpent:C}\nCash Back Amount: {cashAmnt:C}\nEnter a Different Card for Payment or Exit Transaction?\nReturn | Exit\n");
                noMoney = noMoney.ToLower();
                while (noMoney != "return" || noMoney != "exit") {
                    if (noMoney == "return") {
                        validIn = true;
                        while (validIn) { // REENTERS CC VALIDATION IF DECLINED AND WANTS CASH BACK
                            cardNo = Program.Prompt("\nEnter Your Credit Card Number: ");
                            if (cardNo == null || cardNo == "" || cardNo.Length < 15) {
                                notEntered = true;
                                while (notEntered) {
                                    cardNo = Program.Prompt("\nEntry Error: Enter Your Credit Card Number: ");
                                    if (cardNo != null && cardNo != "" && cardNo.Length! < 15) {
                                        notEntered = false;
                                    }
                                }
                            }
                            if (checkLuhn(cardNo) == true) {
                                cardValid = MoneyRequest(cardNo, amountSpent);
                                cardType = CardType(cardNo);
                                receipts[0] = cardType;
                                validIn = false;
                            } else {
                                Console.WriteLine("\nInvalid Input. Please Try Again.");
                            }
                            if (cardValid[1] != "declined") {
                                paidAmnt = double.Parse(cardValid[1]);
                                paidAmnt = Math.Round(paidAmnt, 2);
                                receipts[1] = Convert.ToString(paidAmnt);
                                Console.WriteLine($"Amount Paid: {paidAmnt:C}");
                                if (gotCash && cashAmnt != 0) { // MAKES SURE KIOSK GAVE MONEY
                                    Console.WriteLine($"Cash Back: {cashAmnt:C}");
                                    receipts[2] = Convert.ToString($"{cashAmnt:C}");
                                    MakeChange(cashAmnt);
                                }
                            }
                        }
                        break;
                    } else if (noMoney == "exit") {
                        break;
                    } else {
                        Console.WriteLine("\nError: Invalid Input. Please Try Again.");
                        noMoney = Program.Prompt($"\nCard Has Been Declined.\nBalance Owed: {amountSpent:C}\nCash Back Amount: {cashAmnt:C}\nEnter a Different Card for Payment or Exit Transaction?\nReturn | Exit\n");
                    }
                }
            }
            else if (cardValid[1] == "declined" && cashAmnt == 0) { // CHECKS IF DECLINED AND NO CASH BACK
                string noMoney = Program.Prompt($"\nCard Has Been Declined.\nBalance Owed: {amountSpent:C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                noMoney = noMoney.ToLower();
                while (noMoney != "return" || noMoney != "exit") {
                    if (noMoney == "return") {
                        Program.MakePayment(amountSpent); // GOES BACK TO MAKE PAYMENT IN PROGRAM TO CHOOSE ANOTHER METHOD
                        break;
                    } else if (noMoney == "exit") {
                        break; // BREAKS FROM PROGRAM IF EXITING
                    } else {
                        Console.WriteLine("\nError: Invalid Input. Please Try Again.");
                        noMoney = Program.Prompt($"\nCard Has Been Declined.\nBalance Owed: {amountSpent:C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                    }
                }
            }
            // NOT FINISHED - HAVE TO MAKE UP LOST AMOUNT
            // ALSO NEED TO ADD CC NUMS TO LIST SO THEY CANNOT BE REENTERED
            else if (cardValid[1] != "declined") {
                if (partialPay < amountSpent) {
                    double amountLeft = amountSpent - partialPay;
                    while (amountLeft != 0) {
                        partialPay = double.Parse(cardValid[1]);
                        partial = true;
                        if (partialPay < paidAmnt && cashAmnt != 0) { // CHECKS IF DECLINED AND CASH BACK
                            string noMoney = Program.Prompt($"\nCard Has Been Partially Declined.\nAmount Paid: {partialPay:C}\nBalance Owed: {(amountSpent - partialPay):C}\nCash Back Amount: {cashAmnt:C}\nEnter a Different Card for Payment or Exit Transaction?\nReturn | Exit\n");
                            noMoney = noMoney.ToLower();
                            while (noMoney != "return" || noMoney != "exit") {
                                if (noMoney == "return") {
                                    validIn = true;
                                    while (validIn) { // REENTERS CC VALIDATION IF DECLINED AND WANTS CASH BACK
                                        cardNo = Program.Prompt("\nEnter Your Credit Card Number: ");
                                        if (cardNo == null || cardNo == "" || cardNo.Length < 15) {
                                            notEntered = true;
                                            while (notEntered) {
                                                cardNo = Program.Prompt("\nEntry Error: Enter Your Credit Card Number: ");
                                                if (cardNo != null && cardNo != "" && cardNo.Length! < 15) {
                                                    notEntered = false;
                                                }
                                            }
                                        }
                                        if (checkLuhn(cardNo) == true) {
                                            cardValid = MoneyRequest(cardNo, (partialPay));
                                            cardType = CardType(cardNo);
                                            receipts[0] = cardType;
                                            validIn = false;
                                        } else {
                                            Console.WriteLine("\nInvalid Input. Please Try Again.");
                                        }
                                        if (cardValid[1] != "declined") {
                                            paidAmnt = double.Parse(cardValid[1]);
                                            paidAmnt = Math.Round(paidAmnt, 2);
                                            receipts[1] = Convert.ToString(paidAmnt);
                                            Console.WriteLine($"Amount Paid: {(paidAmnt - partialPay):C}");
                                            if (gotCash && cashAmnt != 0) { // MAKES SURE KIOSK GAVE MONEY
                                                Console.WriteLine($"Cash Back: {cashAmnt:C}");
                                                receipts[2] = Convert.ToString($"{cashAmnt:C}");
                                                MakeChange(cashAmnt);
                                            }
                                        }
                                    }
                                    break;
                                } else if (noMoney == "exit") {
                                    break;
                                } else {
                                    Console.WriteLine("\nError: Invalid Input. Please Try Again.");
                                    noMoney = Program.Prompt($"\nCard Has Been Declined.\nAmount Paid: {partialPay:C}\nBalance Owed: {(paidAmnt - partialPay):C}\nCash Back Amount: {cashAmnt:C}\nEnter a Different Card for Payment or Exit Transaction?\nReturn | Exit\n");
                                }
                            }
                        }
                        if (partialPay < paidAmnt && cashAmnt == 0) { // CHECKS IF DECLINED AND NO CASH BACK
                            string noMoney = Program.Prompt($"\nCard Has Been Partially Declined.\nAmount Paid: {partialPay:C}\nBalance Owed: {(amountSpent - partialPay):C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                            noMoney = noMoney.ToLower();
                            while (noMoney != "return" || noMoney != "exit") {
                                if (noMoney == "return") {
                                    Program.MakePayment(partialPay); // GOES BACK TO MAKE PAYMENT IN PROGRAM TO CHOOSE ANOTHER METHOD
                                    break;
                                } else if (noMoney == "exit") {
                                    break; // BREAKS FROM PROGRAM IF EXITING
                                } else {
                                    Console.WriteLine("\nError: Invalid Input. Please Try Again.");
                                    noMoney = Program.Prompt($"\nCard Has Been Declined.\nAmount Paid: {partialPay:C}\nBalance Owed: {(amountSpent - partialPay):C}\nReturn to Payment Screen to Choose a Different Method or Exit Transaction?\nReturn | Exit\n");
                                }
                            }
                        }
                        amountLeft -= partialPay;
                    }
                }
            }
            return receipts;
        }
        static double CashBack(double cashWanted) {
            double returnCash = 0; // HOW MUCH TO RETURN FOR CASH BACK
            int denomLen = denoms.Length;
            bool kioskEmpty = false;
            double totalWanted = cashWanted;
            // GOES THROUGH KIOSK SIMILAR TO MAKING CHANGE - CHECKS IF KIOSK CAN MAKE CHANGE
            for (int i = denomLen - 1; i >= 0; i--) { // GOES THROUGH DENOMS BY LENGTH
                while (totalWanted >= denoms[i]) { // CONTINUES SORTING THROUGH CHANGE WHILE TOTAL IS ABOVE DENOM AMOUNT
                    if (denomsAmnts[i] == 0) { // CHECKS IF AMOUNT IN KIOSK IS EMPTY
                        denoms[i] = 0; // SETS THAT CURRENCY TO 0 IF NONE IN KIOSK
                        break; // BREAKS OUT OF LOOP
                    } else {
                        denomsAmnts[i]--; // OTHERWISE, IT REMOVES ONE FROM THE AMOUNTS
                        totalWanted -= denoms[i];
                        returnCash += denoms[i];
                    }
                }
                for (int j = 0; j < denomLen; j++) {
                    if (denoms[i] != 0) { // CHECKS IF THERE'S STILL MONEY IN KIOSK
                        kioskEmpty = false; // THE MOMENT IT HITS SOMETHING THAT ISN'T 0
                        break; // BREAK
                    } else {
                        kioskEmpty = true; // OTHERWISE, MAKE KIOSK EMPTY = TRUE
                    }
                }
                if (kioskEmpty) { // CHECKS IF NOTHING IN KIOSK
                    Console.Write("Error: Cannot Complete Cash Back Request - Returning to Payment");
                    returnCash = 0;
                    break;
                }
            }
            return returnCash;
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
