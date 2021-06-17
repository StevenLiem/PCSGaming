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
        HomeWindow hw;

        public LoginWindow(OracleConnection conn,HomeWindow he)
        {
            InitializeComponent();
            this.hw = he;
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

                string kode = MainWindow.ambilstring($"SELECT MEMBER_ID FROM MEMBER WHERE UPPER(USERNAME) = '{TBUser.Text.Trim(' ').ToUpper()}'");
                if (kode.Equals(""))
                {
                    MessageBox.Show("User tidak ditemukan!");
                }
                else
                {
                    if (TBPass.Password.Equals(MainWindow.ambilstring($"SELECT PASSWORD FROM MEMBER WHERE MEMBER_ID = '{kode}'")))
                    {
                        MessageBox.Show("Selamat Datang " + MainWindow.ambilstring($"SELECT REAL_NAME FROM MEMBER WHERE MEMBER_ID = '{kode}'") + " :D");
                        hw.changeuser(kode);
                        hw.btnBund.Visibility = Visibility.Visible;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Password Salah");
                    }
                }
            }
        }

        private void TBlRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow regist = new RegisterWindow(conn);
            regist.ShowDialog();
        }
    }
}
