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
            double    current_Value;
            double        new_value;
            String new_value_string;
            String          prev_op;
            double       temp_value;
            ArrayList       History;

        public MainWindow()
        {
            History          = new ArrayList();
            InitializeComponent();


            // Step 1: Initialize all variables.
                current_Value    =  0; // Represents the current value being held. 
                temp_value       =  0; // Represents a temporary value for new operations
                new_value        =  0; // Represents the number being currently entered
                new_value_string = ""; // Represents the number being currently entered as a string (because digits)
                prev_op          = ""; // Represents the previous operation.


            // --------------- (Bind all buttons to a function based off of name)-------------------------------

                // Step 2:  Loop Through all buttons on the grid 
                for (int i =0; i< grid.Children.Count; i++)
                {
                        // Step 2.1: Grab the button
                            Button ele        = (Button) grid.Children[i];

                        // Step 2.2: Grab that Buttons name and split it by '_' (example: "operation_mult" ==> ["operation","mult"])
                            String[] name_arr = ele.Name.Split('_');

                        // Step 2.3: Grab the first part 
                            String    name    = name_arr[0];
    
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
                Button x             =          (Button) sender;

            // Step 2: Grab the text in the calculator
                String textIn        =       container_num.Text;

            // Step 3: Add an additional digit to the textbox
                textIn              +=        (String)x.Content;

            // Step 4: Add an additional digit to a the new_value string
                new_value_string    +=        (String)x.Content;

            // Step 5: 
                container_num.Text       =                   textIn;

            // Step 6: Check to see what the previous operation was 
                if (new_value_string.CompareTo("") != 0)
                {
                    // Step 6.1: Grab the current new value.
                        new_value        = Double.Parse(new_value_string);

                    // Step 6.2: Check the previous operation and compute the temporary result
                        switch (prev_op){

                            case "=":
                            case  "":
                                temp_value = new_value;
                                break;
                            case "+":
                                temp_value = new_value     + current_Value;
                                break;
                            case "-":
                                temp_value = current_Value -     new_value;
                                break;
                            case "*":
                                temp_value = current_Value *     new_value;
                                break;
                            case "/":
                                temp_value = current_Value /     new_value;
                                break;
                        }

                
                }


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

            String endPiece = container_numText.Substring(Math.Max(0, container_numText.Length - 1));

            if (int.TryParse(endPiece, out int empty))
            {
                container_numText =  "0";
            }
            else
            {
                container_numText = container_numText.Remove(container_numText.Length - 1);
            }

            container_numText      +=        (String)x.Content;
            container_num.Text      =        container_numText;

            // CHANGE STATE VARIABLES
            prev_op                 =        (String)x.Content;
            current_Value           =               temp_value;
            new_value_string        =                       "";
            new_value               =                        0;
            temp_value              =                        0;
             
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
            container_num.Text        =    temp_value.ToString(); // Sets the calculator textbox to the temp value 
            
            
            // CHANGE STATE VARIABLES 
            new_value             =                        0; // Sets the new value to 0.
            prev_op               =                      "="; // Previous operation is resultant
            new_value_string      =    temp_value.ToString(); // Sets the new value string to temp

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
            current_Value        =                        0;
            new_value            =                        0;
            temp_value           =                        0;
            new_value_string     =                       "";
            prev_op              =                       "";
            container_num.Text   =                      "0";


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
