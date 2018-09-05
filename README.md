# Voice Controlled Calculator
## Introduction
This calculator can be controlled like a normal calculator or by using your voice.  
  
![alt text](https://github.com/lulu98/voice-controlled-calculator/blob/master/thumbnail.PNG)
## Theory
To make a calculator work with my voice, I first had to create a normal calculator. Because humans use the prefix notation for their mathematical expressions, a computer will not evaluate these expression without some preprocessing. But computers understand the postfix notation, or at least there is a rather straight forward algorithm for that by using a stack.  
As soon as the expression is in postfix notation, we can evaluate the expression with yet another stack. After this step, you will have evaluated the original mathematical expression. Now you have a classic calculator.  
To make the calculator work, I used the Speech Recognition library of Microsoft. Now you have to identify how numbers are spoken out in your language. I decided on using the English language because the numbers are at least spoken out in order (e.g. 32 - "thirty two" vs "Zwei und Dreißig"). This part actually has cost a lot of tweeking if you want to achieve contstant updating of the number in the display port when speaking out part of the whole number.
## Summary
