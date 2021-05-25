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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PCS_Gaming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static OracleConnection conn;
        public static string dataSource, user, pass;
        public MainWindow()
        {
            InitializeComponent();
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
            dataSource = TBDS.Text;
            user = TBUser.Text;
            pass = TBPass.Password;
            var connectionString = "Data Source = " + dataSource + "; User ID=" + user + "; Password=" + pass;
            conn = new OracleConnection(connectionString);
            try
            {
                conn.Open();
                conn.Close();
                StartNavigation nav = new StartNavigation(conn);
                this.Hide();
                nav.ShowDialog();
            }
            catch (OracleException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }
        }

        public static string ambilstring(string query)
        {
            conn.Open();
            OracleCommand cmd = new OracleCommand(query,conn);
            string eh;
            if(cmd.ExecuteScalar() == null)
            {
                conn.Close();
                return "";
            }
            else
            {
                eh = cmd.ExecuteScalar().ToString();
                conn.Close();
                return eh;
            }
        }
    }
}
