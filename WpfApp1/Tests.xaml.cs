﻿using System;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class Tests : Window
    {
        StringToFormula x;
        public Tests()
        {
            x = new StringToFormula();

            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            String v = textBox.Text;
            double val = x.Eval(v);
            label2.Content = val.ToString();

        }
    }
}
