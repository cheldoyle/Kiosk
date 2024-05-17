using System.Transactions;

namespace Kiosk {
    internal class Program {
        private static void Main(string[] args) {

            KioskMachine kiosk = new KioskMachine(500);
            kiosk.InputRequest();
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