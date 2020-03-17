using System;

namespace Scanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Token LexicalAnalyser = new Token();
            string x = "int main() {int val, counter; read val; counter:= 0;} /*input an integer*/ if x > 0 && y < 6 ";
            LexicalAnalyser.getToken(x);


        }
    }
}
