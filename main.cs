using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public class Program
{
    public class RowComparer : IComparer<string>
    {
        private int _columnIndex;

        public RowComparer(int columnIndex)
        {
            _columnIndex = columnIndex;

        }
        public int Compare(string a, string b)
        {
            string valA = a.Split(',')[_columnIndex];
            string valB = b.Split(',')[_columnIndex];
            if (valA == valB) return 0;

            return int.Parse(valA) > int.Parse(valB) ? 1 : -1;

        }
    }

    public class User
    {
        public string username { get; set; }
        public int score { get; set; }
    }

    public sealed class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(m => m.username).Name("Username");
            Map(m => m.score).Name("Score");
        }
    }
    public static void Main(string[] args)
    {
        // Variables
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

        // Check whether a user is a new user or an existing user.
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

            string currUser = "";
            string userPassword = "";

            // Verify the users credentials when making a new account.
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
                    // Append the login details to a text file.
                    using (StreamWriter sw = File.AppendText("logins.txt"))
                    {
                        sw.WriteLine($"{currUser}={userPassword}");
                    }
                }
            }
            else
            {
                bool loginSuccess = false;

                while (loginSuccess == false)
                {
                    // Logging in with username and password if user exists in text file
                    Console.Write("Please enter your username: ");
                    string username = Console.ReadLine();

                    Console.Write("Please enter your password: ");
                    string password = Console.ReadLine();

                    for (int i = 0; i < usernames.Count; i++)
                    {
                        if (usernames[i] == username && passwords[i] == password)
                        {
                            loginSuccess = true;
                            currUser = username;
                            userPassword = password;
                        }
                    }
                    if (loginSuccess)
                    {
                        Console.WriteLine($"Login Success. You are now logged in as {username} \n");
                    }
                    else
                    {
                        Console.WriteLine($"Login failed. Please check your login details and try again.");
                    }
                }
            }

            // Start of quiz
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
            // Quiz is complete
            Console.WriteLine("The quiz is complete.");
            Console.WriteLine($"Your final score is {score} out of {lenOfQuestions}");
            // Anaylse the users score.
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

            // Appends username and score to csv file.
            var csvPath = File.Open("scores.csv", FileMode.Append);
            using (var streamWriter = new StreamWriter(csvPath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Don't write the header again.
                    HasHeaderRecord = false,
                };

                using (var csvWriter = new CsvWriter(streamWriter, config))
                {
                    csvWriter.WriteField(currUser);
                    csvWriter.WriteField(score);
                    csvWriter.NextRecord();
                }
            }

            // Sorting CSV File
            List<string> rows = File.ReadAllLines("scores.csv").ToList();
            rows = rows.Skip(1).OrderByDescending(r => r, new RowComparer(1)).ToList();

            File.WriteAllText("scores.csv", "");
            // var csvPath = File.Open("scores.csv", FileMode.Append);
            using (StreamWriter sw = File.AppendText("scores.csv"))
            {
                sw.WriteLine("Username,Score");
            }
            File.AppendAllLines("scores.csv", rows);

            // Console.WriteLine the scores to the console.
            using (var reader = new StreamReader("scores.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<UserMap>();
                var users = csv.GetRecords<User>();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Leaderboards: ");
                foreach (var user in users.Take(3))
                {
                    Console.WriteLine($"{user.username} - {user.score}");
                }
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nWould you like to play again (yes or no)?: ");
            Console.ResetColor();
            string nextGame = Console.ReadLine();
            if (nextGame.ToLower().Trim() == "yes")
            {
                Console.Clear();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Thanks for playing!");
                Console.ResetColor();
                playAgain = false;
            }
        }
    }
}