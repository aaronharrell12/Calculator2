using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    ///     DESCRIPTION: The string to Formula class takes in a string and generates a result.
    ///     REFERENCE: https://stackoverflow.com/questions/21750824/how-to-convert-a-string-to-a-mathematical-expression-programmatically
    /// 
    /// 
    ///     ADDED MONOMIAL OPERATORS.
    ///         - Including parsing for character based inputs
    ///         - 
    /// </summary>
    public class StringToFormula
        {
       
            private string[] _operators      =          { "-", "+", "/", "*","%", "^"};
            private string[] _mono_operators = { "sin", "cos", "log","abs","ln","neg"};

            // Hold the binary Operations
            private Func<double, double, double>[] _operations = {
                (a1, a2) => a1 - a2,
                (a1, a2) => a1 + a2,
                (a1, a2) => a1 / a2,
                (a1, a2) => a1 * a2,
                (a1, a2) => a1 % a2,
                (a1, a2) => Math.Pow(a1, a2)
            };
            
            // Holds the Monomial Operations
            private Func<double, double>[] _mono_operations = {
                (a1)     =>   Math.Sin(a1),
                (a1)     =>   Math.Cos(a1), 
                (a1)     => Math.Log10(a1),
                (a1)     =>   Math.Abs(a1),
                (a1)     =>   Math.Log(a1),
                (a1)     =>           -a1
            };



        /// <summary>
        ///     DESCRIPTION: This is the main function to be executed and does the bulk of the work.
        ///     EXAMPLE:    
        ///                 INPUT: "12+3sin(3)"
        ///                 OUTPUT: 12.42336002 
        /// </summary>
        public double Eval(string expression)
            {
                
                List<string>  tokens            = getTokens(expression);//  ["(", "2","+","45",")"]
                Stack<double> operandStack      =   new Stack<double>();//  ["2","45"]
                Stack<string> operatorStack     =   new Stack<string>();//  ["+"]
                Stack<string> monooperatorStack =   new Stack<string>();// holds all monomial operators
                bool next                       =                 false;
                int tokenIndex                  =                     0;

                // Loops through tokens
                while (tokenIndex < tokens.Count)
                {
                    
                    string token = tokens[tokenIndex];
                    
                    //-------------------------- HANDLE SUBEXPRESSIONS -------------------------
                    if (token == "(")
                    {
                        string subExpr = getSubExpression(tokens, ref tokenIndex);
                        operandStack.Push(Eval(subExpr));
                        next = true;
                        continue;
                    }

                    if (token == ")") throw new ArgumentException("Mis-matched parentheses in expression");

                //-------------------------- COMPUTATION STEPS ------------------------------ 





                /*-------------------------- MONOMIAL OPERATIONS --------------------------------------
                    This is the bulk of the monomial calculations in this program. If given the tokens = ["sin","cos","tan","2"]
                    This program will STACK the monomial operations as follows:
                        |tan|
                        |---|
                        |cos|     
                        |---|
                        |sin|
                         ---
                    the program won't run evaluate the stack until "2" is obtained. In which case the flag <next> is set equal to true and then the stack will start evaluation
                    in a loop on the last double value added to the the operandStack. This process is maintained even if "2" is replaced with another expression.
                 */
                    if (next) // check flag
                    {
                        // Monomial operater stack is looped if non empty
                        while (monooperatorStack.Count > 0)
                        {
                            string mono_op = monooperatorStack.Pop();
                            double arg1    = operandStack.Pop();
                            operandStack.Push(_mono_operations[Array.IndexOf(_mono_operators, mono_op)](arg1)); 
                        }
                        // Set the next flag to false;
                        next = false;
                    }

                    // Adds token to mono stack if it is a monomial operator 
                    if (Array.IndexOf(_mono_operators, token) >= 0)
                    {

                            monooperatorStack.Push(token);
                            next = false;
                    }

                /*---------------------------- BINOMIAL OPERATION ------------------------------------
                    Binomial operators are tricky simply due to the fact that there is always an order of operations when applying them.
                    So for our code this is specified at the top by explicitly defining them in order:

                                private string[] _operators  =          { "-", "+", "/", "*","%", "^"};

                    That order isn't a coincidence. It is the order of precedence each operation has over the other. - being the lowest and ^ 
                    being the highest. When evaluating the binomial stack the program checks to see if the new operater token holds less 
                    precedence than the previous one. If that is the case then the entire stack is looped here is an example:

                                         "12+13/5-3"   ---|tokenize|--> ["12","+","13","/","5","-","3"]

                                                                binom stack         numeric stack
                                                                                         |13|
                                 ["/","5","-","3"]      --->        |+|                  |12|
                    At this point in the example we have enough inputs to evaluate the expression 12+13 but if we do we are going to 
                    get the wrong answer. This is because the next  operation "/" takes high precedences that "=" so we must first 
                    evaluate "12/5" before we use addition. So we must continue adding to the stack until we reach a scenario
                    where the next operation doesn't take precedence. This holds for any binary operation. 
                 */
                else if (Array.IndexOf(_operators, token) >= 0)
                    {
                        // Returns error if mono stack is non empty and binom operator is found ( Example : sin+).
                        if (monooperatorStack.Count > 0) throw new ArgumentException("Binary Operator following a Monomial operator");
   
                        // Loops through previous (binom) operator stack if operator found is lower on the order of operations than previous item.  
                        while (operatorStack.Count > 0 && Array.IndexOf(_operators, token) < Array.IndexOf(_operators, operatorStack.Peek()))
                        {
                            string op = operatorStack.Pop();
                            double arg2 = operandStack.Pop();
                            double arg1 = operandStack.Pop();
                            operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                        }

                        //
                        operatorStack.Push(token);
                        next = false;
                    }

                    //--------------------------- NUMERIC VALUES -------------------------------------
                    else
                    {
                        operandStack.Push(double.Parse(token));
                        next = true;
                    }


                   
                    tokenIndex += 1;
                }


            /*
            -------------------------- END OF TOKEN LIST --------------------------------
                When the last of the tokens has been scanned, clear the monomial stack 
                and binomial stack of any operators still in their perspective stacks.
             */


                // ------- CLEAR THE MONOMIAL -----
                if (next)
                {
                
                    while (monooperatorStack.Count > 0)
                    {
                        string mono_op = monooperatorStack.Pop();
                        double arg1 = operandStack.Pop();
                        operandStack.Push(_mono_operations[Array.IndexOf(_mono_operators, mono_op)](arg1));
                    }
                    next = false;
                }
                //------- CLEAR THE BINOMIAL ------
                while (operatorStack.Count > 0)
                    {
                        string op = operatorStack.Pop();
                        double arg2 = operandStack.Pop();
                        double arg1 = operandStack.Pop();
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                    }

             
             return operandStack.Pop(); // Return remain value.
            }





        /// <summary>
        ///     DESCRIPTION: This is a helper function that recursively helps deal with parenthesis. 
        ///                  Generally what it does it grabs all tokens between the two matching parenthesis 
        ///                  and produces a string from them.
        ///     EXAMPLE: 
        ///                     Input:  Tokens = ["(","1","+","5","*","(","2","+","3",")",")","+","4']
        ///            Desired Output:  subExpression = "("1+5*(2+3))"
        ///                     
        ///                     Process:               Loop| 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 |10 |11 |
        ///                             parenlevels 1:     | ( |   |   |   |   |   |   |   |   |   | ) |
        ///                                         2:     |   | 1 | + | 5 | * | ( |   |   |   | ) |   |
        ///                                         3:     |   |   |   |   |   |   | 2 | + | 3 |   |   |
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="index"> The index where the first parenthesis's starts</param>
        /// <returns></returns>
        private string getSubExpression(List<string> tokens, ref int index)
            {
                StringBuilder subExpr = new StringBuilder();
                int parenlevels = 1;
                index += 1;
                while (index < tokens.Count && parenlevels > 0)
                {
                    string token = tokens[index];
                    if (tokens[index] == "(")  parenlevels += 1;
                    if (tokens[index] == ")")  parenlevels -= 1;
                    if (parenlevels > 0)subExpr.Append(token);
                    index += 1;
                }

                if ((parenlevels > 0))  throw new ArgumentException("Mis-matched parentheses in expression");
                
                return subExpr.ToString();
            }

        /// <summary>
        ///     DESCRIPTION: This is a helper function that generates the takes a string as an expression an generates a list of tokens.
        ///     EXAMPLE:
        ///                 "-12+3sin(4)" -----> ["-12","3","*","sin","(","4",")"]
        ///     
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
            public List<string> getTokens(string expression)
            {
                string operators    =          "()^%*/+-"; // List of binomial operators
                List<string> tokens =  new List<string>(); // List of tokens 
                StringBuilder sb    = new StringBuilder(); // Used to store digits and letters of monomial operators
                bool wasNumber      =               false; // Used as a check 
                char last           =                 ' '; // Used as a check 
                int i               =                   0; // checks

                // Loop through string
                foreach (char c in expression.Replace(" ", string.Empty))
                {

                    // SPECIAL CASE: Handles the case for when '-' should be interpreted as a sign instead of a operator
                    if (!Char.IsDigit(last)&& last!=')' && c == '-') sb.Append(c);
                

                    // Checks to see if characters is a binomial operator
                    else if (operators.IndexOf(c) >= 0)
                        {

                        // Adds the number/monomial operator (sb) to the token list.
                        if ((sb.Length > 0))
                        {
                            // Add Token to string to token
                            tokens.Add(sb.ToString());
                            sb.Length = 0;
                        }
            

                        
                        tokens.Add(c.ToString());   // Adds Operator to the token list
                        wasNumber = false;          // Set flag for previous token type added.

                        }

                    // Finds letter and interprets it as the start of a monomial operator
                    else if (Char.IsLetter(c))
                    {

                        // SPECIAL CASE: Handles the case for the transition from numbers to characters
                        //      Example:3sin  --->  3 * sin
                        if (wasNumber)
                        {
                            tokens.Add(sb.ToString());
                            tokens.Add("*");
                            sb.Length = 0;
                            wasNumber = false;// Set flag for previous token type added.
                    }

                        sb.Append(Char.ToLower(c)); // Add letter to string.

                        if (Array.IndexOf(_mono_operators, sb.ToString()) >= 0) // checks to see if is a mono operator.
                        {
                            tokens.Add(sb.ToString());
                            sb.Length = 0;
                        }

                    }

                    // finds number
                    else
                    {
                        // SPECIAL CASE:  When transition from characters (assumed to be used to spell a mono operator) to digits 
                        if (sb.Length > 0 && last != '-' && !wasNumber)  throw new ArgumentException("Non Operator found in expression.");

                        
                        sb.Append(c);     // Adds digit to number String
                        wasNumber = true; // Set flag for previous token type added.
                }

                last = c; // Set the last character. 
                }

                
                if ((sb.Length > 0))  tokens.Add(sb.ToString());// Adds remaining digits as number

                return tokens;
            }
        }
    
}
