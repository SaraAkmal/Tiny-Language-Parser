using System;

namespace Scanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Token LexicalAnalyser = new Token();
            string x = "read(a,b,c) ; ; /*input an integer*/ if x > 0 && y < 6 then ";
            LexicalAnalyser.getToken(x);


        }
    }
}
