using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceControlledCalculator
{
    class MathematicalExpressionEvaluator
    {
        public static double EvaluateMathematicalExpression(string infixString)
        {
            if (infixString == "")
            {
                return 0;
            }
            else
            {
                double result = 0;
                try
                {
                    List<string> postfixString = InfixToPostfix(infixString);
                    result = EvaluatePostfix(postfixString);
                }
                catch (InvalidOperationException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine("Not valid mathematical expression");
                }
                return result;
            }
        }

        private static List<string> InfixToPostfix(string input)
        {
            //seperate by commas
            List<string> postfixString = new List<string>();
            List<string> parsedInputString = ParseMathematicalExpression(input);
            Stack<string> myStack = new Stack<string>();
            for (int i = 0; i < parsedInputString.Count; i++)
            {
                if (IsOperand(parsedInputString[i]))
                {
                    postfixString.Add(parsedInputString[i]);
                }
                else if (IsOperator(parsedInputString[i]))
                {
                    while (myStack.Count != 0 && !IsOpeningParanthesis(myStack.Peek()) && HasHigherPrecedence(myStack.Peek(), parsedInputString[i]))
                    {
                        postfixString.Add(myStack.Pop());
                    }
                    myStack.Push(parsedInputString[i]);
                }
                else if (parsedInputString[i] == "(")
                {
                    myStack.Push(parsedInputString[i]);
                }
                else if (parsedInputString[i] == ")")
                {
                    while (myStack.Count != 0 && !IsOpeningParanthesis(myStack.Peek()))
                    {
                        postfixString.Add(myStack.Pop());
                    }
                    myStack.Pop();
                }
            }
            while (myStack.Count != 0)
            {
                postfixString.Add(myStack.Pop());
            }
            return postfixString;
        }

        private static bool IsOperand(string op)
        {
            return (op != "+" && op != "-" && op != "*" && op != ":" && op != "(" && op != ")");
        }

        private static bool IsOperator(string op)
        {
            return (op == "+" || op == "-" || op == "*" || op == ":");
        }

        private static bool IsOpeningParanthesis(string op)
        {
            return (op == "(");
        }

        private static bool HasHigherPrecedence(string stackTop, string nextElementInList)
        {
            int precedenceLevelStackTop = GetPrecedenceLevel(stackTop);
            int precedenceLevelElementInList = GetPrecedenceLevel(nextElementInList);
            return (precedenceLevelStackTop <= precedenceLevelElementInList);
        }

        private static int GetPrecedenceLevel(string op)
        {
            int precedenceLevel;
            if (op == "(" || op == ")")
            {
                precedenceLevel = 1;
            }
            else if (op == "*" || op == ":")
            {
                precedenceLevel = 2;
            }
            else if (op == "+" || op == "-")
            {
                precedenceLevel = 3;
            }
            else
            {
                precedenceLevel = 4;
            }
            return precedenceLevel;
        }

        private static double EvaluatePostfix(List<string> expression)
        {
            Stack<string> evalStack = new Stack<string>();
            double result = 0;
            for (int i = 0; i < expression.Count; i++)
            {
                if (IsOperand(expression[i]))
                {
                    evalStack.Push(expression[i]);
                }
                else if (IsOperator(expression[i]))
                {
                    double op1 = double.Parse(evalStack.Pop());
                    double op2 = double.Parse(evalStack.Pop());
                    result = PerformOperation(expression[i], op1, op2);
                    evalStack.Push(result.ToString());
                }
            }
            return double.Parse(evalStack.Peek());
        }

        private static double PerformOperation(string oper, double op1, double op2)
        {
            double result = 0;
            switch (oper)
            {
                case "+":
                    result = op1 + op2;
                    break;
                case "-":
                    result = op2 - op1;
                    break;
                case "*":
                    result = op1 * op2;
                    break;
                case ":":
                    result = op2 / op1;
                    break;
                default:
                    break;
            }
            return result;
        }

        // returns each operator and operand as a list of objects
        private static List<string> ParseMathematicalExpression(string inputString)
        {
            string nextListEntry = "";
            List<string> parsedInputString = new List<string>();
            for (int i = 0; i < inputString.Length; i++)
            {
                nextListEntry = "";
                while (i < inputString.Length && inputString[i] != '+' && inputString[i] != '-' && inputString[i] != ':' && inputString[i] != '*' && inputString[i] != '(' && inputString[i] != ')')
                {
                    nextListEntry += inputString[i];
                    i++;
                }
                if (nextListEntry != "")
                {
                    parsedInputString.Add(nextListEntry);
                }
                if (i < inputString.Length)
                {
                    switch (inputString[i])
                    {
                        case '+':
                            nextListEntry = "+";
                            break;
                        case '-':
                            nextListEntry = "-";
                            break;
                        case ':':
                            nextListEntry = ":";
                            break;
                        case '*':
                            nextListEntry = "*";
                            break;
                        case '(':
                            nextListEntry = "(";
                            break;
                        case ')':
                            nextListEntry = ")";
                            break;
                        default:
                            break;
                    }
                    parsedInputString.Add(nextListEntry);
                }
            }
            return parsedInputString;
        }

    }
}
