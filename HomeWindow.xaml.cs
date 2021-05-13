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
using System.Data;

namespace PCS_Gaming
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        OracleConnection conn;
        public HomeWindow(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;
            generateview();
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
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void TBlMember_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow login = new LoginWindow(conn);
            login.ShowDialog();
        }

        private void generateview()
        {
            OracleDataAdapter da = new OracleDataAdapter("SELECT GAME_ID,NAME FROM GAME WHERE IS_ACTIVE_GAME = 1 ORDER BY RELEASE_DATE DESC", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //for untuk ngeluarin berapa game
            spbot.Width = 0;
            for (int i = 0; i < 7; i++)
            {
                spbot.Width = spbot.Width + 172;
                spbot.Children.Add(generategame(dt.Rows[i][1].ToString(), dt.Rows[i][0].ToString()));
            }
        }

        private Border generategame(string namagame,string kode)
        {
            Border b = new Border();
            b.BorderBrush = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            b.BorderThickness = new Thickness(3);
            b.CornerRadius = new CornerRadius(15);
            b.Margin = new Thickness(2.5, 0, 2.5, 0);

            Grid g = new Grid();
            g.Width = 160;
            g.Height = 188;
            //Buat Gambar
            RowDefinition r = new RowDefinition();
            r.Height = new GridLength(6, GridUnitType.Star);
            g.RowDefinitions.Add(r);
            //Buat Judul
            r = new RowDefinition();
            r.Height = new GridLength(4, GridUnitType.Star);
            g.RowDefinitions.Add(r);
            //Mbuat Text
            Label t = new Label();
            t.Content = namagame;
            t.FontSize = 16;
            t.VerticalAlignment = VerticalAlignment.Center;
            t.HorizontalContentAlignment = HorizontalAlignment.Center;
            t.HorizontalAlignment = HorizontalAlignment.Center;
            t.Foreground = new SolidColorBrush(Colors.White);
            t.FontWeight = FontWeights.Bold;
            
            Grid.SetRow(t, 1);
            Grid.SetColumn(t, 0);
            g.Children.Add(t);
            g.Name = kode;
            g.MouseDown += leftclick;
            g.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));

            b.Child = g;
            
            return b;
        }

        private void leftclick(object sender, MouseButtonEventArgs e)
        {
            String kode = ((Grid)sender).Name;
            MessageBox.Show(kode);
        }
    }
}
