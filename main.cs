using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MySql.Data.MySqlClient;
using Spectre.Console;

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
        Console.Clear();
        string connectionString = @"server=localhost;userid=sqluser;password=RNAV2022";
        var connection = new MySqlConnection(connectionString);
        connection.Open();
        // Variables
        bool playAgain = true;
        int pauseDuration = 500;
        // Initialise arrays
        List<string> questions = new List<string>();
        List<string> answers = new List<string>();
        List<List<String>> options = new List<List<string>>();

        using (var cmd = new MySqlCommand("SELECT Question, Answer, OptionA, OptionB, OptionC, OptionD FROM `GCSEComputerScienceQuiz`.`QUIZ`", connection))
        using (var reader = cmd.ExecuteReader())
            while (reader.Read())
            {
                questions.Add(reader.GetString(0));
                answers.Add(reader.GetString(1));
                List<string> data = new List<string> { reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5) };
                options.Add(data);
            }

        // Check whether a user is a new user or an existing user.
        int lenOfQuestions = questions.Count;
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

            using (var cmd = new MySqlCommand("SELECT Username, Password FROM `GCSEComputerScienceQuiz`.`USERS`", connection))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    usernames.Add(reader.GetString(0));
                    passwords.Add(reader.GetString(1));
                }

            // var usernamesAndPasswords = usernames.Zip(passwords, (u, p) => new { Username = u, Password = p });
            // foreach (var item in usernamesAndPasswords)
            // {
            //     Console.WriteLine($"{item.Username} - {item.Password}");
            // }

            // using (StreamReader file = new StreamReader("logins.txt"))
            // {
            //     string line;
            //     while ((line = file.ReadLine()) != null)
            //     {
            //         string[] columns = line.Split('=');
            //         string username = columns[0].Trim();
            //         string password = columns[1].Trim();

            //         usernames.Add(username);
            //         passwords.Add(password);
            //     }
            // }

            string currUser = "";
            string userPassword = "";

            // Verify the users credentials when making a new account.
            if (isNewUser)
            {
                bool passedVerification = false;

                Console.Write("Please enter your new username: ");
                string username = Console.ReadLine();
                string password = "";
                while (passedVerification == false)
                {

                    Console.Write("Please enter your new password: ");
                    password = Console.ReadLine();

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
                        AnsiConsole.Write(new Markup("Press [cyan]ENTER[/] to continue\n"));
                        Console.ReadLine();
                        Console.Clear();
                    }
                }
                // Append the login details to a text file.
                string sqlCommandInsertInto = "INSERT INTO `GCSEComputerScienceQuiz`.`USERS` (Username, Password) VALUES (@username, @password)";
                var cmd = new MySqlCommand(sqlCommandInsertInto, connection);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
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
                        AnsiConsole.Write(new Markup($"Login [green underline]Success[/]. You are now logged in as {username}\n"));
                        AnsiConsole.Write(new Markup("Press [cyan]ENTER[/] to continue\n"));
                        Console.ReadLine();
                        Console.Clear();
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup($"Login [red underline]Failed[/]. Please check your login details and try again.\n"));
                    }
                }
            }

            // Start of quiz
            string greeting = $"Welcome {currUser} to the GCSE Computer Science OCR quiz. \nYou will be tested on 10 questions from the OCR GCSE Computer Science course.";
            Console.WriteLine(greeting);
            Thread.Sleep(pauseDuration);

            // Players score
            int score = 0;
            string index = "ABCD";

            // Loop through questions
            for (int i = 1; i < lenOfQuestions + 1; i++)
            {
                int j;
                switch (answers[i - 1])
                {
                    case "A":
                        j = 0;
                        break;

                    case "B":
                        j = 1;
                        break;

                    case "C":
                        j = 2;
                        break;

                    case "D":
                        j = 3;
                        break;

                    default:
                        j = 0;
                        break;
                }


                string ans = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"Question {i}: {questions[i - 1]}")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.CornflowerBlue))
                        .MoreChoicesText("[blue](Move up and down to reveal more options)[/]")
                        .UseConverter(value => $"{value.ToUpper()} : {options[i - 1][index.IndexOf(value)]}")
                        .AddChoices(new[] {
                            "A", "B", "C", "D"
                        }));

                if (ans == answers[i - 1])
                {
                    // Console.ForegroundColor = ConsoleColor.Green;
                    // Console.WriteLine("\nCORRECT!!!\n");
                    // Console.ResetColor();
                    AnsiConsole.Write(new Markup("[green bold]CORRECT[/]\n"));
                    score++;
                    AnsiConsole.Write(new Markup("Press [cyan]ENTER[/] to continue\n"));
                    Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine(greeting);
                    AnsiConsole.Write(new Markup($"[green italic dim]Score : {score} / {lenOfQuestions}[/]\n"));
                }
                else
                {
                    AnsiConsole.Write(new Markup("[red bold]Wrong[/]\n"));
                    AnsiConsole.Write(new Markup($"The correct answer is [yellow bold underline]{answers[i - 1].ToUpper()}: {options[i - 1][j]}[/]\n"));
                    AnsiConsole.Write(new Markup("Press [cyan]ENTER[/] to continue"));
                    Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine(greeting);
                    AnsiConsole.Write(new Markup($"[green italic dim]Score : {score} / {lenOfQuestions}[/]\n"));
                }
                // Thread.Sleep(pauseDuration);
            };
            // Quiz is complete
            Console.WriteLine("The quiz is complete.");
            Console.WriteLine($"Your final score is {score} out of {lenOfQuestions}");
            // Anaylse the users score.
            if (score < ((int)(0.5 * lenOfQuestions)))
            {
                Console.WriteLine("Better luck next time!");
            }
            else if (score < ((int)(0.8 * lenOfQuestions)))
            {
                Console.WriteLine("Pretty good!");
            }
            else
            {
                Console.WriteLine("Excellent score, well done!");
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