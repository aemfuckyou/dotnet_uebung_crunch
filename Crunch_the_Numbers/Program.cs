// See https://aka.ms/new-console-template for more information
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;


namespace crunch_the_numbers
{
    class Calculator
    {
        private const string inputFormatErrorString = "Input string was not in a correct format.";
        private const string s1 = ">";
        public Dictionary<string, double?> ans = new Dictionary<string, double?>();

        public Dictionary<string, double?> Ans
        {
            get { return ans; }
            set { ans = value; }
            
        }

        private double? splitString(string input)
        {
            var line = input;
            double? result = null;

            var assignmentPattern = @"^([a-zA-Z]+)=(\-?\d+(?>\.\d+)?)$";
            var calculationsPattern = @"^((?>\[[a-zA-Z]+\]|\d+(?>\.\d+)?))(\-|\^|\+|\*|\/)((?>\[[a-zA-Z]+\]|\d+(?>\.\d+)?))$";

            var assMatch = Regex.Matches(line, assignmentPattern);
            var calcMatch = Regex.Matches(line, calculationsPattern);

            // neither assignment nor calculation
            if (assMatch.Count == 0 && calcMatch.Count == 0)
            {
                Console.WriteLine(inputFormatErrorString);
                return result;
            }

            // assignment
            if (assMatch.Count == 1)
            {
                var variableName = assMatch.Select(x => x.Groups[1].Value).First();
                var value = assMatch.Select(x => x.Groups[2].Value).First();

                Console.Out.WriteLine("Assigning value: " + value + " to variable: " + variableName);
                this.Ans[variableName] = Double.Parse(value, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));

                return result;
            }

            // calculation
            if (calcMatch.Count == 1)
            {
                var variablePattern = @"([a-zA-Z]+)";
                var valuePattern = @"(\-?\d+(?>\.\d+)?)";

                var firstOperandSubString = calcMatch.Select(x => x.Groups[1].Value).First();
                var operAtorSubString = calcMatch.Select(x => x.Groups[2].Value).First();
                var secondOperandSubString = calcMatch.Select(x => x.Groups[3].Value).First();

                double? firstOperand = 0.0;
                double? secondOperand = 0.0;

                if(Regex.IsMatch(firstOperandSubString, variablePattern))
                {
                    //first operand was variable; check dictionary
                    firstOperandSubString = firstOperandSubString.Trim ('[', ']');
                    if (Ans.ContainsKey(firstOperandSubString))
                    {
                        Ans.TryGetValue(firstOperandSubString, out firstOperand);
                    }
                    else
                    {
                        Console.Out.WriteLine("Unused Variable used");
                        return result;
                    }
                } 
                else
                {
                    //first operand was a number; parse to double
                    firstOperand = Double.Parse(firstOperandSubString, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));
                }

                if (Regex.IsMatch(secondOperandSubString, variablePattern))
                {
                    //second operand was variable; check dictionary
                    secondOperandSubString = secondOperandSubString.Trim(new char[2] { '[', ']' });
                    if (Ans.ContainsKey(secondOperandSubString))
                    {
                        Ans.TryGetValue(secondOperandSubString, out secondOperand);
                    }
                    else
                    {
                        Console.Out.WriteLine("Unused Variable used");
                        return result;
                    }
                }
                else
                {
                    //first operand was a number; parse to double
                    secondOperand = Double.Parse(secondOperandSubString, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));
                }

                result = calculate(new double[2] {(double)firstOperand, (double)secondOperand}, operAtorSubString);

                if (result != null)
                {
                    this.Ans["ans"] = result;
                }

                return result;

            }

            return result;
        }

        private static double? calculate(double[] numbers, string operAtor)
        {
            double? result = null;

            if (numbers.Length == 2 && operAtor.Length == 1)
            {

                switch (operAtor)
                {
                    case "+":
                        result = addition(numbers);
                        break;
                    case "-":
                        result = substraction(numbers);
                        break;
                    case "*":
                        result = multiplication(numbers);
                        break;
                    case "/":
                        if (numbers[1] == 0) break;
                        result = division(numbers);
                        break;
                    case "^":
                        result = power(numbers);
                        break;
                }
            }
            return result;
        }



       

        private static double addition(double[] numbers)
        {
            double operator1 = numbers[0];
            double operator2 = numbers[1];

            double result = operator1 + operator2;
            return result;
        }

        private static double substraction(double[] numbers)
        {
            double operator1 = numbers[0];
            double operator2 = numbers[1];

            double result = operator1 - operator2;
            return result;

        }

        private static double multiplication(double[] numbers)
        {
            double operator1 = numbers[0];
            double operator2 = numbers[1];

            double result = operator1 * operator2;
            return result;

        }

        private static double division(double[] numbers)
        {
            double operator1 = numbers[0];
            double operator2 = numbers[1];

            double result = operator1 / operator2;
            
            return result;

        }

        private static double power(double[] numbers)
        {
            double operator1 = numbers[0];
            double operator2 = numbers[1];

            double result = Math.Pow(operator1, operator2);
            
            return result;

        }

        private static void Main(string[] args)
        {
            Calculator calc = new Calculator();

            double[] numbers;
            char[] operands;
            double? tempAns = null;



            Console.WriteLine("Hello, this is the Crunch the numbers calculation service. Type /help for more infos");

            while (true)
            {

                Console.Write(s1);

                string input = Console.ReadLine();

                if (input.Length > 0 && input[0] == '/')
                {
                    switch (input)
                    {
                        case "/help":
                            Console.WriteLine("You can enter simple calculations like '1+2' and let them calculate it by pressing enter.\r You can only use one operand at a time. If you want" +
                                " to use negative numbers you need to store them in a variable first. (More of that further down)\r The operands you can use are +, -, *, / and ^. Furthermore" +
                                ", you can assign a variable a value\r\n by using a syntax like 'hello=15'. \r This would result in a variable called 'hello' with a value of 15. \r To simply" +
                                " use the variable within a calculation use [] brackets: '[hello]*5'.\r The result of the last calculation will alwas be available unter the variable 'ans'.\r" +
                                "You can display the variables with the command '/list'. You can clear them by using '/clear'.\r To stop the Crunch the Numbers calculation service, use '/stop'." +
                                " To display this help text at any time use '/help'");
                            break;
                        case "/list":
                            if (calc.Ans.Count > 0)
                            {
                                foreach (KeyValuePair<string, double?> kvp in calc.Ans)
                                {
                                    Console.WriteLine("  [" + kvp.Key + "] : " + kvp.Value);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No values stored yet");
                            }

                            break;
                        case "/clear":
                            calc.Ans.Clear();
                            Console.WriteLine("Variables Cleared");
                            break;
                        case "/stop":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine(inputFormatErrorString);
                            break;
                    }
                }
                else
                {
                    tempAns = calc.splitString(input);
                    if (tempAns != null)
                    {
                        Console.WriteLine(" =" + tempAns);
                    }
                }

            }
        }



    }
    
    

}



