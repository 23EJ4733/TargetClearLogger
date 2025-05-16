//Skeleton Program code for the AQA A Level Paper 1 Summer 2025 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

// not tracing, but I can't be bothered to make a new repository

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetClearCS
{
    internal class Program
    {
        static Random RGen = new Random();

        enum Attack
        {
            Archers,
            Ballista
        }

        static void Main(string[] args)
        {
            List<int> NumbersAllowed = new List<int>();
            List<int> Targets;
            int StartingNumberOfTargets = 20;
            int HandSize = 5;
            int MaxTarget;
            int MaxNumber;
            bool TrainingGame;
            Console.Write("Enter y to play the training game, anything else to play a random game: ");
            string Choice = Console.ReadLine().ToLower();
            Console.WriteLine();
            if (Choice == "y")
            {
                MaxNumber = 1000;
                MaxTarget = 1000;
                TrainingGame = true;
                Targets = new List<int> { -1, -1, -1, -1, -1, 23, 9, 140, 82, 121, 34, 45, 68, 75, 34, 23, 119, 43, 23, 119 };
            }
            else
            {
                MaxNumber = 10;
                MaxTarget = 50;
                TrainingGame = false;
                Targets = CreateTargets(StartingNumberOfTargets, MaxTarget);
            }
            NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber, HandSize);
            PlayGame(Targets, NumbersAllowed, TrainingGame, MaxTarget, MaxNumber, HandSize);
            Console.ReadLine();
        }

        static void PlayGame(List<int> Targets, List<int> NumbersAllowed, bool TrainingGame, int MaxTarget, int MaxNumber, int armyCount)
        {
            int score = -1;
            int supplies = 0;
            int farmCount = 0;
            int wallCount = 0;
            Attack attackType = Attack.Archers;

            bool GameOver = false;
            string UserInput;
            List<string> UserInputInRPN;
            while (!GameOver)
            {
                score++;
                attackType = Attack.Archers;
                supplies += farmCount - Math.Max((armyCount - 5) * (armyCount - 4) / 2, 0);
                DisplayState(Targets, NumbersAllowed, supplies, farmCount, wallCount, attackType);
                void DoStore()
                {
                    while (true)
                    {
                        List<string> storeItems = new List<string>() { };
                        if (supplies >= 2 + farmCount && Targets[farmCount] == -1)
                        {
                            Store($"Place farmland?: -{2 + farmCount} supplies (type f to place)");
                            storeItems.Add("f");
                        }
                        if (supplies >= 5 * (armyCount - 1))
                        {
                            Store($"Grow army?: -{5 * (armyCount - 1)} supplies, -{armyCount - 4} supplies/turn (type a to grow)");
                            storeItems.Add("a");
                        }
                        if (supplies >= Math.Max(4 * wallCount - farmCount, 6) && wallCount < farmCount)
                        {
                            Store($"Build wall?: -{Math.Max(4 * wallCount - farmCount, 6)} supplies (type w to build)");
                            storeItems.Add("w");
                        }
                        if (attackType == Attack.Archers && supplies >= 4 + wallCount && wallCount != 0)
                        {
                            Store($"Arm ballista? (Hit only closest target this turn): -{4 + wallCount} supplies (type b to arm)");
                            storeItems.Add("b");
                        }
                        if (storeItems.Count == 0) return;
                        Store($"Enter anything else to continue from the store");

                        string answer = Console.ReadLine();
                        if (!storeItems.Contains(answer)) return;

                        switch (answer)
                        {
                            case "a":
                                supplies -= 5 * (armyCount - 1);
                                armyCount++;
                                NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber, armyCount);
                                MaxTarget += Math.Max(armyCount - 3, 2);
                                break;
                            case "f":
                                supplies -= 2 + farmCount;
                                farmCount++;
                                MaxTarget++;
                                break;
                            case "w":
                                supplies -= Math.Max(4 * wallCount - farmCount, 6);
                                wallCount++;
                                MaxTarget++;
                                break;
                            case "b":
                                supplies -= 4 + wallCount;
                                attackType = Attack.Ballista;
                                break;
                        }

                        DisplayState(Targets, NumbersAllowed, supplies, farmCount, wallCount, attackType);
                    }
                }
                DoStore();
                Console.Write("Enter an expression: ");
                UserInput = Console.ReadLine();
                Console.WriteLine();
                if (CheckIfUserInputValid(UserInput))
                {
                    UserInputInRPN = ConvertToRPN(UserInput);
                    if (CheckNumbersUsedAreAllInNumbersAllowed(NumbersAllowed, UserInputInRPN, MaxNumber))
                    {
                        int killsLast = CheckIfUserInputEvaluationIsATarget(Targets, UserInputInRPN, attackType, ref supplies);
                        if (killsLast != 0)
                        {
                            RemoveNumbersUsed(UserInput, MaxNumber, NumbersAllowed);
                            if (killsLast > 1)
                            {
                                Announce("So many dead! A plague is festering!");
                                if (armyCount > 2)
                                {
                                    armyCount--;
                                    Announce("Death in your army from the plague!");
                                }
                            }
                            NumbersAllowed = FillNumbers(NumbersAllowed, TrainingGame, MaxNumber, armyCount);
                        }
                    }
                }

                bool updateTargetsThisTurn = true;

                supplies--;
                if (farmCount != 0 && Targets[farmCount] != -1)
                {
                    if (farmCount != wallCount)
                    {
                        Announce($"The barbarians are burning the crops! -{(farmCount - wallCount) * 2} supplies");
                        supplies -= (farmCount - wallCount) * 2;
                        farmCount = wallCount;
                    }
                    else
                    {
                        Announce($"Horror! The barbarians have breached the walls!");
                        wallCount -= 1;
                        updateTargetsThisTurn = false;
                    }
                }

                if (Targets[0] != -1)
                {
                    Announce("The barbarians have overrun you!");
                    GameOver = true;
                }
                else if (supplies < 0)
                {
                    Announce("Your supplies have depleted!");
                    GameOver = true;
                }
                else if (updateTargetsThisTurn)
                {
                    UpdateTargets(Targets, TrainingGame, MaxTarget);
                }
            }
            Console.WriteLine("Game over!");
            Console.WriteLine($"You survived the siege for {score} days.");
        }

        static void Announce(string announcement)
        {
            ColourWriteLine(announcement, ConsoleColor.Yellow);
        }
        static void BattleReport(string report)
        {
            ColourWriteLine(report, ConsoleColor.Magenta);
        }
        static void Store(string storeString)
        {
            ColourWriteLine(storeString, ConsoleColor.Green);
        }
        static void ColourWrite(string input, ConsoleColor colour)
        {
            ColourWrite(input, colour, Console.BackgroundColor);
        }
        static void ColourWrite(string input, ConsoleColor fg, ConsoleColor bg)
        {
            ConsoleColor fore = Console.ForegroundColor;
            ConsoleColor back = Console.BackgroundColor;
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Write(input);
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
        }
        static void ColourWriteLine(string input, ConsoleColor colour)
        {
            ColourWrite($"{input}\n", colour);
        }
        static void ColourWriteLine(string input, ConsoleColor fg, ConsoleColor bg)
        {
            ColourWrite($"{input}\n", fg, bg);
        }

        static int CheckIfUserInputEvaluationIsATarget(List<int> Targets, List<string> UserInputInRPN, Attack attackType, ref int Score)
        {
            int UserInputEvaluation = EvaluateRPN(UserInputInRPN);
            int kills = 0;
            if (UserInputEvaluation != -1)
            {
                if (attackType == Attack.Archers)
                {
                    for (int Count = 0; Count < Targets.Count; Count++)
                    {
                        if (Targets[Count] == UserInputEvaluation)
                        {
                            Score += 2;
                            Targets[Count] = -1;
                            kills++;
                        }
                    }
                    if (kills == 0)
                    {
                        BattleReport("The archers have missed!");
                    }
                    else if (kills == 1)
                    {
                        BattleReport("The archers have hit their mark!");
                    }
                    else
                    {
                        BattleReport("The archers have hit their marks!");
                    }
                }
                else if (attackType == Attack.Ballista)
                {
                    for (int Count = 0; Count < Targets.Count; Count++)
                    {
                        if (Targets[Count] == UserInputEvaluation)
                        {
                            Score += 2;
                            Targets[Count] = -1;
                            kills++;
                            break;
                        }
                    }
                    if (kills == 0)
                    {
                        BattleReport("The ballista has missed!");
                    }
                    else
                    {
                        BattleReport("The ballista has hit it's mark!");
                    }
                }
            }
            return kills;
        }

        static void RemoveNumbersUsed(string UserInput, int MaxNumber, List<int> NumbersAllowed)
        {
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
        }

        static void UpdateTargets(List<int> Targets, bool TrainingGame, int MaxTarget)
        {
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
        }

        static bool CheckNumbersUsedAreAllInNumbersAllowed(List<int> NumbersAllowed, List<string> UserInputInRPN, int MaxNumber)
        {
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
                        return false;
                    }
                }
            }
            return true;
        }

        static bool CheckValidNumber(string Item, int MaxNumber)
        {
            if (Regex.IsMatch(Item, "^[0-9]+$"))
            {
                int ItemAsInteger = Convert.ToInt32(Item);
                if (ItemAsInteger > 0 && ItemAsInteger <= MaxNumber)
                {
                    return true;
                }
            }
            return false;
        }

        static void DisplayState(List<int> Targets, List<int> NumbersAllowed, int Score, int farmCount, int wallCount, Attack attack)
        {
            DisplayTargets(Targets, farmCount, wallCount, attack);
            Console.WriteLine();
            DisplayNumbersAllowed(NumbersAllowed);
            DisplayScore(Score);
            Console.WriteLine($"Currently attacking with {attack}");
        }

        static void DisplayScore(int Score)
        {
            Console.WriteLine($"Current supplies: {Score}");
            Console.WriteLine();
            Console.WriteLine();
        }

        static void DisplayNumbersAllowed(List<int> NumbersAllowed)
        {
            Console.Write("Numbers available: ");
            foreach (int Number in NumbersAllowed)
            {
                Console.Write($"{Number}  ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        static void DisplayTargets(List<int> Targets, int farmCount, int wallCount, Attack attack)
        {
            Console.Write("|");
            for (int i = 0; i < Targets.Count; i++)
            {
                if (Targets[i] == -1)
                {
                    if (i < wallCount)
                    {
                        if (attack == Attack.Ballista)
                        {
                            ColourWrite("}", ConsoleColor.DarkBlue, ConsoleColor.DarkGray);
                        }
                        else
                        {
                            ColourWrite("\u2591", ConsoleColor.Gray, ConsoleColor.DarkGray);
                        }
                    }
                    else if (i < farmCount)
                    {
                        ColourWrite("#", ConsoleColor.Yellow);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                else
                {
                    List<int> without = Targets.ToArray().ToList();
                    without.Remove(Targets[i]);
                    ConsoleColor fg = Console.ForegroundColor;
                    if (Array.IndexOf(without.ToArray(), Targets[i]) != -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(Targets[i]);
                    Console.ForegroundColor = fg;

                }
                Console.Write("|");
            }
            Console.WriteLine();
        }

        static List<string> ConvertToRPN(string UserInput)
        {
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
            return UserInputInRPN;
        }

        static int EvaluateRPN(List<string> UserInputInRPN)
        {
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
                return (int)Math.Truncate(Convert.ToDouble(S[0]));
            }
            else
            {
                return -1;
            }
        }

        static int GetNumberFromUserInput(string UserInput, ref int Position)
        {
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
                return -1;
            }
            else
            {
                return Convert.ToInt32(Number);
            }
        }

        static bool CheckIfUserInputValid(string UserInput)
        {
            return Regex.IsMatch(UserInput, @"^([0-9]+[\+\-\*\/])+[0-9]+$");
        }

        static int GetTarget(int MaxTarget)
        {
            return RGen.Next(MaxTarget - 50, MaxTarget) + 1;
        }

        static int GetNumber(int MaxNumber)
        {
            return RGen.Next(MaxNumber) + 1;
        }

        static List<int> CreateTargets(int SizeOfTargets, int MaxTarget)
        {
            List<int> Targets = new List<int>();
            for (int Count = 1; Count <= 5; Count++)
            {
                Targets.Add(-1);
            }
            for (int Count = 1; Count <= SizeOfTargets - 5; Count++)
            {
                int add = GetTarget(MaxTarget);
                if (Targets.Contains(add))
                {
                    Count--;
                }
                else
                {
                    Targets.Add(add);
                }
            }
            return Targets;
        }

        static List<int> FillNumbers(List<int> NumbersAllowed, bool TrainingGame, int MaxNumber, int HandSize)
        {
            if (TrainingGame)
            {
                return new List<int> { 2, 3, 2, 8, 512 };
            }
            else
            {
                while (NumbersAllowed.Count < HandSize)
                {
                    NumbersAllowed.Add(GetNumber(MaxNumber));
                }
                while (NumbersAllowed.Count > HandSize)
                {
                    NumbersAllowed.RemoveAt(RGen.Next(NumbersAllowed.Count));
                }
                return NumbersAllowed;
            }
        }
    }
}