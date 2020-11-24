using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public class atomic_Fraction
    {
        double atomic_numerator;
        double atomic_denominator;
        Grid fracGrid;
        
        public atomic_Fraction(double atom_num,double atom_denom){

            // Part 1: SET THE VALUE OF NUMERATOR AND DENOMINATOR

                // Step 1: Set value of Numerator
                    atomic_numerator   =   atom_num;

                // Step 2: Set value of Denominator
                    atomic_denominator = atom_denom;



            // Part 2: CREATE THE FRACTION GRID
               
                // Step 1: Initialize Grid
                    fracGrid = new Grid();

                    // Step 1.1 Set Height and Width
                       fracGrid.Height = 50;
                       fracGrid.Width  = 40;

                // Step 2 : Create the rows in the Grid

                    // Step 2.1: Initialize the Rows
                        RowDefinition row1 = new RowDefinition();
                        RowDefinition row2 = new RowDefinition();
                        RowDefinition row3 = new RowDefinition();

                    // Step 2.2: Define the RowDefinition Heights
                        GridLength numberHeights = new GridLength(4);
                        GridLength lineHeight    = new GridLength(1);

                    // Step 2.3: Set the Row heights
                        row1.Height = numberHeights;
                        row2.Height =    lineHeight;
                        row3.Height = numberHeights;

                    // Step 2.4: Add the rows to the Grid
                        fracGrid.RowDefinitions.Add(row1);
                        fracGrid.RowDefinitions.Add(row2);
                        fracGrid.RowDefinitions.Add(row3);

                // Step 3: Add labels to each row 

                    // Step 3.1: Create the neccessary Labels

                        // Step 3.1.1: Initilize Label objects
                            Label num_label     = new Label();
                            Label denom_label   = new Label();

                        // Step 3.1.2: Set content values
                            num_label.Content   = atomic_numerator;
                            denom_label.Content = atomic_denominator;

                        // Step 3.1.3: Set the rows of the values
                            Grid.SetRow(num_label  , 0);
                            Grid.SetRow(denom_label, 2);

                    // Step 3.2: Add those labels to that row
                        fracGrid.Children.Add(num_label);
                        fracGrid.Children.Add(denom_label);
            
                // Step 4: Update layout
                    fracGrid.UpdateLayout();
        }



        public atomic_Fraction add(double number)
        {
            double num      = number * atomic_denominator + atomic_numerator;
            double denom    = atomic_denominator;
            return new atomic_Fraction(num, denom);
        }

        public atomic_Fraction add(atomic_Fraction frac)
        {
            double num   = frac.atomic_numerator * atomic_denominator + frac.atomic_denominator * atomic_numerator;
            double denom = frac.atomic_denominator * atomic_denominator;
            return new atomic_Fraction(num, denom);
        }

        public Grid getGrid()
        {
            return fracGrid;
        }
       



    }
}
