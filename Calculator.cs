using System;
using System.Windows.Forms;
using System.Drawing;

namespace VoiceControlledCalculator
{
    public partial class Calculator : Form
    {
        #region Global variable declaration
        MathematicalExpressionBuilder myExpressionBuilder = null;

        Image noTalkingImage = Image.FromFile("C:\\Projects\\Visual Studio\\VoiceControlledCalculator\\no-talking.jpg");
        Image talkingImage = Image.FromFile("C:\\Projects\\Visual Studio\\VoiceControlledCalculator\\talking.jpg");

        #endregion

        public Calculator()
        {
            InitializeComponent();
            myExpressionBuilder = new MathematicalExpressionBuilder(this);
            btnMute.BackgroundImage = noTalkingImage;
            btnMute.BackgroundImageLayout = ImageLayout.Stretch;
        }

        #region Buttons
        
        private void myBtn(string newInput)
        {
            myExpressionBuilder.SetCurrentExpression(myExpressionBuilder.GetCurrentExpression() + newInput);
            inputBox.Text = myExpressionBuilder.GetCurrentExpression();
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
            myExpressionBuilder.ResetAllData();
            inputBox.Text = myExpressionBuilder.GetCurrentExpression();
            resultBox.Text = "";
            headingLabel.Focus();
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            myExpressionBuilder.ResetAllData();
            inputBox.Text = myExpressionBuilder.GetCurrentExpression();
            resultBox.Text = "";
            myExpressionBuilder.ResetMathematicalExpressionBuilderData();
            if (myExpressionBuilder.IsSpeechRecognitionOn())
            {
                btnMute.BackgroundImage = noTalkingImage;
                btnMute.BackgroundImageLayout = ImageLayout.Stretch;
                myExpressionBuilder.StopSpeechRecognition();
                EnableButtons();
            }
            else
            {
                btnMute.BackgroundImage = talkingImage;
                btnMute.BackgroundImageLayout = ImageLayout.Stretch;
                myExpressionBuilder.StartSpeechRecognition();
                DisableButtons();
            }
            headingLabel.Focus();
        }

        private void btnDecimalPoint_Click(object sender, EventArgs e)
        {
            myBtn(btnDecimalPoint.Text);
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            resultBox.Text = MathematicalExpressionEvaluator.EvaluateMathematicalExpression(inputBox.Text).ToString();
            headingLabel.Focus();
        }

        #endregion
        
        private bool IsOperand(string op)
        {
            return (op != "+" && op != "-" && op != "*" && op != ":" && op != "(" && op != ")");
        }

        private bool IsOperator(string op)
        {
            return (op == "+" || op == "-" || op == "*" || op == ":");
        }

        public void setInputBoxText(string newInput)
        {
            inputBox.Text = newInput;
        }

        public void extendInputBoxText(string newInput)
        {
            inputBox.Text += newInput;
        }

        public void ExternalButton_Click(string btnName)
        {
            switch (btnName)
            {
                case "equals":
                    btnEquals_Click(btnEquals, EventArgs.Empty);
                    break;
                case "clear":
                    btnDeleteAll_Click(btnDeleteAll, EventArgs.Empty);
                    break;
                case "mute":
                    btnMute_Click(btnMute, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }

        private void EnableButtons()
        {
            btn0.Enabled = true;
            btn1.Enabled = true;
            btn2.Enabled = true;
            btn3.Enabled = true;
            btn4.Enabled = true;
            btn5.Enabled = true;
            btn6.Enabled = true;
            btn7.Enabled = true;
            btn8.Enabled = true;
            btn9.Enabled = true;
            btnBracketOpen.Enabled = true;
            btnBracketClose.Enabled = true;
            btnPlus.Enabled = true;
            btnMinus.Enabled = true;
            btnMultiply.Enabled = true;
            btnDivide.Enabled = true;
            btnDecimalPoint.Enabled = true;
            btnDeleteAll.Enabled = true;
            btnEquals.Enabled = true;
        }

        private void DisableButtons()
        {
            btn0.Enabled = false;
            btn1.Enabled = false;
            btn2.Enabled = false;
            btn3.Enabled = false;
            btn4.Enabled = false;
            btn5.Enabled = false;
            btn6.Enabled = false;
            btn7.Enabled = false;
            btn8.Enabled = false;
            btn9.Enabled = false;
            btnBracketOpen.Enabled = false;
            btnBracketClose.Enabled = false;
            btnPlus.Enabled = false;
            btnMinus.Enabled = false;
            btnMultiply.Enabled = false;
            btnDivide.Enabled = false;
            btnDecimalPoint.Enabled = false;
            btnDeleteAll.Enabled = false;
            btnEquals.Enabled = false;
        }
    }
}
