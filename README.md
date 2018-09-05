# Voice Controlled Calculator
## Introduction
This calculator can be controlled like a normal calculator or by using your voice.  
  
![alt text](https://github.com/lulu98/voice-controlled-calculator/blob/master/thumbnail.PNG)
## Theory
To make a calculator work with my voice, I first had to create a normal calculator. Because humans use the infix notation for their mathematical expressions, a computer will not evaluate these expressions without some preprocessing. But computers understand the postfix notation, or at least there is a rather straight forward algorithm for that by using a stack.  
(Follow: https://www.youtube.com/watch?v=vq-nUF0G4fI)   
  
As soon as the expression is in postfix notation, we can evaluate the expression with yet another stack. After this step, you will have evaluated the original mathematical expression. Now you have a classic calculator.  
(Follow: https://www.youtube.com/watch?v=MeRb_1bddWg)   
  
To make the calculator work, I used the Speech Recognition library of Microsoft. Now you have to identify how numbers are spoken out in your language. I decided on using the English language because the numbers are at least spoken out in order (e.g. 32 - "thirty two" vs "Zwei und Dreißig"). This part actually has cost a lot of tweeking to achieve constant updating of the number in the display port when speaking out part of the whole number. Because you will not be able to store every number in RAM or a data type, you must put the different parts of a whole number smartly together to get the whole number.
## Summary
The following steps must be followed:  
1. Transform mathematical expression from infix to postfix
2. Evaluate postfix expression  
3. Add speaking rules  

This application can be extended with more features like calculating the root or exponents. But you would need to change the function that calculates infix to postfix.
