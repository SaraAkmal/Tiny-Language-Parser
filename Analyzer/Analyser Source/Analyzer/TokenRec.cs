using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyzer
{
    class TokenRec
    {
        public enum TokenType
        {
            DATATYPE, IF, THEN, ELSE, END, REPEAT, UNTIL, READ, WRITE, ADDOP, MULOP, EQUAL, LEFTBRACES, RIGHTBRACES, QUOTATIONS, STRING, IDENTIFIER, RETURN,
            BOOLEAN_OP, CONDITION_OP, LEFTBRACKET, RIGHTBRACKET, SEMICOLON, ASSIGNMENT, NUM, ERROR, COMA, STARTCOMMENT, ENDCOMMENT, LESS_THAN, BIGGER_THAN
        }



        public TokenType Token_Type;
        string Token_Value;

        public TokenRec(TokenType Token_Type, string Token_Value)
        {
            this.Token_Type = Token_Type;
            this.Token_Value = Token_Value;

        }

        public string getTokenValue()
        {
            return Token_Value;
        }


    }
}
