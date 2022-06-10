using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        bool playAgain = true;
        int pauseDuration = 500;
        // Initialise arrays
        string[] questions = {
            "What from the following is an example of a EMBEDDED SYSTEM?\nA: Smartphone\nB: Microwave\nC: Keyboard\nD: Mouse\n",
            "What are the three types of bus?\nA: Address, Control and Data\nB: Data, Control and Instruction\nC: Instruction, Address and Data\nD: Address, Instruction and Control\n",
            "What program holds the address of the next instruction waiting to be fetched from memory?\nA: MDR\nB: MAR\nC: Accumulator\nD: Program Counter\n",
            "What prevention method can stop data interception via a third party hacker?\nA: Encryption\nB: User-Access Levels\nC: Physical Security\nD: Penetration Testing\n",
            "What is a server?\nA: A computer that controls security\nB: A storage device\nC: A computer that writes files\nD: A computer that manages and stores files\n",
            "What is the purpose of a domain name server?\nA: To act as a understandable name instead of the IP Address. \nB: To demonstrate the true value of the Domain\nC: To replace the IP address\nD: To remove an IP Address\n",
            "What program holds the address of the next instruction waiting to be fetched from memory?\nA: MDR\nB: MAR\nC: Accumulator\nD: Program Counter\n",
            "What program holds the address of the next instruction waiting to be fetched from memory?\nA: MDR\nB: MAR\nC: Accumulator\nD: Program Counter\n",
            "What program holds the address of the next instruction waiting to be fetched from memory?\nA: MDR\nB: MAR\nC: Accumulator\nD: Program Counter\n",
            "What program holds the address of the next instruction waiting to be fetched from memory?\nA: MDR\nB: MAR\nC: Accumulator\nD: Program Counter\n"
            };

        string[] answers = {
            "b",
            "a",
            "d",
            "a",
            "d",
            "a",
            "d",
            "d",
            "d",
            "d"
        };

        string[] displayAnswers = {
          "microwave",
          "address, control and data",
          "program counter",
          "encryption",
          "a computer that manages and stores files",
          "to act as an understandable name instead of an ip address",
          "program counter",
          "program counter",
          "program counter",
          "program counter"
        };

        // Length of array
        int lenOfQuestions = questions.Length;
        while (playAgain)
        {
            bool isNewUser = false;
            bool selectedItem = true;
            while (selectedItem)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Are you an existing user or a new user?");
                Console.Write("Enter 1 for a new user, or 2 for an existing user: ");
                Console.ResetColor();
                int selection = Convert.ToInt32(Console.ReadLine());
                if (selection == 1)
                {
                    isNewUser = true;
                    selectedItem = false;
                }
                else if (selection == 2)
                {
                    isNewUser = false;
                    selectedItem = false;
                }
                else
                {
                    Console.WriteLine("Invalid answer. Please try again!");
                    selectedItem = true;
                }
            }

            List<string> usernames = new List<string>();
            List<string> passwords = new List<string>();

            using (StreamReader file = new StreamReader("logins.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] columns = line.Split('=');
                    string username = columns[0].Trim();
                    string password = columns[1].Trim();

                    usernames.Add(username);
                    passwords.Add(password);
                }
            }

            /*
            var fileContents = File.ReadLines("logins.txt");
            foreach (string line in fileContents)
            {
                string[] columns = line.Trim('\n').Split(' ');
                string username = columns[0];
                string password = columns[1];

                usernames.Add(username);
                passwords.Add(password);
            }
            */

            /*
            usernames.ForEach(i => Console.Write("{0}\n", i));
            passwords.ForEach(i => Console.Write("{0}\n", i));
            */

            string currUser = "";
            string userPassword = "";

            if (isNewUser)
            {
                bool passedVerification = false;

                while (passedVerification == false)
                {
                    Console.Write("Please enter your new username: ");
                    string username = Console.ReadLine();

                    Console.Write("Please enter your new password: ");
                    string password = Console.ReadLine();

                    if (usernames.Contains(username))
                    {
                        // Cannot make a new account using existing login details
                        Console.WriteLine("This account is already in use. Please try again.");
                    }
                    else if (password.Length < 8)
                    {
                        // The password provided is too short
                        Console.WriteLine("The password is too short. Please try again.");
                    }
                    else
                    {
                        passedVerification = true;
                        Console.WriteLine($"Registration success. You are now logged in as {username} \n");
                        currUser = username;
                        userPassword = password;
                    }
                    using (StreamWriter sw = File.AppendText("logins.txt"))
                    {
                        sw.WriteLine($"{currUser}={userPassword}");
                    }
                }
            } else {
              bool loginSuccess = false;

              while(loginSuccess == false) {
                Console.Write("Please enter your username: ");
                string username = Console.ReadLine();

                Console.Write("Please enter your password: ");
                string password = Console.ReadLine();

                for (int i = 0; i < usernames.Count; i++) {
                  if (usernames[i] == username && passwords[i] == password) {
                    loginSuccess = true;
                    currUser = username;
                    userPassword = password;
                  }
                }
                if (loginSuccess) {
                  Console.WriteLine($"Login Success. You are now logged in as {username} \n");
                } else {
                  Console.WriteLine($"Login failed. Please check your login details and try again.");
                }
              }
            }

            Console.WriteLine($"Welcome {currUser} to the GCSE Computer Science OCR quiz.");
            Console.WriteLine("You will be tested on 10 questions from the OCR GCSE Computer Science course.\n");
            Thread.Sleep(pauseDuration);

            // Players score
            int score = 0;

            // Loop through questions
            for (int i = 1; i < lenOfQuestions + 1; i++)
            {  
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"QUESTION {i}:\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(questions[i - 1]);
                Console.Write("A, B, C or D?: ");
                Console.ResetColor();
                string ans = Console.ReadLine().ToLower().Trim();
                if (ans == answers[i - 1])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nCORRECT!!!\n");
                    Console.ResetColor();
                    score++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nWRONG!!!\n");
                    Console.ResetColor();
                    Console.WriteLine($"The correct answer is {answers[i - 1].ToUpper()}: {displayAnswers[i - 1]}\n");
                }
                Thread.Sleep(pauseDuration);
            };
            Console.WriteLine("The quiz is complete.");
            Console.WriteLine($"Your final score is {score} out of {lenOfQuestions}");
            if (score < 5)
            {
                Console.WriteLine("Better luck next time!");
            }
            else if (score < 8)
            {
                Console.WriteLine("Pretty good!");
            }
            else if (score < 10)
            {
                Console.WriteLine("Excellent score, well done!");
            }
            else
            {
                Console.WriteLine("Full marks!");
            }
            Console.Write("\nWould you like to play again (yes or no)?: ");
            string nextGame = Console.ReadLine();
            if (nextGame.ToLower().Trim() == "yes") {
              Console.Clear();
            } else {
              Console.WriteLine("Thanks for playing!");
              playAgain = false;
            }
        }
    }
}