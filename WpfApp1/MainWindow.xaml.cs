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
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double current_Value;
        public MainWindow()
        {
           
            InitializeComponent();
            Console.Out.WriteLine("ldkkjf");
            Debug.WriteLine("something");
            Debug.Write("dkaljfopawiejefkao akdjfopjawpeo");
            
            for (int i =0; i< grid.Children.Count; i++)
            {

                Button ele = (Button) grid.Children[i];
                String[] name_arr = ele.Name.Split('_');
                String    name   = name_arr[0];
                //Trace.WriteLine(name);
                if (name.Equals("Number"))
                {
                    ele.Click += new RoutedEventHandler(Numbers_Clicked);
                    
                }
                if (name.Equals("Operation"))
                {
                    name = name_arr[1];
                    
                    if (name.Equals("Bin"))
                    {
                        ele.Click += new RoutedEventHandler(Operations_Clicked);
                    }
                }
                grid.Children[i] = ele;
            }
        }

        private void Numbers_Clicked(object sender, RoutedEventArgs e)
        {

            Console.Out.WriteLine("ldkkjf");
            Trace.WriteLine("dafeaf");
            Debug.Write("dkaljfopawiejefkao akdjfopjawpeo");
            Button x = (Button) sender;
            String textIn = textBlock.Text;

            textIn += (String)x.Content;

            textBlock.Text = textIn;

        }
        private void Operations_Clicked(object sender, RoutedEventArgs e)
        {

            Button x = (Button)sender;
            
            // Text from user input
            String textIn = textBlock.Text;
            Console.WriteLine(textIn);


            String endPiece = textIn.Substring(Math.Max(0, textIn.Length - 1));
            if (int.TryParse(endPiece, out int empty))
            {
                textIn = "(" + textIn + ")";
            }
            else
            {
                textIn = textIn.Remove(textIn.Length - 1);
            }

            textIn += (String)x.Content;

            Console.WriteLine(textIn);
            textBlock.Text = textIn;




        }
        private void Operations_Res_Clicked(object sender, RoutedEventArgs e)
        {

            Button x = (Button)sender;

            // Text from user input
            String textIn = textBlock.Text;
            Console.WriteLine(textIn);


            String endPiece = textIn.Substring(Math.Max(0, textIn.Length - 1));
            if (int.TryParse(endPiece, out int empty))
            {
                textIn = "(" + textIn + ")";
            }
            else
            {
                textIn = textIn.Remove(textIn.Length - 1);
            }

            textIn += (String)x.Content;

            Console.WriteLine(textIn);
            textBlock.Text = textIn;




        }
    }


}
