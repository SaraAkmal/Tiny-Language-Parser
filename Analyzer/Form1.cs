using Analyzer;
using dParser;
using Scanner;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;


namespace dParser
{

    public partial class Form1 : Form
    {


        Parser parser;
        Boolean clicked = false;
        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            this.treeView1.Dispose();

        }

        public void button1_Click(object sender, EventArgs e)
        {
            clicked = true;
            var str = richTextBox1.Text;

            Token scanner = new Token();
            scanner.getToken(str);
            richTextBox2.Text = scanner.slicer(scanner.GetText());

            parser = new Parser(scanner.getTokenQueue());
            parser.gettree(ref treeView1);


        }
        public void button3_Click(object sender, EventArgs e)
        {
            if (clicked == false)
            {
                var str = richTextBox1.Text;

                Token scanner = new Token();
                scanner.getToken(str);
                richTextBox2.Text = scanner.slicer(scanner.GetText());

                parser = new Parser(scanner.getTokenQueue());
                parser.gettree(ref treeView1);
            }

            parser.parse();
            this.treeView1.BringToFront();


        }


    }

}
class Parser //:dParser.Form1
{
    static int tokenNum = 0;
    Queue<TokenRec> tokenQueue = new Queue<TokenRec>();
    Queue<TokenRec> tokenArr = new Queue<TokenRec>();
    TreeNode currentNode;
    Stack<TreeNode> lastNode = new Stack<TreeNode>(100);
    Stack<int> tempId = new Stack<int>(100);
    static TokenRec[] token;
    Analyzer.Properties.Form2 myform = new Analyzer.Properties.Form2();
    Boolean FlagError = false;





    public Parser(Queue<TokenRec> tokenQueue)
    {
        this.tokenQueue = tokenQueue;



    }
    public void parse()
    {


        token = getArrayOfTokens(tokenQueue);
        Console.WriteLine(tokenQueue.Count);

        currentNode = test.Nodes.Add("Node" + tokenNum, "Start");
        runProgram();


    }

    public TokenRec runProgram()
    {
        TokenRec currentToken = tokenQueue.Dequeue();
        return stmtSequence(currentToken);
    }

    private TokenRec stmtSequence(TokenRec currentToken)
    {

        Console.WriteLine(currentToken.Token_Type);
        currentToken = statement(currentToken);


        while (tokenQueue.Count != 0 && currentToken.Token_Type == TokenRec.TokenType.SEMICOLON)
        {
            Match(TokenRec.TokenType.SEMICOLON, currentToken);
            currentToken = tokenQueue.Dequeue();


        }
        if (tokenQueue.Count != 0)
        {
            if (currentToken.Token_Type == TokenRec.TokenType.ELSE)
            {
                return currentToken;
            }
            else if (currentToken.Token_Type == TokenRec.TokenType.UNTIL)
            {
                Console.WriteLine(currentToken.getTokenValue());
                return currentToken;
            }
            else if (currentToken.Token_Type == TokenRec.TokenType.CONDITION_OP)
            {
                myform.addText("Invalid Token Found");
                myform.Show();
                return currentToken;
            }
            else
            {


                Console.WriteLine(currentToken.getTokenValue());
                return stmtSequence(currentToken);
            }
        }

        else
        {
            Console.WriteLine(currentToken.getTokenValue());
            return currentToken;
        }

    }
    TokenRec statement(TokenRec currentToken)
    {

        switch (currentToken.Token_Type)
        {
            case TokenRec.TokenType.IDENTIFIER:
                return assignStmt(currentToken);

            case TokenRec.TokenType.IF:
                return ifStmt(currentToken);

            case TokenRec.TokenType.READ:
                return readStmt(currentToken);

            case TokenRec.TokenType.REPEAT:
                return repeatStmt(currentToken);

            case TokenRec.TokenType.WRITE:
                return writeStmt(currentToken);


            default:
                FlagError = true;
                myform.addText("Invalid Token Found");
                myform.Show();
                return currentToken;
                //throw new SystemException("Invalid statement found.");

        }


    }

    private TokenRec readStmt(TokenRec currentToken)
    {

        lastNode.Push(currentNode);
        Match(TokenRec.TokenType.READ, currentToken);

        currentToken = tokenQueue.Dequeue();
        Console.WriteLine(currentToken.Token_Type);

        currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "read " + currentToken.getTokenValue());

        Match(TokenRec.TokenType.IDENTIFIER, currentToken);
        if (tokenQueue.Count == 0)
        {

            myform.addText("Error no semicolon");
            myform.Show();

        }
        else
        {
            currentToken = tokenQueue.Dequeue();
        }

        currentNode = lastNode.Pop();
        return currentToken;
    }


    private TokenRec writeStmt(TokenRec currentToken)
    {
        lastNode.Push(currentNode);
        currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "write");

        Match(TokenRec.TokenType.WRITE, currentToken);

        currentToken = tokenQueue.Dequeue();

        currentToken = exp(currentToken);
        currentNode = lastNode.Pop();

        return currentToken;
    }

    private TokenRec repeatStmt(TokenRec currentToken)
    {
        lastNode.Push(currentNode);
        currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "repeat");


        Match(TokenRec.TokenType.REPEAT, currentToken);
        currentToken = tokenQueue.Dequeue();
        currentToken = stmtSequence(currentToken);

        currentNode = lastNode.Pop();
        currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "until");

        Match(TokenRec.TokenType.UNTIL, currentToken);
        currentToken = tokenQueue.Dequeue();

        currentToken = exp(currentToken);

        currentNode = lastNode.Pop();

        return currentToken;
    }

    private TokenRec ifStmt(TokenRec currentToken)
    {
        lastNode.Push(currentNode);
        currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "if");

        Match(TokenRec.TokenType.IF, currentToken);
        currentToken = tokenQueue.Dequeue();

        currentToken = exp(currentToken);

        currentNode = lastNode.Pop();


        Match(TokenRec.TokenType.THEN, currentToken);
        currentToken = tokenQueue.Dequeue();
        currentToken = stmtSequence(currentToken);


        if (currentToken.Token_Type == TokenRec.TokenType.ELSE)
        {
            Match(TokenRec.TokenType.ELSE, currentToken);
            currentToken = stmtSequence(currentToken);
        }
        else if (currentToken.Token_Type == TokenRec.TokenType.END)
        {
            Match(TokenRec.TokenType.END, currentToken);
        }
        return currentToken;
    }


    public TokenRec assignStmt(TokenRec currentToken)
    {
        lastNode.Push(currentNode);
        currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "assign " + currentToken.getTokenValue());

        Match(TokenRec.TokenType.IDENTIFIER, currentToken);



        TokenRec nextToken = tokenQueue.Dequeue();

        Match(TokenRec.TokenType.ASSIGNMENT, nextToken);

        nextToken = tokenQueue.Dequeue();



        currentToken = exp(nextToken);
        currentNode = lastNode.Pop();


        return currentToken;


    }

    public TokenRec exp(TokenRec currentToken)
    {


        currentToken = simpleExp(currentToken);


        if (tokenQueue.Count != 0 && (currentToken.Token_Type == TokenRec.TokenType.BIGGER_THAN || currentToken.Token_Type == TokenRec.TokenType.LESS_THAN || currentToken.Token_Type == TokenRec.TokenType.EQUAL))
        {
            lastNode.Push(currentNode);
            currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "op " + currentToken.getTokenValue());

            Match(currentToken.Token_Type, currentToken);
            TokenRec nextToken = tokenQueue.Dequeue();


            currentToken = simpleExp(nextToken);


            if (currentToken.Token_Type != TokenRec.TokenType.LESS_THAN && currentToken.Token_Type != TokenRec.TokenType.BIGGER_THAN && (FlagError == false))
            {
                currentNode.Nodes.Add("Node" + (tokenNum + 1), token[tempId.Pop()].getTokenValue());
            }


            return currentToken;
        }
        else
            return currentToken;


    }

    private TokenRec simpleExp(TokenRec currentToken)
    {


        currentToken = term(currentToken);


        while (tokenQueue.Count != 0 && currentToken.Token_Type == TokenRec.TokenType.ADDOP)
        {

            lastNode.Push(currentNode);
            currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "op " + currentToken.getTokenValue());

            Console.WriteLine(currentToken.getTokenValue());
            Match(currentToken.Token_Type, currentToken);


            TokenRec nextToken = tokenQueue.Dequeue();

            currentToken = term(nextToken);
            currentNode.Nodes.Add("Node" + (tokenNum + 1), token[tempId.Pop()].getTokenValue());

        }

        if (tempId.Count != 0 && currentToken.Token_Type != TokenRec.TokenType.LESS_THAN && currentToken.Token_Type != TokenRec.TokenType.EQUAL && currentToken.Token_Type != TokenRec.TokenType.BIGGER_THAN)
        {
            currentNode.Nodes.Add("Node" + (tokenNum + 1), token[tempId.Pop()].getTokenValue());
        }

        return currentToken;
    }


    private TokenRec term(TokenRec currentToken)
    {
        currentToken = factor(currentToken);



        while (tokenQueue.Count != 0 && currentToken.Token_Type == TokenRec.TokenType.MULOP)
        {
            lastNode.Push(currentNode);
            currentNode = currentNode.Nodes.Add("Node" + (tokenNum + 1), "op " + currentToken.getTokenValue());

            Console.WriteLine(currentToken.getTokenValue());

            Match(currentToken.Token_Type, currentToken);

            TokenRec nextToken = tokenQueue.Dequeue();


            currentToken = factor(nextToken);
            currentNode.Nodes.Add("Node" + (tokenNum + 1), token[tempId.Pop()].getTokenValue());

        }


        return currentToken;

    }


    private TokenRec factor(TokenRec currentToken)
    {
        switch (currentToken.Token_Type)
        {
            case TokenRec.TokenType.LEFTBRACKET:


                lastNode.Push(currentNode);
                currentNode.Nodes.Add("Node" + (tokenNum + 1), "(");

                Match(TokenRec.TokenType.LEFTBRACKET, currentToken);
                Console.WriteLine(currentToken.getTokenValue());
                TokenRec nextToken = tokenQueue.Dequeue();
                nextToken = exp(nextToken);

                currentNode = lastNode.Pop();
                currentNode.Nodes.Add("Node" + (tokenNum + 1), ")");

                Match(TokenRec.TokenType.RIGHTBRACKET, nextToken);


                Console.WriteLine(currentToken.getTokenValue());


                if (tokenQueue.Count != 0)
                    return tokenQueue.Dequeue();
                else
                    return nextToken;


            case TokenRec.TokenType.NUM:

                tempId.Push(tokenNum);

                Match(TokenRec.TokenType.NUM, currentToken);


                Console.WriteLine(currentToken.getTokenValue());

                if (tokenQueue.Count != 0)
                    return tokenQueue.Dequeue();
                else
                    return currentToken;


            case TokenRec.TokenType.IDENTIFIER:
                tempId.Push(tokenNum);
                Match(TokenRec.TokenType.IDENTIFIER, currentToken);

                if (tokenQueue.Count != 0)
                    return tokenQueue.Dequeue();
                else
                    return currentToken;


            default:
                FlagError = true;
                myform.addText("Invalid Token Found");
                myform.Show();
                return currentToken;
                //throw new SystemException("Invalid statement found.");
        }





    }

    TokenRec[] getArrayOfTokens(Queue<TokenRec> TQueue)
    {
        TokenRec[] tokenArray = new TokenRec[TQueue.Count];

        int count = TQueue.Count;
        for (int i = 0; i < count; i++)
        {
            tokenArray[i] = TQueue.Dequeue();


        }
        for (int i = 0; i < count; i++)
        {
            TQueue.Enqueue(tokenArray[i]);


        }
        return tokenArray;
    }



    void Match(TokenRec.TokenType expectedToken, TokenRec currentToken)
    {
        if (currentToken.Token_Type == expectedToken)
        {
            tokenNum++;
            if (tokenNum < token.Length)
            {
                currentToken = token[tokenNum];
                if (currentToken.getTokenValue() == "{")
                {
                    tokenNum += 2;
                    currentToken = token[tokenNum];
                }
            }

        }
        else
        {
            FlagError = true;
            myform.addText("Error: undefined token");
            myform.Show();

        }

    }



    TreeView test;
    public void gettree(ref TreeView treeview)
    {
        test = treeview;
    }

}
