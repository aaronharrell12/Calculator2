using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections;
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Variables used in calculator
        double current_Value;
        double new_value;
        String new_value_string;
        String prev_op;
        double temp_value;
        ArrayList History;
        bool res_op;
        StringToFormula stf;
        String MonoString;
        String oldHist;
        int perCount;
        bool checkLeft;
        bool checkRight;

        const string binomial_operators = "+-/*^%";
        public MainWindow()
        {
            History = new ArrayList();
            stf = new StringToFormula();
            InitializeComponent();


            // Step 1: Initialize all variables.
            current_Value = 0; // Represents the current value being held. 
            temp_value = 0; // Represents a temporary value for new operations
            new_value = 0; // Represents the number being currently entered
            new_value_string = ""; // Represents the number being currently entered as a string (because digits)
            prev_op = ""; // Represents the previous operation.
            res_op = true;
            MonoString = "";
            oldHist="";
            perCount = 0;
            checkLeft = false;
            checkRight = false;
            // --------------- (Bind all buttons to a function based off of name)-------------------------------

            // Step 2:  Loop Through all buttons on the grid 
            for (int i = 0; i < grid.Children.Count; i++)
            {
                // Step 2.1: Grab the button
                Button ele = (Button)grid.Children[i];

                // Step 2.2: Grab that Buttons name and split it by '_' (example: "operation_mult" ==> ["operation","mult"])
                String[] name_arr = ele.Name.Split('_');

                // Step 2.3: Grab the first part 
                String name = name_arr[0];
                
                // Step 2.4 Check to see if button is a number
                if (name.Equals("Number"))
                {
                    ele.Click += new RoutedEventHandler(Numbers_Clicked);

                }
                else if (name.Equals("Constant"))
                {

                    ele.Click += new RoutedEventHandler(Constants_Clicked);
                }
                // Step 2.5: Check to see if button is a operation
                else if (name.Equals("Operation"))
                {

                    // Step 2.5.1: Grab the Second part
                    name = name_arr[1];

                    // Step 2.5.2: Check to see what type of operation:
                    switch (name)
                    {
                        // binary operations (+,-,/,*)
                        case "Bin":
                            ele.Click += new RoutedEventHandler(Operations_Clicked);
                            break;
                        case "Mon":
                            ele.Click += new RoutedEventHandler(Operations_mono_Clicked);
                            break;
                        case "Per":
                            ele.Click += new RoutedEventHandler(Operations_Per_Clicked);
                            break;
                        // Equal to operation (=)
                        case "Result":
                            ele.Click += new RoutedEventHandler(Operations_Res_Clicked);
                            break;
                        // Clear opeartion
                        case "clr":
                            ele.Click += new RoutedEventHandler(Operations_clr_Clicked);
                            break;

                    }

                }

                // Step 2.6: Update the grid button
                grid.Children[i] = ele;
            }

        }



        ///     Adds a digit to the number window and updates the temporary value (temp_value) based on:
        ///         - current_value (based on previous results)
        ///         - new_value     (based on what is being type in)
        private void Numbers_Clicked(object sender, RoutedEventArgs e)
        {

            



            // Step 1: Grab the button being sent
            Button x = (Button)sender;

            // Step 2: Grab the text in the calculator
            String textIn = container_num.Text;


            // First check for multiple decimals and also check 
            if ( !((String)x.Content).Equals(".") || !textIn.Contains(".") )
            {

                if (textIn.Equals("0") || prev_op.Equals("Res") || prev_op.Equals("Binom") || prev_op.Equals("Clr") || prev_op.Equals("Constant")||!MonoString.Equals(""))
                {

                    textIn = (String)x.Content;
                }

                else
                {
                    // Step 3: Add an additional digit to the textbox
                    textIn += (String)x.Content;
                }

            }

     
            res_op = false;
            prev_op = "num";

            // Step 5: 
            container_num.Text = textIn;


            MonoString = "";
            // TESTING ONLY (CHECKS VALUES)
            /*
            curr_val_box.Text    = current_Value.ToString();
            new_val_box.Text     =     new_value.ToString();
            new_val_str_box.Text =         new_value_string;
            temp_box.Text        =    temp_value.ToString();
            */
            

        }

        private void Constants_Clicked(object sender, RoutedEventArgs e)
        {

            Button x        = (Button)    sender;

            String constant = (String) x.Content;
            String num      =                 "";
            switch (constant)
            {
                case "e":
                    num = Math.E.ToString();
                    break;
                case "pi":
                    num = Math.PI.ToString();
                    break;
            }

            container_num.Text = num;

            prev_op = "Constant";
            MonoString = "";
        }

        private void Operations_mono_Clicked(object sender, RoutedEventArgs e)
        {
            Button x = (Button)sender;


            // Text from user input
            String container_numText = container_num.Text;

            String content = (String)x.Content;

            // Check to see if the mono string is empty
            if (MonoString.Equals(""))
            {   
                // Define the mono string to be [operation] ( [value] ) example sin(4)
                MonoString = content + "(" + container_num.Text + ")";
                oldHist    = History_win.Text;


                char lastInput;

                //  Grab the last input [Also check to see if history container is empty.]
                    if (oldHist.Length > 0)
                    {
                        lastInput = (oldHist)[(oldHist).Length - 1];
                    }
                    else
                    {
                        lastInput = ' ';
                    }
                if (lastInput == ')')
                {
                    oldHist = oldHist + "*";
                }

            }
            else
            {
                // Define the mono string to be [operation]([monostring]) example cos(sin(4))
                MonoString = content + "(" + MonoString + ")";
            }

    
            container_num.Text = (stf.Eval(MonoString)).ToString();
            History_win.Text = oldHist + MonoString;
            prev_op = "mono";
            checkLeft = false;


        }

        /// <summary>
        ///     This handles binomial operations (operations that take in two inputs)
        ///     
        ///     Examples: 
        ///             1+2
        ///             3*4
        ///             4%5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // When an binomial_operations is clicked modify the window
        private void Operations_Clicked(object sender, RoutedEventArgs e)
        {
            
            Button x = (Button)sender;

            // Button Opeartion
            String op = (String)x.Content;

            // Text from user input
            String container_numText = container_num.Text;

            // Text from history window
            String history_winText   = History_win.Text;

            // If no number has been input 
            if (prev_op.Equals("Binom"))
            {
                history_winText = history_winText.Substring(0, history_winText.Length - 1) + op ;
            }
            else
            {

                if (MonoString.Equals(""))
                {
                    // Adds additional operations and values to the history container
                    history_winText += container_numText + op;

                }
                else
                {
                    history_winText += op;
                    MonoString = "";
                }
                // Updates the history container
               

            }
            History_win.Text = history_winText;
            checkLeft = false;
            prev_op = "Binom";
            // CHANGE STATE VARIABLES
            //prev_op = (String)x.Content;
            //current_Value = temp_value;
            //new_value_string = "";
            //new_value = 0;
            //temp_value = 0;

            // TESTING
            /*
            curr_val_box.Text       = current_Value.ToString();
            new_val_box.Text        =     new_value.ToString();
            new_val_str_box.Text    =         new_value_string;
            temp_box.Text           =    temp_value.ToString();
            */
        }



        /// <summary>
        ///     Handles clicks on the parathesis buttons (left and right). 
        ///     
        ///             - Handles events where :
        ///                 - Right parathesis has been pressed without 
        ///            
        ///     Instructions:
        ///         When handling more edge cases. Please add to the switch statement to make the coding easier to read.
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operations_Per_Clicked(object sender, RoutedEventArgs e)
        {
            
            // Grab button 
            Button x = (Button)sender;
            // Grab button text
            String content_per =(String) x.Content;
            // Hold last input in history container
            char lastInput;      



            //  Grab the last input [Also check to see if history container is empty.]
                if (History_win.Text.Length > 0)
                {
                    lastInput = (History_win.Text)[(History_win.Text).Length - 1];
                }
                else
                {
                    lastInput = ' ';
                }


            // Seperate edge cases for left and right parathesis
            switch(content_per)
            {

                case "(":
                    perCount += 1;

                    // Case 1: Correct for the case where adding a left parathesis after a right parathesis add an implied multiplacation symbol. "18+sin(3)"---> "18+sin(3)*("
                    if (lastInput==')')
                    {
                        History_win.Text += "*(";
                    }

                    // Default
                    else
                    {
                        History_win.Text += "(";
                    }
                    break;

                case ")":
                    if (perCount > 0)
                    {
                        perCount -= 1;

                        // Case 1 : Avoids closing parathesis where the previous symbol isn't a number - Examples: "12+("---> "12+()"  or "12+(12+"---> "12+(12+)"
                        if (!Char.IsDigit(lastInput) && lastInput!=')')
                        {
                            History_win.Text += container_num.Text + ")";
                            container_num.Text = "0";
                        }

                        // Default
                        else
                        {
                            History_win.Text += ")";
                        }
                    }
                    break;
            }



            // CHANGE PROGRAM STATE VARIABLES 
            MonoString = "";
            prev_op = "Per";


        }

        /// <summary>
        ///     Handles the clicking of the results button (=).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operations_Res_Clicked(object sender, RoutedEventArgs e)
        {

           

            // Grabs the history container text
            String history_winText = History_win.Text;

            // Hold last input in history container
            char lastInput;



            //  Grab the last input [Also check to see if history container is empty.]
                if (History_win.Text.Length > 0)
                {
                    lastInput = (History_win.Text)[(History_win.Text).Length - 1];
                }
                else
                {
                    lastInput = ' ';
                }

            // Checks to see if there is a dangling binomial operator
                if (binomial_operators.IndexOf(lastInput)>=0)
                {
                    history_winText += container_num.Text;
                }

            // Closes any open parathesises 
                while (perCount > 0)
                {
                    history_winText += ")";
                    perCount--;
                }

            container_num.Text = stf.Eval(history_winText).ToString(); // Sets the calculator textbox to the temp value 
            History_win.Text = "";
            res_op = true;
            MonoString = "";
            // CHANGE STATE VARIABLES 
            new_value = 0; // Sets the new value to 0.
            new_value_string = temp_value.ToString(); // Sets the new value string to temp
            prev_op = "Res";
            // TESTING 
            /* curr_val_box.Text     = current_Value.ToString();
               new_val_box.Text      =     new_value.ToString();
               temp_box.Text         =    temp_value.ToString();
               new_val_str_box.Text  =         new_value_string;      */
        }

        private void Operations_clr_Clicked(object sender, RoutedEventArgs e)
        {
            checkLeft = false;
            res_op = true;
            perCount = 0;
            MonoString = "";
            prev_op = "Clr";
            container_num.Text = "0";
            History_win.Text = "";



            // TESTING
            /*
            curr_val_box.Text    = current_Value.ToString();
            new_val_box.Text     =     new_value.ToString();
            new_val_str_box.Text =         new_value_string;
            temp_box.Text        =    temp_value.ToString();
            */
        }

 


        // Useless function [DONT DELETE]
        private void nothing(object sender, RoutedEventArgs e)
        {

        }

        private void Operation_del_Click(object sender, RoutedEventArgs e)
        {
            // Hold last input in history container
            char lastIn;



            //  Grab the last input [Also check to see if history container is empty.]
            if (container_num.Text.Length > 1)
            {
                lastIn = (container_num.Text)[(container_num.Text).Length - 2];
            }
            else
            {
                lastIn = ' ';
            }

            if (prev_op.Equals("num"))
            {
                if(lastIn == ' ' || lastIn== '-' || lastIn=='0' )
                {
                    container_num.Text = "0";
                }
                else if (lastIn == '.')
                {
                    container_num.Text = (container_num.Text).Substring(0, (container_num.Text).Length - 2);
                }
                else
                {
                    container_num.Text = (container_num.Text).Substring(0, (container_num.Text).Length - 1);
                }
           
            }
        }

   
    }


}
