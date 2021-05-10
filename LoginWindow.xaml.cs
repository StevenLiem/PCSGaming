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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        OracleConnection conn;
        public LoginWindow(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;
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

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TBUser.Text) && !string.IsNullOrEmpty(TBPass.Password))
            {
                string user = TBUser.Text;
                string pass = TBPass.Password;

                if (user.Equals("admin") && pass.Equals("admin"))
                {
                    AdminNavigation admin = new AdminNavigation(conn);
                    admin.ShowDialog();
                }
            }
        }
    }
}
