using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Drawing;

namespace VoiceControlledCalculator
{
    public partial class Calculator : Form
    {
        #region Global variable declaration

        System.Globalization.CultureInfo myCulture = null;
        SpeechRecognitionEngine recEngine = null;
        private static Dictionary<string, long> numberTable = null;
        private double temporaryOperand = 0;
        private double processingNumber = 0;
        private double processingNumber2 = 0;
        private double processingNumber3 = 0;
        private double processingNumber4 = 0;
        private bool isDecimalNumber = false;
        private bool isSpeechRecognitionOn = false;

        private string currentExpression = "";
        private string nextExpression = "0";

        Image noTalkingImage = Image.FromFile("C:\\Projects\\Visual Studio\\VoiceControlledCalculator\\no-talking.jpg");
        Image talkingImage = Image.FromFile("C:\\Projects\\Visual Studio\\VoiceControlledCalculator\\talking.jpg");

        #endregion

        public Calculator()
        {
            InitializeComponent();
            myCulture = new System.Globalization.CultureInfo("en-US");
            recEngine = new SpeechRecognitionEngine(myCulture);
            InitializeNumberTable();
            prepareSpeachRecognition();
            btnMute.BackgroundImage = noTalkingImage;
            btnMute.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void InitializeNumberTable()
        {
            numberTable = new Dictionary<string, long> {{"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
                {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
                {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
                {"fourteen",14},{"fifteen",15},{"sixteen",16},
                {"seventeen",17},{"eightteen",18},{"nineteen",19},{"twenty",20},
                {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
                {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
                {"thousand",1000},{"million",1000000},{"billion",1000000000},
                {"trillion",1000000000000},{"quadrillion",1000000000000000},
                {"quintillion",1000000000000000000}};
        }

        private string[] constructTheCommands()
        {
            List<string> commands = new List<string>();
            foreach (KeyValuePair<string, long> entry in numberTable)
            {
                commands.Add(entry.Key);
            }
            commands.Add("point");
            commands.Add("open bracket");
            commands.Add("close bracket");
            commands.Add("clear");
            commands.Add("mute");
            commands.Add("plus");
            commands.Add("minus");
            commands.Add("times");
            commands.Add("divided by");
            commands.Add("equals");
            return commands.ToArray();
        }

        private void prepareSpeachRecognition()
        {
            Choices commands = new Choices();
            string[] actionCommands = constructTheCommands();

            commands.Add(actionCommands);
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            gBuilder.Culture = myCulture;
            Grammar grammar = new Grammar(gBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recEngine_SpeechRecognized);
            recEngine.SetInputToDefaultAudioDevice();
            //recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        void recEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            foreach (KeyValuePair<string, long> entry in numberTable)
            {
                if (entry.Key.Equals(e.Result.Text))
                {
                    double tempResult = 0;
                    if (isDecimalNumber)
                    {
                        inputBox.Text += entry.Value;
                        nextExpression += entry.Value;
                    }
                    else if (entry.Value < 10)
                    {
                        if (processingNumber4 == 0)
                        {
                            processingNumber4 = entry.Value;
                        }
                        processingNumber = entry.Value;
                        nextExpression = (double.Parse(nextExpression) + entry.Value).ToString();
                    }
                    else if (entry.Value >= 10 && entry.Value < 100)
                    {
                        processingNumber2 = entry.Value;
                        nextExpression = (double.Parse(nextExpression) + processingNumber2).ToString();
                    }
                    else if (entry.Value >= 100 && entry.Value < 1000)
                    {
                        processingNumber4 *= entry.Value;
                        processingNumber3 = processingNumber4;
                        if (((double.Parse(nextExpression) - processingNumber) % 1000) == 0)
                        {
                            nextExpression = ((double.Parse(nextExpression)-processingNumber) + processingNumber3).ToString();
                        }
                        else
                        {
                            nextExpression = processingNumber3.ToString();
                        }
                        processingNumber = 0;
                    }
                    else if (entry.Value >= 1000)
                    {
                        tempResult = (processingNumber + processingNumber2 + processingNumber3) * entry.Value;
                        temporaryOperand += tempResult;
                        nextExpression = temporaryOperand.ToString();
                        tempResult = 0;
                        processingNumber = 0;
                        processingNumber2 = 0;
                        processingNumber3 = 0;
                        processingNumber4 = 0;
                    }
                    inputBox.Text = currentExpression + nextExpression;
                }
            }
            switch (e.Result.Text)
            {
                case "plus":
                    resetExpressionCharacteristics("+");
                    break;
                case "minus":
                    resetExpressionCharacteristics("-");
                    break;
                case "times":
                    resetExpressionCharacteristics("*");
                    break;
                case "divided by":
                    resetExpressionCharacteristics(":");
                    break;
                case "open bracket":
                    resetExpressionCharacteristics("(");
                    break;
                case "close bracket":
                    resetExpressionCharacteristics(")");
                    break;
                case "point":
                    isDecimalNumber = true;
                    nextExpression += ",";
                    inputBox.Text = currentExpression + nextExpression;
                    break;
                case "equals":
                    resetExpressionData();
                    currentExpression = "";
                    nextExpression = "0";
                    btnEquals_Click(btnEquals, EventArgs.Empty);
                    break;
                case "clear":
                    resetExpressionData();
                    currentExpression = "";
                    nextExpression = "0";
                    btnDeleteAll_Click(btnDeleteAll, EventArgs.Empty);
                    break;
                case "mute":
                    btnMute_Click(btnMute, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }

        private void resetExpressionCharacteristics(string btnText)
        {
            //if (!nextExpression.Equals("0"))
            if ((currentExpression.Length == 0 && btnText != "(" && btnText != ")") || ((currentExpression.Length > 0 && (currentExpression[currentExpression.Length - 1].ToString() != "(") && (currentExpression[currentExpression.Length - 1].ToString() != ")") && IsOperator(btnText)) || !nextExpression.Equals("0")))
            {
                currentExpression += nextExpression;
                nextExpression = "0";
                inputBox.Text = currentExpression;
            }
            resetExpressionData();
            currentExpression += btnText;
            inputBox.Text = currentExpression;
        }

        private void resetExpressionData()
        {
            isDecimalNumber = false;
            temporaryOperand = 0;
            processingNumber = 0;
            processingNumber2 = 0;
            processingNumber3 = 0;
            processingNumber4 = 0;
        }

        #region Buttons
        
        private void myBtn(string newInput)
        {
            currentExpression += newInput;
            inputBox.Text = currentExpression;
            headingLabel.Focus();
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            myBtn(btn0.Text);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            myBtn(btn1.Text);
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            myBtn(btn2.Text);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            myBtn(btn3.Text);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            myBtn(btn4.Text);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            myBtn(btn5.Text);
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            myBtn(btn6.Text);
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            myBtn(btn7.Text);
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            myBtn(btn8.Text);
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            myBtn(btn9.Text);
        }

        private void btnBracketOpen_Click(object sender, EventArgs e)
        {
            myBtn(btnBracketOpen.Text);
        }

        private void btnBracketClose_Click(object sender, EventArgs e)
        {
            myBtn(btnBracketClose.Text);
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            myBtn(btnPlus.Text);
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            myBtn(btnMinus.Text);
        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            myBtn(btnDivide.Text);
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            myBtn(btnMultiply.Text);
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            currentExpression = "";
            inputBox.Text = currentExpression;
            resultBox.Text = "";
            headingLabel.Focus();
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            currentExpression = "";
            nextExpression = "0";
            inputBox.Text = currentExpression;
            resultBox.Text = "";
            resetExpressionData();
            if (isSpeechRecognitionOn)
            {
                btnMute.BackgroundImage = noTalkingImage;
                btnMute.BackgroundImageLayout = ImageLayout.Stretch;
                recEngine.RecognizeAsyncStop();
                isSpeechRecognitionOn = false;
            }
            else
            {
                btnMute.BackgroundImage = talkingImage;
                btnMute.BackgroundImageLayout = ImageLayout.Stretch;
                recEngine.RecognizeAsync(RecognizeMode.Multiple);
                isSpeechRecognitionOn = true;
            }
            headingLabel.Focus();
        }

        private void btnDecimalPoint_Click(object sender, EventArgs e)
        {
            myBtn(btnDecimalPoint.Text);
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            headingLabel.Focus();
            double result = 0;
            List<string> postfixString = InfixToPostfix(inputBox.Text);
            result = EvaluatePostfix(postfixString);
            resultBox.Text = result.ToString();
        }

        #endregion

        private List<string> InfixToPostfix(string input)
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
                    while (myStack.Count != 0 && !IsOpeningParanthesis(myStack.Peek()) && HasHigherPrecedence(myStack.Peek(),parsedInputString[i]))
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

        private bool IsOperand(string op)
        {
            return (op != "+" && op != "-" && op != "*" && op != ":" && op != "(" && op != ")");
        }

        private bool IsOperator(string op)
        {
            return (op == "+" || op == "-" || op == "*" || op == ":");
        }
        
        private bool IsOpeningParanthesis(string op)
        {
            return (op == "(");
        }

        private bool HasHigherPrecedence(string stackTop, string nextElementInList)
        {
            int precedenceLevelStackTop = GetPrecedenceLevel(stackTop);
            int precedenceLevelElementInList = GetPrecedenceLevel(nextElementInList);
            return (precedenceLevelStackTop <= precedenceLevelElementInList);
        }

        private int GetPrecedenceLevel(string op)
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

        private double EvaluatePostfix(List<string> expression)
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

        private double PerformOperation(string oper, double op1, double op2)
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
        private List<string> ParseMathematicalExpression(string inputString)
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
