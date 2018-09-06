using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace VoiceControlledCalculator
{
    class MathematicalExpressionBuilder
    {
        Calculator calculator = null;
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
        
        public MathematicalExpressionBuilder(Calculator myCalc)
        {
            calculator = myCalc;
            myCulture = new System.Globalization.CultureInfo("en-US");
            recEngine = new SpeechRecognitionEngine(myCulture);
            InitializeNumberTable();
            prepareSpeachRecognition();
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
                        calculator.extendInputBoxText(entry.Value.ToString());
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
                            nextExpression = ((double.Parse(nextExpression) - processingNumber) + processingNumber3).ToString();
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
                    calculator.setInputBoxText(currentExpression + nextExpression);
                }
            }
            switch (e.Result.Text)
            {
                case "plus":
                    UpdateExpressionCharacteristics("+");
                    break;
                case "minus":
                    UpdateExpressionCharacteristics("-");
                    break;
                case "times":
                    UpdateExpressionCharacteristics("*");
                    break;
                case "divided by":
                    UpdateExpressionCharacteristics(":");
                    break;
                case "open bracket":
                    UpdateExpressionCharacteristics("(");
                    break;
                case "close bracket":
                    UpdateExpressionCharacteristics(")");
                    break;
                case "point":
                    isDecimalNumber = true;
                    nextExpression += ",";
                    calculator.setInputBoxText(currentExpression + nextExpression);
                    break;
                case "equals":
                    resetExpressionData();
                    currentExpression = "";
                    nextExpression = "0";
                    calculator.ExternalButton_Click("equals");
                    break;
                case "clear":
                    resetExpressionData();
                    currentExpression = "";
                    nextExpression = "0";
                    calculator.ExternalButton_Click("clear");
                    break;
                case "mute":
                    calculator.ExternalButton_Click("mute");
                    break;
                default:
                    break;
            }
        }

        private void UpdateExpressionCharacteristics(string btnText)
        {
            //if (!nextExpression.Equals("0"))
            if ((currentExpression.Length == 0 && btnText != "(" && btnText != ")") || ((currentExpression.Length > 0 && (currentExpression[currentExpression.Length - 1].ToString() != "(") && (currentExpression[currentExpression.Length - 1].ToString() != ")") && IsOperator(btnText)) || !nextExpression.Equals("0")))
            {
                currentExpression += nextExpression;
                nextExpression = "0";
                calculator.setInputBoxText(currentExpression);
            }
            resetExpressionData();
            currentExpression += btnText;
            calculator.setInputBoxText(currentExpression);
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

        private bool IsOperator(string op)
        {
            return (op == "+" || op == "-" || op == "*" || op == ":");
        }

        public void StartSpeechRecognition()
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
            isSpeechRecognitionOn = true;
        }

        public void StopSpeechRecognition()
        {
            recEngine.RecognizeAsyncStop();
            isSpeechRecognitionOn = false;
        }

        public void ResetMathematicalExpressionBuilderData()
        {
            resetExpressionData();
        }

        public string GetCurrentExpression()
        {
            return currentExpression;
        }

        public void SetCurrentExpression(string exp)
        {
            currentExpression = exp;
        }

        public void ResetAllData()
        {
            resetExpressionData();
            currentExpression = "";
            nextExpression = "0";
        }

        public bool IsSpeechRecognitionOn()
        {
            return isSpeechRecognitionOn;
        }
    }
}
