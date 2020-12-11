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
        bool prev_op;
        double temp_value;
        ArrayList History;
        bool res_op;
        StringToFormula stf;
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
            prev_op = false; // Represents the previous operation.
            res_op = true;


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
                // Step 2.5: Check to see if button is a operation
                if (name.Equals("Operation"))
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

            if ( !((String)x.Content).Equals(".") || !textIn.Contains(".") ) {
                if (textIn.Equals("0") || prev_op || res_op)
                {
                    textIn = (String)x.Content;
                }

                else
                {
                    // Step 3: Add an additional digit to the textbox
                    textIn += (String)x.Content;
                }

            }

            prev_op = false;
            res_op = false;


            // Step 5: 
            container_num.Text = textIn;



            // TESTING ONLY (CHECKS VALUES)
            /*
            curr_val_box.Text    = current_Value.ToString();
            new_val_box.Text     =     new_value.ToString();
            new_val_str_box.Text =         new_value_string;
            temp_box.Text        =    temp_value.ToString();
            */
            

        }

        // When an operation is clicked modify the window
        private void Operations_Clicked(object sender, RoutedEventArgs e)
        {

            Button x = (Button)sender;

            // Text from user input
            String container_numText = container_num.Text;

            // Text from history window
            String history_winText   = History_win.Text;


            if (prev_op)
            {
                history_winText = history_winText.Substring(0, history_winText.Length - 1)+ (String)x.Content;

            }
            else
            {

                // Adds additional operations and values to the history container
                history_winText += container_numText + (String)x.Content;


                // Updates the history container
               

            }
            History_win.Text = history_winText;
            prev_op = true;
            
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
        private void Operations_Res_Clicked(object sender, RoutedEventArgs e)
        {


            // 
            container_num.Text = stf.Eval(History_win.Text+container_num.Text).ToString(); // Sets the calculator textbox to the temp value 
            History_win.Text = "";
            res_op = true;

            // CHANGE STATE VARIABLES 
            new_value = 0; // Sets the new value to 0.
            prev_op = false; // Previous operation is resultant
            new_value_string = temp_value.ToString(); // Sets the new value string to temp

            // TESTING 
            /*
            curr_val_box.Text     = current_Value.ToString();
            new_val_box.Text      =     new_value.ToString();
            temp_box.Text         =    temp_value.ToString();
            new_val_str_box.Text  =         new_value_string;
            */
        }

        private void Operations_clr_Clicked(object sender, RoutedEventArgs e)
        {
            // RESET ALL VALUES TO DEFAULT
           // current_Value = 0;
            //new_value = 0;
            //temp_value = 0;
            //new_value_string = "";
            //prev_op = "";
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

        private void Operation_Bin_mul_Click(object sender, RoutedEventArgs e)
        {

        }


        private void drawFunction(object sender, RoutedEventArgs e)
        {

        }


        // Useless function [DONT DELETE]
        private void nothing(object sender, RoutedEventArgs e)
        {

        }
    }


}
