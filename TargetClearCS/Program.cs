//Skeleton Program code for the AQA A Level Paper 1 Summer 2025 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetClearCS
{
    public static class Trace
    {
        public static class Table
        {
            public static List<(string, object)> History = new List<(string, object)>();


            public static void Set<T>(string id, ref T obj, T value)
            {
                History.Add((id, Trace.Format(value)));
                obj = value;
            }
            public static void Became<T>(string id, T value)
            {
                History.Add((id, value));
            }

            public static string[] GetTable()
            {
                HashSet<string> list = new HashSet<string>();
                foreach (var i in History) list.Add(i.Item1.ToString());
                string[] titles = list.ToArray();
                int[] widest = new int[titles.Length];
                for (int i = 0; i < widest.Length; i++) widest[i] = titles[i].Length;
                List<string[]> values = new List<string[]>() { new string[titles.Length] };
                foreach (var i in History)
                {
                    if (values.Last()[Array.IndexOf(titles, i.Item1)] != null) values.Add(new string[titles.Length]);
                    values.Last()[Array.IndexOf(titles, i.Item1)] = i.Item2.ToString();
                    widest[Array.IndexOf(titles, i.Item1)] = Math.Max(values.Last()[Array.IndexOf(titles, i.Item1)].Length, widest[Array.IndexOf(titles, i.Item1)]);
                }
                for (int i = 0; i < values.Count; i++) for (int j = 0; j < values[i].Length; j++) if (values[i][j] == null) values[i][j] = string.Empty;
                for (int i = 0; i < titles.Length; i++) titles[i] = $" {titles[i].PadLeft(widest[i])} ";
                foreach (var i in values) for (int j = 0; j < i.Length; j++) i[j] = $" {i[j].PadLeft(widest[j])} ";
                List<string> output = new List<string>() { titles.Aggregate(string.Empty, (i,x) => $"{i}|{x}") };
                output.Add(new string('=', output.Last().Length));
                foreach (var i in values)
                {
                    output.Add(i.Aggregate(string.Empty, (j, x) => $"{j}|{x}"));
                }
                return output.ToArray();
            }
        }

        const string indentString = "  |  ";
        static string indent = "";
        static string name = $"Game started {DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second} on {DateTime.Now.Date.Day}_{DateTime.Now.Date.Month}.txt";

        public static string Format(object input)
        {
            if (input is List<int>)
            {
                List<int> formInput = (List<int>)input;
                string returner = "[";
                foreach (object i in formInput)
                {
                    returner += $"'{Format(i)}',";
                }
                return returner.TrimEnd(',') + "]";
            }
            if (input is List<string>)
            {
                List<string> formInput = (List<string>)input;
                string returner = "[";
                foreach (object i in formInput)
                {
                    returner += $"'{Format(i)}',";
                }
                return returner.TrimEnd(',') + "]";
            }
            return input.ToString();
        }
        public static void Open(string write, params (string, object)[] parameters)
        {
            using (StreamWriter sw = new StreamWriter(name,true))
            {
                sw.WriteLine($"{indent}RUN {write}");
                foreach ((string,object) p in parameters)
                {
                    sw.WriteLine($"{indent}PARAM '{p.Item1}' : {p.Item2.GetType().Name} '{Format(p.Item2)}'");
                }
                indent += indentString;
            }
        }
        public static string ReadLine()
        {
            string userInput = Console.ReadLine();
            using (StreamWriter sw = new StreamWriter(name, true))
            {
                sw.WriteLine($"{indent}INPUT '{userInput}'");
            }
            return userInput;
        }
        public static void Write(object wrote)
        {
            Console.Write(wrote);
            using (StreamWriter sw = new StreamWriter(name, true))
            {
                sw.WriteLine($"{indent}OUTPUT '{wrote}'");
            }
        }
        public static void WriteLine()
        {
            WriteLine("");
        }
        public static void WriteLine(object wrote)
        {
            Console.WriteLine(wrote);
            using (StreamWriter sw = new StreamWriter(name, true))
            {
                sw.WriteLine($"{indent}OUTPUT '{wrote}\\n'");
            }
        }
        public static void Close()
        {
            indent = indent.Substring(indentString.Length);
        }
        public static void Return((string, object) returned)
        {
            indent = indent.Substring(indentString.Length);
            using (StreamWriter sw = new StreamWriter(name, true))
            {
                if (returned.Item1 == "n/a")
                {
                    sw.WriteLine($"{indent}RETURNED {returned.Item2.GetType().Name} '{Format(returned.Item2)}'");

                }
                else
                {
                    sw.WriteLine($"{indent}RETURNED '{returned.Item1}' : {returned.Item2.GetType().Name} '{Format(returned.Item2)}'");

                }
            }
        }
        public static void TraceTable()
        {
            string[] write = Table.GetTable();
            using (StreamWriter sw = new StreamWriter(name, true))
            {
                foreach (string s in write) { sw.WriteLine($"{indent} # {s} #"); }
            }
        }
    }
    internal class Program
    {
        static Random RGen = new Random();

        static void Main(string[] args)
        {
            try
            {
                Trace.Open("Main");
                List<int> NumbersAllowed = new List<int>();
                List<int> Targets;
                int MaxNumberOfTargets = 20;
                Trace.Table.Became("MaxNumberOfTargets", MaxNumberOfTargets);
                int MaxTarget;
                int MaxNumber;
                bool TrainingGame;
                Trace.Write("Enter y to play the training game, anything else to play a random game: ");
                string Choice = Trace.ReadLine().ToLower();
                Trace.Table.Became("Choice", Choice);
                Trace.WriteLine();
                if (Choice == "y")
                {
                    MaxNumber = 1000;
                    Trace.Table.Became("MaxNumber", MaxNumber);
                    MaxTarget = 1000;
                    Trace.Table.Became("MaxTarget", MaxTarget);
                    TrainingGame = true;
                    Trace.Table.Became("TrainingGame", TrainingGame);
                    Targets = new List<int> { -1, -1, -1, -1, -1, 23, 9, 140, 82, 121, 34, 45, 68, 75, 34, 23, 119, 43, 23, 119 };
                    Trace.Table.Became("Targets", Targets);
                }
                else
                {
                    MaxNumber = 10;
                    Trace.Table.Became("MaxNumber", MaxNumber);
                    MaxTarget = 50;
                    Trace.Table.Became("MaxTarget", MaxTarget);
                    TrainingGame = false;
                    Trace.Table.Became("TrainingGame", TrainingGame);
                    Targets = CreateTargets(MaxNumberOfTargets, MaxTarget);
                    Trace.Table.Became("Targets", Targets);
                }
                NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber);
                Trace.Table.Became("NumbersAllowed", NumbersAllowed);
                PlayGame(Targets, NumbersAllowed, TrainingGame, MaxTarget, MaxNumber);
                Trace.ReadLine();
                Trace.Close();
            }
            catch { }

            Trace.TraceTable();
        }

        static void PlayGame(List<int> Targets, List<int> NumbersAllowed, bool TrainingGame, int MaxTarget, int MaxNumber)
        {
            Trace.Open("PlayGame", ("Targets",Targets), ("NumbersAllowed", NumbersAllowed), ("TrainingGame", TrainingGame), ("MaxTarget", MaxTarget), ("MaxNumber", MaxNumber));
            int Score = 0;
            bool GameOver = false;
            string UserInput;
            List<string> UserInputInRPN;
            while (!GameOver)
            {
                DisplayState(Targets, NumbersAllowed, Score);
                Trace.Write("Enter an expression: ");
                UserInput = Trace.ReadLine();
                Trace.WriteLine();
                if (CheckIfUserInputValid(UserInput))
                {
                    UserInputInRPN = ConvertToRPN(UserInput);
                    if (CheckNumbersUsedAreAllInNumbersAllowed(NumbersAllowed, UserInputInRPN, MaxNumber))
                    {
                        if (CheckIfUserInputEvaluationIsATarget(Targets, UserInputInRPN, ref Score))
                        {
                            RemoveNumbersUsed(UserInput, MaxNumber, NumbersAllowed);
                            NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber);
                        }
                    }
                }
                Score--;
                if (Targets[0] != -1)
                {
                    GameOver = true;
                }
                else
                {
                    UpdateTargets(Targets, TrainingGame, MaxTarget);
                }
            }
            Trace.WriteLine("Game over!");
            DisplayScore(Score);
            Trace.Close();
        }

        static bool CheckIfUserInputEvaluationIsATarget(List<int> Targets, List<string> UserInputInRPN, ref int Score)
        {
            Trace.Open("CheckIfUserInputEvaluationIsATarget", ("Targets", Targets), ("UserInputInRPN", UserInputInRPN), ("Score", Score));
            int UserInputEvaluation = EvaluateRPN(UserInputInRPN);
            bool UserInputEvaluationIsATarget = false;
            if (UserInputEvaluation != -1)
            {
                for (int Count = 0; Count < Targets.Count; Count++)
                {
                    if (Targets[Count] == UserInputEvaluation)
                    {
                        Score += 2;
                        Targets[Count] = -1;
                        UserInputEvaluationIsATarget = true;
                    }
                }
            }
            Trace.Return(("UserInputEvaluationIsATarget", UserInputEvaluationIsATarget));
            return UserInputEvaluationIsATarget;
        }

        static void RemoveNumbersUsed(string UserInput, int MaxNumber, List<int> NumbersAllowed)
        {
            Trace.Open("RemoveNumbersUsed", ("UserInput", UserInput), ("MaxNumber", MaxNumber), ("NumbersAllowed", NumbersAllowed));
            List<string> UserInputInRPN = ConvertToRPN(UserInput);
            foreach (string Item in UserInputInRPN)
            {
                if (CheckValidNumber(Item, MaxNumber))
                {
                    if (NumbersAllowed.Contains(Convert.ToInt32(Item)))
                    {
                        NumbersAllowed.Remove(Convert.ToInt32(Item));
                    }
                }
            }
            Trace.Close();
        }

        static void UpdateTargets(List<int> Targets, bool TrainingGame, int MaxTarget)
        {
            Trace.Open("UpdateTargets", ("Targets", Targets), ("TrainingGame", TrainingGame), ("MaxTarget", MaxTarget));
            for (int Count = 0; Count < Targets.Count - 1; Count++)
            {
                Targets[Count] = Targets[Count + 1];
            }
            Targets.RemoveAt(Targets.Count - 1);
            if (TrainingGame)
            {
                Targets.Add(Targets[Targets.Count - 1]);
            }
            else
            {
                Targets.Add(GetTarget(MaxTarget));
            }
            Trace.Close();
        }

        static bool CheckNumbersUsedAreAllInNumbersAllowed(List<int> NumbersAllowed, List<string> UserInputInRPN, int MaxNumber)
        {
            Trace.Open("CheckNumbersUsedAreAllInNumbersAllowed", ("NumbersAllowed", NumbersAllowed), ("UserInputInRPN", UserInputInRPN), ("MaxNumber", MaxNumber));
            List<int> Temp = new List<int>();
            foreach (int Item in NumbersAllowed)
            {
                Temp.Add(Item);
            }
            foreach (string Item in UserInputInRPN)
            {
                if (CheckValidNumber(Item, MaxNumber))
                {
                    if (Temp.Contains(Convert.ToInt32(Item)))
                    {
                        Temp.Remove(Convert.ToInt32(Item));
                    }
                    else
                    {
                        Trace.Return(("n/a",false));
                        return false;
                    }
                }
            }
            Trace.Return(("n/a", true));
            return true;
        }

        static bool CheckValidNumber(string Item, int MaxNumber)
        {
            Trace.Open("CheckValidNumber", ("Item", Item), ("MaxNumber", MaxNumber));
            if (Regex.IsMatch(Item, "^[0-9]+$"))
            {
                int ItemAsInteger = Convert.ToInt32(Item);
                if (ItemAsInteger > 0 && ItemAsInteger <= MaxNumber)
                {
                    Trace.Return(("n/a", true));
                    return true;
                }
            }
            Trace.Return(("n/a", false));
            return false;
        }

        static void DisplayState(List<int> Targets, List<int> NumbersAllowed, int Score)
        {
            Trace.Open("DisplayState", ("Targets", Targets), ("NumbersAllowed", NumbersAllowed), ("Score", Score));
            DisplayTargets(Targets);
            DisplayNumbersAllowed(NumbersAllowed);
            DisplayScore(Score);
            Trace.Close();
        }

        static void DisplayScore(int Score)
        {
            Trace.Open("DisplayScore", ("Score", Score));
            Trace.WriteLine($"Current score: {Score}");
            Trace.WriteLine();
            Trace.WriteLine();
            Trace.Close();
        }

        static void DisplayNumbersAllowed(List<int> NumbersAllowed)
        {
            Trace.Open("DisplayNumbersAllowed", ("NumbersAllowed", NumbersAllowed));
            Trace.Write("Numbers available: ");
            foreach (int Number in NumbersAllowed)
            {
                Trace.Write($"{Number}  ");
            }
            Trace.WriteLine();
            Trace.WriteLine();
            Trace.Close();
        }

        static void DisplayTargets(List<int> Targets)
        {
            Trace.Open("DisplayTargets", ("Targets", Targets));
            Trace.Write("|");
            foreach (int T in Targets)
            {
                if (T == -1)
                {
                    Trace.Write(" ");
                }
                else
                {
                    Trace.Write(T);
                }
                Trace.Write("|");
            }
            Trace.WriteLine();
            Trace.WriteLine();
            Trace.Close();
        }

        static List<string> ConvertToRPN(string UserInput)
        {
            Trace.Open("ConvertToRPN", ("UserInput", UserInput));
            int Position = 0;
            Dictionary<string, int> Precedence = new Dictionary<string, int>
            {
                { "+", 2 }, { "-", 2 }, { "*", 4 }, { "/", 4 }
            };
            List<string> Operators = new List<string>();
            int Operand = GetNumberFromUserInput(UserInput, ref Position);
            List<string> UserInputInRPN = new List<string> { Operand.ToString() };
            Operators.Add(UserInput[Position - 1].ToString());
            while (Position < UserInput.Length)
            {
                Operand = GetNumberFromUserInput(UserInput, ref Position);
                UserInputInRPN.Add(Operand.ToString());
                if (Position < UserInput.Length)
                {
                    string CurrentOperator = UserInput[Position - 1].ToString();
                    while (Operators.Count > 0 && Precedence[Operators[Operators.Count - 1]] > Precedence[CurrentOperator])
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                    if (Operators.Count > 0 && Precedence[Operators[Operators.Count - 1]] == Precedence[CurrentOperator])
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                    Operators.Add(CurrentOperator);
                }
                else
                {
                    while (Operators.Count > 0)
                    {
                        UserInputInRPN.Add(Operators[Operators.Count - 1]);
                        Operators.RemoveAt(Operators.Count - 1);
                    }
                }
            }
            Trace.Return(("UserInputInRPN", UserInputInRPN));
            return UserInputInRPN;
        }

        static int EvaluateRPN(List<string> UserInputInRPN)
        {
            Trace.Open("EvaluateRPN", ("UserInputInRPN", UserInputInRPN));
            List<string> S = new List<string>();
            while (UserInputInRPN.Count > 0)
            {
                while (!"+-*/".Contains(UserInputInRPN[0]))
                {
                    S.Add(UserInputInRPN[0]);
                    UserInputInRPN.RemoveAt(0);
                }
                double Num2 = Convert.ToDouble(S[S.Count - 1]);
                S.RemoveAt(S.Count - 1);
                double Num1 = Convert.ToDouble(S[S.Count - 1]);
                S.RemoveAt(S.Count - 1);
                double Result = 0;
                switch (UserInputInRPN[0])
                {
                    case "+":
                        Result = Num1 + Num2;
                        break;
                    case "-":
                        Result = Num1 - Num2;
                        break;
                    case "*":
                        Result = Num1 * Num2;
                        break;
                    case "/":
                        Result = Num1 / Num2;
                        break;
                }
                UserInputInRPN.RemoveAt(0);
                S.Add(Convert.ToString(Result));
            }
            if (Convert.ToDouble(S[0]) - Math.Truncate(Convert.ToDouble(S[0])) == 0.0)
            {
                Trace.Return(("n/a", (int)Math.Truncate(Convert.ToDouble(S[0]))));
                return (int)Math.Truncate(Convert.ToDouble(S[0]));
            }
            else
            {
                Trace.Return(("n/a", -1));
                return -1;
            }
        }

        static int GetNumberFromUserInput(string UserInput, ref int Position)
        {
            Trace.Open("GetNumberFromUserInput", ("UserInput", UserInput), ("Position", Position));
            string Number = "";
            bool MoreDigits = true;
            while (MoreDigits)
            {
                if (Regex.IsMatch(UserInput[Position].ToString(), "[0-9]"))
                {
                    Number += UserInput[Position];
                }
                else
                {
                    MoreDigits = false;
                }
                Position++;
                if (Position == UserInput.Length)
                {
                    MoreDigits = false;
                }
            }
            if (Number == "")
            {
                Trace.Return(("n/a", -1));
                return -1;
            }
            else
            {
                Trace.Return(("n/a", Convert.ToInt32(Number)));
                return Convert.ToInt32(Number);
            }
        }

        static bool CheckIfUserInputValid(string UserInput)
        {
            Trace.Open("CheckIfUserInputValid", ("UserInput", UserInput));
            Trace.Return(("n/a",Regex.IsMatch(UserInput, @"^([0-9]+[\+\-\*\/])+[0-9]+$")));
            return Regex.IsMatch(UserInput, @"^([0-9]+[\+\-\*\/])+[0-9]+$");
        }

        static int GetTarget(int MaxTarget)
        {
            Trace.Open("GetTarget", ("MaxTarget", MaxTarget));
            int returned = RGen.Next(MaxTarget) + 1;
            Trace.Return(("n/a", returned));
            return returned;
        }

        static int GetNumber(int MaxNumber)
        {
            Trace.Open("GetNumber", ("MaxNumber", MaxNumber));
            int returned = RGen.Next(MaxNumber) + 1;
            Trace.Return(("n/a", returned));
            return returned;
        }

        static List<int> CreateTargets(int SizeOfTargets, int MaxTarget)
        {
            Trace.Open("CreateTargets", ("SizeOfTargets", SizeOfTargets), ("MaxTarget", MaxTarget));
            List<int> Targets = new List<int>();
            for (int Count = 1; Count <= 5; Count++)
            {
                Targets.Add(-1);
            }
            for (int Count = 1; Count <= SizeOfTargets - 5; Count++)
            {
                Targets.Add(GetTarget(MaxTarget));
            }
            Trace.Return(("Targets", Targets));
            return Targets;
        }

        static List<int> FillNumbers(List<int> NumbersAllowed, bool TrainingGame, int MaxNumber)
        {
            Trace.Open("FillNumbers", ("NumbersAllowed", NumbersAllowed), ("TrainingGame", TrainingGame), ("MaxNumber", MaxNumber));
            if (TrainingGame)
            {
                Trace.Return(("n/a", new List<int> { 2, 3, 2, 8, 512 }));
                return new List<int> { 2, 3, 2, 8, 512 };
            }
            else
            {
                while (NumbersAllowed.Count < 5)
                {
                    NumbersAllowed.Add(GetNumber(MaxNumber));
                }
                Trace.Return(("NumbersAllowed", NumbersAllowed));
                return NumbersAllowed;
            }
        }
    }
}