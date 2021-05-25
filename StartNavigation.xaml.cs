using Oracle.DataAccess.Client;
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
using System.Windows.Shapes;

namespace PCS_Gaming
{
    /// <summary>
    /// Interaction logic for StartNavigation.xaml
    /// </summary>
    public partial class StartNavigation : Window
    {
        OracleConnection conn;
        public StartNavigation(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAdmin_Click(object sender, RoutedEventArgs e)
        {
            HomeCashier home = new HomeCashier(conn);
            home.ShowDialog();
        }

        private void ButtonCustomer_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow home = new HomeWindow(conn);
            home.ShowDialog();
        }
    }
}
