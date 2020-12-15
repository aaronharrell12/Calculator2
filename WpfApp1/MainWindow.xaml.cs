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
        ///     (Requirements 1.1,2.1)
        ///     Function 
        ///     DESCRIPTION:  Adds a digit/decimal to the number window.
        ///     --------------------------------------------------------|                             --------------------------------------------------------
        ///     |                                                       |                            |                                                        |
        ///     --------------------------------------------------------| ------ press 3/2/./4 ---->  --------------------------------------------------------|
        ///     |                                                      0|                            |                                                    32.4|
        ///     --------------------------------------------------------|                             --------------------------------------------------------|
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numbers_Clicked(object sender, RoutedEventArgs e)
            {
                if (prev_op.Equals("Res")) History_win.Text = "";

                Button numberButton      =               (Button)sender; // Grab the button being sent
                String numberValue       = (String)numberButton.Content; // Grab the digit or decimal
                String textIn            =           container_num.Text; // Grab the text in the calculator


                // First check for multiple decimals and also check 
                if ( !(numberValue).Equals(".") || !textIn.Contains(".") )
                {
                    // Clears number container under a couple different scenarios
                    if (textIn.Equals("0") || !prev_op.Equals("Num"))
                    {
                        if((numberValue).Equals(".")) container_num.Text = "0"+ numberValue;
                        else                          container_num.Text = numberValue;
                    }

                    // Adds a digit (or decimal) to the input
                    else container_num.Text += numberValue;

                }

                // SETS STATE VARIABLES
                res_op              =  false;
                prev_op             =  "Num";
            
                MonoString          =     "";
          
            

            }


        /// <summary>
        ///     (Requirements 1.6,2.5)
        ///     DESCRIPTION: Handles constant inputs like 
        ///         - e 
        ///         - pi
        ///     and replaces them with the approriate math value in the number container
        ///     --------------------------------------------------------|                          --------------------------------------------------------
        ///     |                                                       |                         |                                                       |
        ///     --------------------------------------------------------|  ------ press pi ---->  --------------------------------------------------------|
        ///     |                                                      0|                         |                      3.1415926535897932384626433832795|
        ///     --------------------------------------------------------|                         --------------------------------------------------------|
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Constants_Clicked(object sender, RoutedEventArgs e)
            {
            if (prev_op.Equals("Res")) History_win.Text = "";
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

                //----------------------UPDATE CONTAINERS-----------------------------
                // Adds num to the containeer
                container_num.Text = num;



                //-----------------------SET STATE VARIABLES-----------------------------
                prev_op    = "Constant";
                MonoString =         "";
            }



        /// <summary>
        ///    (Requirements: 1.3, 2.3)
        ///     DESCRIPTION: This is a handler for monomial button operators. 
        ///     
        ///     DESIGN:
        ///          These buttons will "STACK". Meaning that if the user pressed several monomial operators in a row they will stack on top of each other:
        ///            --------------------------------------------------------------------------------------------------------------------------------------
        ///            |                || State 0  |    |   State 1   |    |      State 2     |    |       State 3        |    |        State 4            |
        ///            | ---------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | User Presses   || No Press |--->|   [sin]     |--->|       [cos]      |--->|          [ln]        |--->|         [abs]             |
        ///            |----------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | Number Window  ||   "4"    |--->|  "0.06975"  |--->|     "0.999999"   |--->|        "-7.411"      |--->|        "7.411"            |
        ///            |----------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
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
        ///            |----------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | Number Window  ||   "4"    |--->|  "0.06975"  |--->|     "0.999999"   |--->|        "-7.411"      |--->|        "7.411"            |
        ///            |----------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | History window ||  "12+"   |--->| "12+sin(4)" |--->|  "12+cos(sin(4))"|--->|  "12+ln(cos(sin(4)))"|--->| "12+abs(ln(cos(sin(4))))" |
        ///            | ~~~~~~~~~~~~~~~||~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ History window update variables ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|
        ///            | OldHist        ||   ""     |--->|   "12+"     |--->|      "12+"       |--->|       "12+"          |--->|        "12+"              |
        ///            |----------------||----------|----|-------------|----|------------------|----|----------------------|----|---------------------------|
        ///            | MonoString     ||   ""     |--->|  "sin(4)"   |--->|   "cos(sin(4))"  |--->| "ln(cos(sin(4)))"    |--->| "abs(ln(cos(sin(4))))"    |
        ///            --------------------------------------------------------------------------------------------------------------------------------------
        ///   
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operations_mono_Clicked(object sender, RoutedEventArgs e)
            {
            if (prev_op.Equals("Res")) History_win.Text = "";
                Button monoOpButton      =               (Button)sender; // Grabs the button
                String container_numText =           container_num.Text; // Grabs the number container text
                String operator_         = (String)monoOpButton.Content; // Grabs the text from the button
                String operand           =                           "";
            
                if (MonoString.Equals(""))
                {
                    operand    = container_num.Text;
                    oldHist    = History_win.Text; //  Sets old hist to be the current state of the history container



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

                else operand = MonoString; // Define the mono string to be [operation]([monostring]) example cos(sin(4))




                 MonoString = operator_ + "(" + operand + ")"; //  Set the Mono string to be "[Operator](<number_container_value>)" EXAMPLE : "sin(4)"

                //--------------------------- UPDATE CONTAINERS ---------------------------------
               
                container_num.Text = (stf.Eval(MonoString)).ToString(); // Send the evaluated string to the number container
                History_win.Text   =              oldHist + MonoString; // Update history container

               

                //---------------------------SET STATE VARIABLES-----------------------------
                prev_op = "mono";


            }

        /// <summary>
        ///     ( Requirements: 1.2, 2.2)
        ///     DESCRIPTION: This handles binomial operations (operations that take in two inputs)
        ///     
        ///     Examples: 
        ///             1+2
        ///             3*4
        ///             4%5
        ///     --------------------------------------------------------|                          --------------------------------------------------------
        ///     |                                                       |                         |                                                     5+|
        ///     --------------------------------------------------------|  ------ press +  ---->  --------------------------------------------------------|
        ///     |                                                      5|                         |                                                      5|
        ///     --------------------------------------------------------|                         --------------------------------------------------------|
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // When an binomial_operations is clicked modify the window
        private void Operations_Clicked(object sender, RoutedEventArgs e)
            {
            if (prev_op.Equals("Res")) History_win.Text = "";
                Button binomButton       =              (Button)sender;
                String operator_         = (String)binomButton.Content;// Button Opeartion
                String numContainer      =          container_num.Text;// Text from user input
                String histContainer     =            History_win.Text;// Text from history window
                char lastHistChar;                                     // Defines the last input in history container  

                // Grab the last character from the history container
                if (History_win.Text.Length > 0) lastHistChar = (History_win.Text)[(History_win.Text).Length - 1];
                else                             lastHistChar = ' ';

            // If previous operation was a binomial then interpret press as user wanting to change the previous operation.
                if (prev_op.Equals("Binom")) histContainer = histContainer.Substring(0, histContainer.Length - 1) + operator_;

                else
                {

                    // If operand is already represented in source code
                    if (lastHistChar == ')') histContainer += operator_;

                    // Otherwise
                    else histContainer += numContainer + operator_;

                }

                // UPDATE WINDOWS
                History_win.Text = histContainer;

                // SET STATE VARIABLES
                prev_op = "Binom";
                MonoString = "";

        }



        /// <summary>
        ///     Requirments(1.8,2.6,2.61,2.62,2.63)
        ///     DESCRIPTIONS: Handles clicks on the parathesis buttons (left and right). 
        ///     
        ///             - Handles events where :
        ///                 - Right parathesis has been pressed without 
        ///                 
        ///     --------------------------------------------------------                           --------------------------------------------------------
        ///     |                                              5+sin(3)*|                         |                                             5+sin(3)*(|   
        ///     --------------------------------------------------------|  ------ press (  ---->  --------------------------------------------------------|
        ///     |                                                      5|                         |                                                      5|
        ///     --------------------------------------------------------|                         --------------------------------------------------------|     
        ///     INSTRUCTIONS:
        ///         When handling more edge cases. Please add to the switch statement to make the coding easier to read.
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operations_Per_Clicked(object sender, RoutedEventArgs e)
            {
                if (prev_op.Equals("Res")) History_win.Text = "";    


                
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
                            else            History_win.Text +=  "(";   // Default
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
        ///     (Requirements 1.8, 2.7)
        ///     DESCRIPTION: Handles the clicking of the results button (=).
        ///     The powerhouse behind this operation is going to be the StringToExpression class which can evaluate expresions.    
        ///     
        ///     --------------------------------------------------------                           --------------------------------------------------------
        ///     |                                              5+sin(3)*|                         |                                            5+sin(3)*5=|   
        ///     --------------------------------------------------------|  ------ press =  ---->  --------------------------------------------------------|
        ///     |                                                      5|                         | 5.7056000402993361105037240140405513992346663212613279|
        ///     --------------------------------------------------------|                         --------------------------------------------------------|
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operations_Res_Clicked(object sender, RoutedEventArgs e)
            {

                string binomial_operators =        "+-/*^%("; // Sets the usual binomial operators used. 
                String History_winText    = History_win.Text; // Grabs the history container text
                char lastHistChar;                            // Hold last input in history container



                //   Last character in the history window is defined.
                    if (History_winText.Length > 0) lastHistChar = (History_winText)[(History_winText).Length - 1];
                    else lastHistChar = ' ';
         

                // Edge Case: Dangling binomial operator or open/empty left parenthesis
                    if (binomial_operators.IndexOf(lastHistChar) >=0) History_winText += container_num.Text;
               

                // Edge Case: When parethesis aren't closed. 
                    while (perCount > 0)
                    {
                        History_winText += ")";
                        perCount--;
                    }
                // Edge Case: When history container is zero
                    if(History_winText.Equals("") ) History_winText = container_num.Text;


            //---------------------------------- UPDATE CONTAINERS ----------------------------------
            if (!prev_op.Equals("Res")) // First time click (=)
            {
                if (   History_winText.Equals("NaN") 
                    && History_winText.Equals("∞") 
                    && History_winText.Equals("-∞") ) 
                     container_num.Text =                      History_winText;              // Edge case: Infinity and null in the result window
                else container_num.Text = stf.Eval(History_winText).ToString();              // Default Case:  evaluates history expression


                History_win.Text        = History_winText + "=";                             // Adds and equal sign on the end of the history container.
            }
            else History_win.Text       = History_winText;                                   // Repeatedly clicking the result buttons should span the same results
            

            //-------------------------------CHANGE STATE VARIABLES --------------------------------
                prev_op    = "Res";
                res_op     =  true;
                MonoString =    "";
            }






        /// <summary>
        ///     (Requirements: 1.4)
        ///   DESCRIPTION: Clear button operator. Resets Windows back to default and resets the state variables to default.
        ///     --------------------------------------------------------                           --------------------------------------------------------
        ///     |                                              5+sin(3)*|                         |                                                       |   
        ///     --------------------------------------------------------|  ------ press cls  -->  --------------------------------------------------------|
        ///     |                                                      5|                         |                                                      0|
        ///     --------------------------------------------------------|                         --------------------------------------------------------|
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operations_clr_Clicked(object sender, RoutedEventArgs e)
            {


                //----------------------UPDATE CONTAINERS-------------------------------
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
        ///     (Requirements: 1.5, 2.4, 2.41, 2.42 )
        /// DESCIPTION: Allows users to back track the inputs being entered in the number container, WHILST TYPING IN NUMBERS. If the number container contains a result from calculations it clears the container.
        ///     --------------------------------------------------------                           --------------------------------------------------------
        ///     |                                              5+sin(3)*|                         |                                              5+sin(3)*|
        ///     --------------------------------------------------------|  ------ press DEL  -->  --------------------------------------------------------|
        ///     |                                                    543|                         |                                                     54|
        ///     --------------------------------------------------------|                         --------------------------------------------------------|
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
                if (prev_op.Equals("Num"))
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
