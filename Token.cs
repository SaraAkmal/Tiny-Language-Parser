using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner
{
    class Token
    {
        enum states { START, COMMENT, NUM, IDENTIFIER, ASSIGN, FUNCTION, DONE };
        states state = states.START;

        // is digit 
        bool isDigit(char d) { return (d >= '0' && d <= '9'); }

        //is letter
        bool isAcceptable(char l)
        {
            bool value = false;
            if (l >= 'a' && l <= 'z')
                value = true;
            else if (l >= 'A' && l <= 'Z')
                value = true;
            else if (l >= '0' && l <= '9')
                value = true;
            else if (l == '_')
                value = true;

            return value;
        }

        //is symbol 
        bool isSymbol(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '=' || c == '<' || c == '>' || c == '(' || c == ',' || c == ')' || c == ';' || c == '&' || c == '|' || c == '{' || c == '}');
        }
        //is whiteSpace
        bool isSpace(char s) { return (s == ' ' || s == '\t' || s == '\n'); }

        //reserved words
        string[] Reserved_Keywords = {"int","float","string",
                                      "if", "then", "else","elseif",
                                      "end","endl", "return",
                                      "repeat", "until", "read", "write",  };




        private bool isConditionOperator(char c, char d)
        {
            bool value = false;

            if ((c == '=' && d == '=') || c == '<' || c == '>' || (c == '<' && d == '>'))
                value = true;
            return value;

        }

        private bool isBooleanOperator(char a, char b)
        {
            bool value = false;


            if ((a == '&' && b == '&') || (a == '|' && b == '|'))
                value = true;

            return value;

        }
        private bool isArithmaticOperator(char c)
        {
            bool value = false;


            if (c == '+' || c == '-' || c == '*' || c == '/')
                value = true;

            return value;

        }

        // private bool isFunctionCall(string text)

        int FunctionCallToken(string myToken, string txt, int i)
        {

            bool error = false;
            string functionCallstr = myToken;
            myToken = "";
            while (txt[i] == ' ') { i++; };
            if (txt[i] == '(')
            {
                functionCallstr += txt[i];
                Console.WriteLine('(' + ": T_Symbol");

                while (txt[i] != ')')
                {

                    i++;

                    if (i == txt.Length - 1)
                    {
                        break;
                    }
                    functionCallstr += txt[i];
                    myToken += txt[i];

                    if (txt[i] == ',')
                    {


                        Console.WriteLine(myToken.Remove(myToken.Length - 1) + ": T_Identifier");
                        Console.WriteLine(',' + ": T_Symbol");

                        myToken = "";
                    }


                    if (txt[i] == ';' || txt[i] == '\n')
                    {
                        Console.WriteLine("Error Function call not complete");
                        error = true;
                        break;
                    }



                }
                if (!error)
                {
                    Console.WriteLine(myToken.Remove(myToken.Length - 1) + ": T_Identifier");
                    Console.WriteLine(')' + ": T_Symbol");
                    Console.WriteLine(functionCallstr + ": T_FunctionCall");

                }
            }


            return i;
        }

        public void getToken(string txt)
        {
            txt += " ";
            string myToken = "";
            int i = 0;
            bool res_flag = false;
            string historyIdentifier = "";
            string historyExpression = "";
            bool expFlag = false;
            bool funFlag = false;
            states stateHist = states.START;

            while (state != states.DONE)
            {
                stateHist = states.START;
                switch (state)
                {
                    case states.START:
                        if (isSpace(txt[i]))
                        {
                            i++;
                            if (i == txt.Length) state = states.DONE;
                            else state = states.START;
                        }
                        else if (isDigit(txt[i]))
                        {
                            state = states.NUM;
                        }
                        else if (Char.IsLetter(txt[i]))
                        {
                            state = states.IDENTIFIER;
                        }
                        //assingment exp
                        else if (txt[i] == ':' && txt[i + 1] == '=')
                        {

                            state = states.ASSIGN;

                        }
                        //function call
                        else if (txt[i] == '(')
                        {
                            Console.WriteLine(txt[i] + ": T_Symbol");

                            if (stateHist == states.IDENTIFIER)
                                state = states.FUNCTION;
                            else
                                i++;

                        }
                        //comment
                        else if (txt[i] == '/' && txt[i + 1] == '*')
                        {
                            i = i + 2;
                            state = states.COMMENT;
                        }
                        else if (isSymbol(txt[i]))
                        {
                            if (isBooleanOperator(txt[i], txt[i + 1]))
                            {
                                Console.WriteLine(txt[i] + "" + txt[i + 1] + ": T_BooleanOperator");
                            }

                            else if (isConditionOperator(txt[i], txt[i + 1]))
                            {

                                Console.WriteLine(txt[i] + ": T_ConditionOperator");

                            }
                            else if (isArithmaticOperator(txt[i]))
                            {
                                if (expFlag == true)
                                { historyExpression += txt[i]; }
                                Console.WriteLine(txt[i] + ": T_ArithmaticOperator");
                            }

                            else if (txt[i] == ';')
                            {
                                Console.WriteLine(txt[i] + ": T_semicolon");
                                if (expFlag == true)
                                {
                                    Console.WriteLine(historyIdentifier + "" + historyExpression + ": T_Assignment_Statement");
                                    historyIdentifier = "";
                                    historyExpression = "";
                                    expFlag = false;
                                }
                                else if (funFlag == true)
                                {
                                    Console.WriteLine(historyIdentifier + historyExpression + ": T_Function_Call:");

                                    historyIdentifier = "";
                                    historyExpression = "";
                                    funFlag = false;
                                }


                            }
                            else if (txt[i] == '{' || txt[i] == '}')
                            {
                                Console.WriteLine(txt[i] + ": T_Bracket");
                                if (funFlag == true)
                                {

                                    Console.WriteLine(historyIdentifier + historyExpression + ": T_Function_Call:");

                                    historyIdentifier = "";
                                    historyExpression = "";
                                    funFlag = false;
                                }
                            }

                            else if (txt[i] == ')')
                            {
                                Console.WriteLine(txt[i] + ": T_Symbol");

                                if (funFlag == true)
                                {
                                    historyExpression += txt[i];

                                }
                            }


                            else if (expFlag == true)
                            {
                                Console.WriteLine(txt[i - 1] + "" + txt[i] + ": T_Symbol");
                            }

                            i++;
                            if (i == txt.Length) state = states.DONE;
                            else state = states.START;
                        }

                        break;

                    case states.ASSIGN:
                        expFlag = true;
                        historyExpression += txt[i];
                        historyExpression += txt[i + 1];
                        i++;
                        state = states.START;
                        break;

                    case states.FUNCTION:

                        funFlag = true;
                        historyExpression += txt[i];
                        i++;
                        state = states.START;
                        break;

                    case states.NUM:
                        string float_str = "";
                        while (isDigit(txt[i]))
                        {

                            myToken += txt[i];
                            i++;
                        }
                        if (txt[i] == '.')
                        {
                            float_str += txt[i];
                            i++;
                            while (isDigit(txt[i]))
                            {

                                float_str += txt[i];
                                i++;

                            }

                            Console.WriteLine(myToken + "" + float_str + ": T_Float");



                        }
                        else
                        {
                            //label1.Text += mytoken += " , number \n";
                            Console.WriteLine(myToken + ": T_Digit");

                        }
                        if (expFlag == true || funFlag == true)
                        { historyExpression += myToken; }

                        myToken = "";
                        float_str = "";
                        if (i == txt.Length) state = states.DONE;
                        else state = states.START;

                        break;

                    case states.IDENTIFIER:
                        stateHist = states.IDENTIFIER;
                        while (isAcceptable(txt[i]))
                        {
                            myToken += txt[i];
                            i++;
                        }
                        for (int count = 0; count < 8; count++)
                        {
                            if (Reserved_Keywords[count] == myToken) res_flag = true;
                        }
                        if (res_flag)
                            Console.WriteLine(myToken + ": T_KeyWord");

                        else
                            Console.WriteLine(myToken + ": T_Identifier");

                        if (expFlag == true || funFlag == true)
                        { historyExpression += myToken; }

                        //else label1.Text += mytoken += " , identifier \n";
                        if (funFlag != true)
                            historyIdentifier = myToken;

                        // i = FunctionCallToken(myToken, txt, i);


                        //label1.Text += mytoken + " , reserved word \n";



                        myToken = "";
                        res_flag = false;

                        if
                            (i == txt.Length) state = states.DONE;
                        else
                            state = states.START;

                        break;

                    case states.COMMENT:

                        while ((txt[i] != '*') || (txt[i + 1] != '/'))
                        {

                            myToken += txt[i];

                            i++;
                        }
                        i = i + 2;
                        Console.WriteLine(myToken + ": T_Comment");
                        myToken = "";
                        if (i == txt.Length)
                            state = states.DONE;

                        else
                            state = states.START;

                        break;


                }



            }








        }

    }
}
