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
        String                   prev_op;
        String                MonoString;
        String                   oldHist;
        bool                      res_op;
        int                     perCount;
        StringToFormula              stf;
       
        public MainWindow()
        {
            
            InitializeComponent();


            // ------------------------------- Initialize all variables. ------------------------------------------

            stf                 =   new StringToFormula();              // Object Used to compute expressions 
            prev_op             =                      "";              // Represents the previous operation.
            res_op              =                    true;

            // State variables used for [Monomial Operator Button]
            MonoString          =                      "";              // Stores the entire Expression(String) of repeated [Monomial operators] 
            oldHist             =                      "";              // Stores the value of the history window before the first [Monomial Operator] is pressed

            // State Variables used for [Parenthesis Operator Button]
            perCount            =                       0;              // Counts the number of mismatched parenthesis



            // --------------- (Bind all buttons to a function based off of name)-------------------------------
            /*
             * 
                EXPLAINATION OF BINDING CODE
                ---------------------------------------------------------------------------------
                Naming conventions of buttons are (View XAML Code for all the names):
                    1. [Classification]_[Name]
                    2. [Classification]_[Type]_[Name]   

                The following code splits the buttons Name splits the data into the variable <calcButtonNameArr>
                            calcButtonNameArr = {"[Classifcation]", "[Type]","[Name]"}
                and binds the button based on its [Classification] and [Type]

                Examples: 
                         ---------------------
                        | Operation_Bin_plus  | ---->  Classification = Operation , Type = Bin , Name = plus 
                         ---------------------

                         ---------------------
                        |       Number_1      | ---->  Classification = Operation   and Name = 1
                         ---------------------
                ---------------------------------------------------------------------------------
             */


            for (int i = 0; i < grid.Children.Count; i++)
            {
   
                Button   calcButton        = (Button)  grid.Children[i]; // Grabs button from buttonGrid
                String[] calcButtonNameArr = calcButton.Name.Split('_'); // Splits the name
                String   calcButtonClfctin =       calcButtonNameArr[0]; // Grabs the button classification

                switch (calcButtonClfctin)
                {
                    case "Number":
                        calcButton.Click += new RoutedEventHandler(Numbers_Clicked);
                        break;
                    case "Constant":
                        calcButton.Click += new RoutedEventHandler(Constants_Clicked);
                        break;
                    case "Operation":
                        String calcButtonType = calcButtonNameArr[1];
                        switch (calcButtonType)
                        {
                            // binary operations (+,-,/,*)
                            case "Bin":
                                calcButton.Click += new RoutedEventHandler(Operations_Clicked);
                                break;

                            // Monomial Operations
                            case "Mon":
                                calcButton.Click += new RoutedEventHandler(Operations_mono_Clicked);
                                break;
                           
                            // Parenthesis
                            case "Per":
                                calcButton.Click += new RoutedEventHandler(Operations_Per_Clicked);
                                break;

                            // Equal to operation (=)
                            case "Result":
                                calcButton.Click += new RoutedEventHandler(Operations_Res_Clicked);
                                break;

                            // Clear opeartion
                            case "clr":
                                calcButton.Click += new RoutedEventHandler(Operations_clr_Clicked);
                                break;
                        }
                        break;
                }
          

                // Update grid button
                grid.Children[i] = calcButton;
            }

        }


        /// <summary>
        ///     Function 
        ///     DESCRIPTION:  Adds a digit/decimal to the number window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Numbers_Clicked(object sender, RoutedEventArgs e)
            {

           
                Button numberButton      =               (Button)sender; // Grab the button being sent
                String numberValue       = (String)numberButton.Content; // Grab the digit or decimal
                String textIn            =           container_num.Text; // Grab the text in the calculator


                // First check for multiple decimals and also check 
                if ( !(numberValue).Equals(".") || !textIn.Contains(".") )
                {
                    // Clears number container under a couple different scenarios
                    if (textIn.Equals("0") || prev_op.Equals("Res") || prev_op.Equals("Binom") || prev_op.Equals("Clr") || prev_op.Equals("Constant")||!MonoString.Equals(""))
                    {

                        container_num.Text = numberValue;
                    }

                    // Adds a digit (or decimal) to the input
                    else container_num.Text += numberValue;

                }

                // SETS STATE VARIABLES
                res_op              =  false;
                prev_op             =  "num";
            
                MonoString          =     "";
          
            

            }


        /// <summary>
        ///     DESCRIPTION: Handles constant inputs like 
        ///         - e 
        ///         - pi
        ///     and replaces them with the approriate math value in the number container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Constants_Clicked(object sender, RoutedEventArgs e)
            {

                Button constantButton        = (Button)                 sender; // Grabs grid button
                String constant              = (String) constantButton.Content; // Grabs text contained in button
                String num                   =                              ""; // Defines number to be added


                // Approriately chooses the correct value for <num>
                switch (constant)
                {
                    case "e":
                        num = Math.E.ToString();
                        break;
                    case "pi":
                        num = Math.PI.ToString();
                        break;
                }

                // Adds num to the containeer
                container_num.Text = num;



                // SET STATE VARIABLES
                prev_op    = "Constant";
                MonoString =         "";
            }



        /// <summary>
        ///   
        ///     DESCRIPTION: This is a handler for monomial button operators. 
        ///     
        ///     DESIGN:
        ///          These buttons will "STACK". Meaning that if the user pressed several monomial operators in a row they will stack on top of each other:
        ///            --------------------------------------------------------------------------------------------------------------------------------------
        ///            |                || State 0  |    |   State 1   |    |      State 2     |    |       State 3        |    |        State 4            |
        ///            | ---------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | User Presses   || No Press |--->|   [sin]     |--->|       [cos]      |--->|          [ln]        |--->|         [abs]             |
        ///            | Number Window  ||   "4"    |--->|  "0.06975"  |--->|     "0.999999"   |--->|        "-7.411"      |--->|        "7.411"            |
        ///            | History window ||  "12+"   |--->| "12+sin(4)" |--->|  "12+cos(sin(4))"|--->|  "12+ln(cos(sin(4)))"|--->| "12+abs(ln(cos(sin(4))))" |
        ///            -------------------------------------------------------------------------------------------------------------------------------------- 
        ///             
        ///          Updating the history window is done by storing two string:
        ///                         <OldHist>     - (stores original state of history window) 
        ///                         <MonoString>  - which store the states of 
        ///                         
        ///          To see how they work in action here is an example: 
        ///            --------------------------------------------------------------------------------------------------------------------------------------
        ///            |                || State 0  |    |   State 1   |    |      State 2     |    |       State 3        |    |        State 4            |
        ///            | ---------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | User Presses   || No Press |--->|   [sin]     |--->|       [cos]      |--->|          [ln]        |--->|         [abs]             |
        ///            | Number Window  ||   "4"    |--->|  "0.06975"  |--->|     "0.999999"   |--->|        "-7.411"      |--->|        "7.411"            |
        ///            | History window ||  "12+"   |--->| "12+sin(4)" |--->|  "12+cos(sin(4))"|--->|  "12+ln(cos(sin(4)))"|--->| "12+abs(ln(cos(sin(4))))" |
        ///            | ~~~~~~~~~~~~~~~||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ History window update variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|
        ///            | OldHist        ||   ""     |--->|   "12+"     |--->|      "12+"       |--->|       "12+"          |--->|        "12+"              |   
        ///            | MonoString     ||   ""     |--->|  "sin(4)"   |--->|   "cos(sin(4))"  |--->| "ln(cos(sin(4)))"    |--->| "abs(ln(cos(sin(4))))"    |
        ///            --------------------------------------------------------------------------------------------------------------------------------------
        ///   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Operations_mono_Clicked(object sender, RoutedEventArgs e)
            {
                Button monoOpButton      =               (Button)sender; // Grabs the button
                String container_numText =           container_num.Text; // Grabs the number container text
                String content           = (String)monoOpButton.Content; // Grabs the text from the button

            
                if (MonoString.Equals(""))
                {   
                
                    MonoString = content + "(" + container_num.Text + ")"; //  Set the Mono string to be "[Operator](<number_container_value>)" EXAMPLE : "sin(4)"
                    oldHist    =                         History_win.Text; //  Sets old hist to be the current state of the history container



                    // SPECIAL CASE: 
                    //      Problem:
                    //              Description: If history container's current state ends with a right parathesis 
                    //              Example    : 12+3*4+(4)  ---> 12+3*4+(4)sin(3)
                    //      Solution:
                    //              Description: Interpret the "next operation" as multiplication
                    //              Example    : 12+3*4+(4)  ---> 12+3*4+(4)*sin(3)
                

                    //  Grab the last character in history window
                        char lastHistChar;     
                        if (oldHist.Length > 0) lastHistChar = (oldHist)[(oldHist).Length - 1];
                        else lastHistChar = ' ';
                
                    // Check to see if ')'  is the last character in history window
                        if (lastHistChar == ')') oldHist += "*";
                }

                else MonoString = content + "(" + MonoString + ")"; // Define the mono string to be [operation]([monostring]) example cos(sin(4))




                // UPDATE WINDOWS 
                container_num.Text = (stf.Eval(MonoString)).ToString(); // Send the evaluated string to the number container
                History_win.Text   =              oldHist + MonoString; // Update history container



                // SET STATE VARIABLES
                prev_op = "mono";


            }

        /// <summary>
        ///     DESCRIPTION: This handles binomial operations (operations that take in two inputs)
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
            
                Button binomButton = (Button)sender;
                String op = (String)binomButton.Content;// Button Opeartion
                String container_numText = container_num.Text;// Text from user input
                String history_winText   = History_win.Text;// Text from history window


                // If previous operation was a binomial then interpret press as user wanting to change the previous operation.
                if (prev_op.Equals("Binom"))   history_winText = history_winText.Substring(0, history_winText.Length - 1) + op ;

                else
                {
                    // Add operation and container number to the history container
                    if (MonoString.Equals("")) history_winText += container_numText + op;

                    // MonoStrings selfadd there value to the container in real time. So no need to add container value. [Read more about how they work in monomial operator section]
                    else
                    {
                        history_winText += op;
                        MonoString = "";
                    }
                }


                // UPDATE WINDOWS
                History_win.Text = history_winText;

                // SET STATE VARIABLES
                prev_op = "Binom";
    ;

            }



        /// <summary>
        ///     DESCRIPTIONS: Handles clicks on the parathesis buttons (left and right). 
        ///     
        ///             - Handles events where :
        ///                 - Right parathesis has been pressed without 
        ///            
        ///     INSTRUCTIONS:
        ///         When handling more edge cases. Please add to the switch statement to make the coding easier to read.
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Operations_Per_Clicked(object sender, RoutedEventArgs e)
            {
            
            
                Button x           = (Button)sender;     // Grab button 
                String content_per = (String) x.Content; // Grab button text
                char lastInput;                          // Defines the last input in history container  



                //  Grab the last input [Also check to see if history container is empty.]
                if (History_win.Text.Length > 0) lastInput = (History_win.Text)[(History_win.Text).Length - 1];
                else                             lastInput = ' ';
                


                // Seperate edge cases for left and right parathesis
                switch(content_per)
                {

                    case "(":
                        perCount += 1;
                        if (lastInput==')') History_win.Text += "*(";   // Case 1: Correct for the case where adding a left parathesis after a right parathesis add an implied multiplacation symbol. "18+sin(3)"---> "18+sin(3)*("
                        else                History_win.Text +=  "(";   // Default
                        break;

                    case ")":
                        if (perCount > 0)
                        {
                            perCount -= 1;

                            // Case 1 : Avoids closing parathesis where the previous symbol isn't a number/')' - Examples: "12+("---> "12+()"  or "12+(12+"---> "12+(12+)"
                            if (!Char.IsDigit(lastInput) && lastInput!=')')
                            {
                                History_win.Text += container_num.Text + ")";
                                container_num.Text = "0";
                            }

                            // Default
                            else History_win.Text += ")";
 
                        }
                        break;
                }



                // CHANGE PROGRAM STATE VARIABLES 
                MonoString = "";
                prev_op = "Per";


            }




        /// <summary>
        ///     DESCRIPTION: Handles the clicking of the results button (=).
        ///     The powerhouse behind this operation is going to be the StringToExpression class which can evaluate expresions.    
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Operations_Res_Clicked(object sender, RoutedEventArgs e)
            {

                string binomial_operators = "+-/*^%"; // Sets the usual binomial operators used. 
                String history_winText = History_win.Text; // Grabs the history container text
                char lastInput; // Hold last input in history container



                //  Grab the last input [Also check to see if history container is empty.]
                    if (History_win.Text.Length > 0)lastInput = (History_win.Text)[(History_win.Text).Length - 1];
                    else lastInput = ' ';
         

                // Checks to see if there is a dangling binomial operator in this history container
                    if (binomial_operators.IndexOf(lastInput)>=0) history_winText += container_num.Text;
               

                // Closes any open parathesises 
                    while (perCount > 0)
                    {
                        history_winText += ")";
                        perCount--;
                    }
            


                // UPDATE WINDOWS 
                container_num.Text = stf.Eval(history_winText).ToString(); // Sets the calculator textbox to the temp value 
                History_win.Text = "";

                // CHANGE STATE VARIABLES 
                prev_op = "Res";
                res_op = true;
                MonoString = "";
            }






        /// <summary>
        ///   DESCRIPTION: Clear button operator. Resets Windows back to default and resets the state variables to default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Operations_clr_Clicked(object sender, RoutedEventArgs e)
            {


                // UPDATE WINDOWS
                container_num.Text = "0";
                History_win.Text = "";


                // CHANGE STATE VARIABLES
                res_op = true;
                perCount = 0;
                MonoString = "";
                prev_op = "Clr";

            }

 


        /// <summary>
        /// DESCRIPTION : This is a useless function that is used as a placeholder to bind the menu button. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void nothing(object sender, RoutedEventArgs e)
            {

            }




        /// <summary>
        /// DESCIPTION: Allows users to back track the inputs being entered in the number container, WHILST TYPING IN NUMBERS. If the number container contains a result from calculations it clears the container.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
            private void Operation_del_Click(object sender, RoutedEventArgs e)
            {
                // Hold last input in history container
                char lastIn;



                //  Grab the last input [Also check to see if history container is empty.]
                if (container_num.Text.Length > 1) lastIn = (container_num.Text)[(container_num.Text).Length - 2];
                else lastIn = ' ';
     
                // Checks to see if the user is typing in numbers.
                if (prev_op.Equals("num"))
                {

                    // Checks to see if user is at the "end" of the deleting process and the container can be reset. 
                    if(lastIn == ' ' || lastIn== '-' || lastIn=='0' )  container_num.Text = "0";

                    /// Deletes the decimal point if every number after is deleted.
                    else if (lastIn == '.') container_num.Text = (container_num.Text).Substring(0, (container_num.Text).Length - 2);

                    // Base case - takes away a digit from the container.
                    else container_num.Text = (container_num.Text).Substring(0, (container_num.Text).Length - 1);
                }
            }

   
    }


}
