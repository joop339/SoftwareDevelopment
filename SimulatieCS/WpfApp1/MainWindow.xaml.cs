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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            auto.Visibility = Visibility.Hidden;
        }

        public void Update()
        {
            //while (auto.Visibility == Visibility.Visible && auto.Margin.Left < 100)
            //{
            auto.Margin = new Thickness(auto.Margin.Left + 1, 314, 601, 0);
            //}
        }

        private void slButton_Click(object sender, RoutedEventArgs e)
        {
            if (stoplicht.Fill == Brushes.Green)
            {
                stoplicht.Fill = Brushes.Red;
            }
            else
            {
                stoplicht.Fill = Brushes.Green;
            }

        }

        private void autoButton_Click(object sender, RoutedEventArgs e)
        {
            if (auto.Visibility == Visibility.Hidden)
            {
                auto.Visibility = Visibility.Visible;
                Update();
            }
            if (auto.Visibility == Visibility.Visible)
            {
                Update();
            }
        }

    }
}
