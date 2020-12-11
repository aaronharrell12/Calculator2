using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{

        public class StringToFormula
        {
            private string[] _operators = { "-", "+", "/", "*", "^", "%" };
            private string[] _mono_operators = { "sin", "cos", "log","abs","ln","pm"};
            private string[] _constants = { "e", "pi" };
            private Func<double, double, double>[] _operations = {
                (a1, a2) => a1 - a2,
                (a1, a2) => a1 + a2,
                (a1, a2) => a1 / a2,
                (a1, a2) => a1 * a2,
                (a1, a2) => Math.Pow(a1, a2)

            };
            private Func<double, double>[] _mono_operations = {
                (a1)     => Math.Sin(a1),
                (a1)     => Math.Cos(a1),
                (a1)     => Math.Log(a1),
                (a1)     => Math.Abs(a1),
                (a1)     => Math.Log(a1),
                (a1)     => -a1
            };

            public double Eval(string expression)
            {
                List<string>  tokens            = getTokens(expression);//  ["(", "2","+","45",")"]
                Stack<double> operandStack      =   new Stack<double>();//  ["2","45"]
                Stack<string> operatorStack     =   new Stack<string>();//  ["+"]
                Stack<string> monooperatorStack =   new Stack<string>();
                bool next = false;
                int tokenIndex = 0;

                while (tokenIndex < tokens.Count)
                {
                    string token = tokens[tokenIndex];
                    if (token == "(")
                    {
                        string subExpr = getSubExpression(tokens, ref tokenIndex);
                        operandStack.Push(Eval(subExpr));
                        next = true;
                        continue;
                    }
                    if (token == ")")
                    {
                        throw new ArgumentException("Mis-matched parentheses in expression");
                    }


                    //If this is an operator  
                    if (Array.IndexOf(_mono_operators, token) >= 0)
                    {

                        monooperatorStack.Push(token);

                    }
                    else if (Array.IndexOf(_operators, token) >= 0)
                    {
                        if (monooperatorStack.Count > 0)
                        {
                            throw new ArgumentException("Binary Operator following a Monomial operator");
                        }
                        while (operatorStack.Count > 0 && Array.IndexOf(_operators, token) < Array.IndexOf(_operators, operatorStack.Peek()))
                        {
                            string op = operatorStack.Pop();
                            double arg2 = operandStack.Pop();
                            double arg1 = operandStack.Pop();
                            operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                        }
                        operatorStack.Push(token);
                        next = false;
                    }
                    else
                    {
                        operandStack.Push(double.Parse(token));
                        next = true;
                    }


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
                    tokenIndex += 1;
                }



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

            while (operatorStack.Count > 0)
                {
                    string op = operatorStack.Pop();
                    double arg2 = operandStack.Pop();
                    double arg1 = operandStack.Pop();
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                }
                return operandStack.Pop();
            }

            private string getSubExpression(List<string> tokens, ref int index)
            {
                StringBuilder subExpr = new StringBuilder();
                int parenlevels = 1;
                index += 1;
                while (index < tokens.Count && parenlevels > 0)
                {
                    string token = tokens[index];
                    if (tokens[index] == "(")
                    {
                        parenlevels += 1;
                    }

                    if (tokens[index] == ")")
                    {
                        parenlevels -= 1;
                    }

                    if (parenlevels > 0)
                    {
                        subExpr.Append(token);
                    }

                    index += 1;
                }

                if ((parenlevels > 0))
                {
                    throw new ArgumentException("Mis-matched parentheses in expression");
                }
                return subExpr.ToString();
            }

            public List<string> getTokens(string expression)
            {
                string operators = "()^*/+-";
                List<string> tokens = new List<string>();
                StringBuilder sb = new StringBuilder();
                bool wasNumber = false;
   
                int i = 0;
                foreach (char c in expression.Replace(" ", string.Empty))
                {

                    // Finds operator
                    if (operators.IndexOf(c) >= 0)
                    {
                        // Checks to see if there are digits 
                        if ((sb.Length > 0))
                        {
                            tokens.Add(sb.ToString());
                            sb.Length = 0;
                        }

                        // Adds Operator
                        tokens.Add(c.ToString());

                        wasNumber = false;

                    }

                    // Finds Character
                    else if (Char.IsLetter(c))
                    {

                        // 3sin  --->  3 * sin
                        if (wasNumber)
                        {
                            tokens.Add(sb.ToString());
                            tokens.Add("*");
                            sb.Length = 0;
                            wasNumber = false;
                        }

                        sb.Append(Char.ToLower(c));

                        if (Array.IndexOf(_mono_operators, sb.ToString()) >= 0)
                        {
                            tokens.Add(sb.ToString());
                            sb.Length = 0;
                        }

                    }

                    // finds number
                    else
                    {
                        if (sb.Length > 0 && !wasNumber)
                        {
                            throw new ArgumentException("Non Operator found in expression.");
                        }

                        sb.Append(c);
                        wasNumber = true;
                    }
                }

                // Adds remaining digits as number
                if ((sb.Length > 0))
                {
                    tokens.Add(sb.ToString());
                }
                return tokens;
            }
        }
    
}
