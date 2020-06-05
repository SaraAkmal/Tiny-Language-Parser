using Analyzer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Scanner
{
    class Token
    {

        Queue<TokenRec> tokenQueue = new Queue<TokenRec>();

        public Queue<TokenRec> getTokenQueue()
        {
            return tokenQueue;
        }

        string text = "";

        enum states { START, COMMENT, NUM, IDENTIFIER, ASSIGN, FUNCTION, DONE, STRING, IF_STATEMENT };
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
            return (c == '+' || c == '-' || c == '–' || c == '*' || c == '/' || c == '=' || c == '<' || c == '>' || c == '(' || c == ',' || c == ')' || c == ';' || c == '&' || c == '|' || c == '{' || c == '}');
        }
        //is whiteSpace
        bool isSpace(char s) { return (s == ' ' || s == '\t' || s == '\r'); } //|| s == '\n'

        //reserved words
        string[] Reserved_Keywords = {"int","float","string",
                                      "if", "then", "else","elseif",
                                      "end","endl", "return",
                                      "repeat", "until", "read", "write"};

        //DATA TYPES
        string[] data_types = { "int", "float", "string" };
        bool isDataType(String text)
        {
            if (text == "int" || text == "float" || text == "string" || text == "void" || text == "bool")
                return true;
            else
                return false;
        }


        private bool isConditionOperator(char c, char d)
        {
            bool value = false;

            if ((c == '=' && d == '=') || c == '<' || c == '>' || c == '=' || (c == '<' && d == '>'))
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


            if (c == '*' || c == '/' || c == '+' || c == '-' || c == '–')
                value = true;

            return value;
        }

        private bool ismulop(char c)
        {
            bool value = false;


            if (c == '*' || c == '/')
                value = true;

            return value;

        }

        private bool isaddop(char c)
        {
            bool value = false;


            if (c == '+' || c == '-' || c == '–')
                value = true;

            return value;

        }




        public void getToken(string txt)
        {

            txt += " ";
            string myToken = "";

            int i = 0;
            bool res_flag = false;

            bool stateHist = false;
            bool AssignmentState = false;


            state = states.START;
            try
            {

                while (state != states.DONE)
                {



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
                                if (res_flag)
                                {
                                    text += "Error Identifier cant start with number~";
                                    state = states.DONE;
                                }
                                else
                                    state = states.NUM;
                            }
                            else if (Char.IsLetter(txt[i]))
                            {
                                state = states.IDENTIFIER;
                            }
                            else if (txt[i] == '_')
                            {
                                state = states.IDENTIFIER;
                            }
                            //assingment exp
                            else if (txt[i] == ':' && txt[i + 1] == '=')
                            {

                                text += (txt[i] + "" + txt[i + 1] + ": T_Assignment_Operator~");
                                state = states.ASSIGN;
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.ASSIGNMENT, ":="));

                            }

                            else if (txt[i] == ',')
                            {
                                stateHist = false;
                                text += (txt[i] + ": T_Symbol~");
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.COMA, ","));


                                i++;

                            }
                            else if (txt[i] == '=')
                            {
                                stateHist = false;
                                text += (txt[i] + ": T_Symbol~");
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.EQUAL, "="));


                                i++;

                            }
                            else if (txt[i] == '{' || txt[i] == '}')
                            {
                                if (txt[i] == '{')
                                {
                                    i++;
                                    state = states.COMMENT;
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.STARTCOMMENT, "{"));
                                }


                                else
                                {
                                    i++;
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.ENDCOMMENT, "}"));
                                    stateHist = false;


                                    if (i == txt.Length)
                                        state = states.DONE;
                                    else
                                    {
                                        state = states.START;
                                    }
                                }

                                // text += (txt[i] + ": T_Braces~");
                            }

                            else if (txt[i] == '"')
                            {
                                //tokens.Add(txt[i]); PARSER
                                // myToken += txt[i];
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.QUOTATIONS, char.ToString(txt[i])));
                                state = states.STRING;
                                i++;
                            }

                            else if (isSymbol(txt[i]))
                            {



                                if (res_flag)
                                {
                                    text += "Error Identifier cant start with symbol~";
                                    state = states.DONE;
                                    return;
                                }

                                else if (isBooleanOperator(txt[i], txt[i + 1]))
                                {
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.BOOLEAN_OP, txt[i] + "" + txt[i + 1]));


                                    text += (txt[i] + "" + txt[i + 1] + ": T_BooleanOperator~");

                                    i++;
                                }

                                else if (isConditionOperator(txt[i], txt[i + 1]))
                                {

                                    if ((txt[i] == '<' && txt[i + 1] == '>'))
                                    {
                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.CONDITION_OP, txt[i] + "" + txt[i + 1]));

                                        text += (txt[i] + "" + txt[i + 1] + ": T_ConditionOperator~");
                                        i++;
                                    }
                                    else if (txt[i] == '<')
                                    {
                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.LESS_THAN, char.ToString(txt[i])));
                                        text += (txt[i] + ": T_ConditionOperator~");
                                    }
                                    else if (txt[i] == '>')
                                    {
                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.BIGGER_THAN, char.ToString(txt[i])));
                                        text += (txt[i] + ": T_ConditionOperator~");
                                    }
                                    else if (txt[i] == '=')
                                    {
                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.EQUAL, char.ToString(txt[i])));
                                        text += (txt[i] + ": T_ConditionOperator~");
                                    }
                                    else
                                    {

                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.CONDITION_OP, char.ToString(txt[i])));

                                        text += (txt[i] + ": T_ConditionOperator~");
                                    }




                                }

                                else if (isArithmaticOperator(txt[i]))
                                {

                                    if (ismulop(txt[i]))
                                    {
                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.MULOP, char.ToString(txt[i])));
                                    }
                                    else if (isaddop(txt[i]))
                                    {

                                        tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.ADDOP, char.ToString(txt[i])));
                                    }



                                    text += (txt[i] + ": T_ArithmaticOperator~");
                                }

                                else if (txt[i] == ';')
                                {
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.SEMICOLON, ";"));

                                    stateHist = false;
                                    text += (txt[i] + ": T_semicolon~");





                                }


                                else if (txt[i] == ')')
                                {
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.RIGHTBRACKET, ")"));

                                    text += (txt[i] + ": T_Symbol~");


                                }

                                //function call
                                else if (txt[i] == '(')
                                {
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.LEFTBRACKET, "("));

                                    text += (txt[i] + ": T_Symbol~");

                                    if (stateHist)
                                    {
                                        state = states.FUNCTION;

                                        break;
                                    }



                                }
                                else if (AssignmentState == true)
                                {
                                    text += (txt[i - 1] + "" + txt[i] + ": T_Symbol~");

                                }

                                else
                                {


                                    text += (txt[i] + ": T_Symbol~");

                                }
                                i++;
                                if (i == txt.Length)
                                    state = states.DONE;
                                else
                                {

                                    state = states.START;
                                }

                            }


                            //still not done but checking when there is a new line if there is a semicolon in last line for errors
                            else if (txt[i] == '\n') // because \n\r represents new line
                            {



                                if (i == txt.Length) state = states.DONE;
                                else
                                {
                                    i++;
                                    state = states.START;
                                }



                            }

                            else
                            {
                                text += (txt[i] + ": " + "invalid char~");
                                i++;
                                state = states.START;
                            }

                            break;

                        case states.STRING:
                            try
                            {
                                while (txt[i] != '"')
                                {
                                    myToken += txt[i];
                                    i++;
                                }
                            }
                            catch (Exception e)
                            {
                                text += e.Message;
                            }
                            //myToken += txt[i];


                            tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.STRING, myToken));

                            text += (myToken + ": T_String~");
                            tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.QUOTATIONS, myToken));
                            myToken = "";

                            i++;
                            if (i == txt.Length) state = states.DONE;
                            else state = states.START;

                            break;



                        case states.ASSIGN:
                            stateHist = false;
                            AssignmentState = true;
                            i = i + 2;
                            state = states.START;
                            break;

                        case states.FUNCTION:
                            stateHist = false;
                            i++;
                            state = states.START;
                            break;

                        case states.NUM:
                            stateHist = false;
                            string float_str = "";
                            try
                            {
                                while (isDigit(txt[i]))
                                {

                                    myToken += txt[i];
                                    i++;
                                }
                            }
                            catch (Exception e)
                            {
                                text += e.Message;
                            }
                            if (txt[i] == '.')
                            {
                                float_str += txt[i];
                                i++;
                                try
                                {
                                    while (isDigit(txt[i]))
                                    {

                                        float_str += txt[i];
                                        i++;

                                    }
                                }
                                catch (Exception e)
                                {
                                    text += e.Message;
                                }


                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.NUM, myToken));

                                text += (myToken + "" + float_str + ": T_Float~");



                            }
                            else
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.NUM, myToken));

                                text += (myToken + ": T_Digit~");

                            }




                            myToken = "";
                            float_str = "";
                            if (i == txt.Length) state = states.DONE;
                            else state = states.START;

                            break;

                        case states.IDENTIFIER:
                            res_flag = false;
                            try
                            {
                                while (isAcceptable(txt[i]))
                                {
                                    myToken += txt[i];
                                    i++; // the letter after the res word ---------------------------->
                                }
                            }
                            catch (Exception e)
                            {
                                text += e.Message;
                            }

                            for (int count = 0; count < 14; count++)
                            {
                                if (Reserved_Keywords[count] == myToken) res_flag = true;
                            }

                            for (int n = 0; n < data_types.Length; n++)
                            {
                                if (myToken.Equals(data_types[n]))
                                {
                                    tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.DATATYPE, myToken));
                                    text += (myToken + ": T_DataType~");
                                }
                            }
                            if (res_flag)
                            {



                                text += (myToken + ": T_KeyWord~");


                            }

                            else
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.IDENTIFIER, myToken));

                                text += (myToken + ": T_Identifier~");

                                stateHist = true;



                            }
                            if (myToken == "repeat")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.REPEAT, myToken));





                            }




                            if (myToken == "until")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.UNTIL, myToken));



                            }

                            if (myToken == "then")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.THEN, myToken));


                            }


                            if (myToken == "read")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.READ, myToken));

                            }

                            if (myToken == "end")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.END, myToken));

                            }

                            if (myToken == "else")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.ELSE, myToken));

                            }

                            else if (myToken == "write")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.WRITE, myToken));
                                res_flag = false;

                            }
                            else if (myToken == "if")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.IF, myToken));
                                res_flag = false;
                            }
                            else if (myToken == "return")
                            {
                                tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.RETURN, myToken));
                                res_flag = false;

                            }






                            myToken = "";


                            if
                                (i == txt.Length) state = states.DONE;
                            else
                                state = states.START;

                            break;

                        case states.COMMENT:
                            try
                            {
                                while ((txt[i] != '}'))
                                {

                                    myToken += txt[i];

                                    i++;
                                }
                            }

                            catch (Exception e)
                            {
                                text += e.Message;
                            }
                            //tokenQueue.Enqueue(new TokenRec(TokenRec.TokenType.STRING, myToken));

                            myToken = "";
                            if (i == txt.Length)
                                state = states.DONE;

                            else
                                state = states.START;

                            break;


                    }



                }
            }
            catch (Exception e)
            {
                text += e.Message;
            }


            foreach (TokenRec x in tokenQueue)
            {
                Console.WriteLine(x.Token_Type);
            }




        }

        public string slicer(string text)
        {
            string[] textarr = text.Split('~');
            text = "";
            foreach (var word in textarr)
            {

                text += word;
                text += "\n";
            }
            return text;
        }
        public string GetText()
        {
            return text;
        }


    }


}
