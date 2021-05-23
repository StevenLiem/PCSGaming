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
using System.IO;

namespace PCS_Gaming
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        OracleConnection conn;
        string currentuser = "", currentgame = "";
        string imageFolderPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 10) + "Images\\";
        DataTable dt;
        int subtotal;
        List<CartItem> userCart;
        public HomeWindow(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;
            generateview();
            userCart = new List<CartItem>();
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
            if (currentuser.Equals(""))
            {
                LoginWindow login = new LoginWindow(conn, this);
                login.ShowDialog();
            }
            else
            {
                if (MessageBox.Show("Yakin ingin Logout? Cart anda akan dihapus!", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    currentuser = "";
                    TBlMember.Text = "Guest";
                    userCart = new List<CartItem>();
                }
            }
            
        }

        private void generateview()
        {
            spbot.Children.Clear();
            OracleDataAdapter da = new OracleDataAdapter("SELECT GAME_ID,NAME FROM GAME WHERE IS_ACTIVE_GAME = 1 ORDER BY RELEASE_DATE DESC", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //for untuk ngeluarin berapa game
            for (int i = 0; i < 7; i++)
            {
                spbot.Children.Add(generategame(dt.Rows[i][1].ToString(), dt.Rows[i][0].ToString()));
            }
        }

        private Border generategame(string namagame,string kode)
        {
            Border b = new Border();
            b.BorderBrush = new SolidColorBrush(Color.FromRgb(81, 45, 168));
            b.BorderThickness = new Thickness(3);
            b.CornerRadius = new CornerRadius(5);
            b.Margin = new Thickness(2.5, 0, 2.5, 0);
            b.VerticalAlignment = VerticalAlignment.Center;

            Grid g = new Grid();
            g.Width = 160;
            g.Height = 188;
            //Buat Gambar
            RowDefinition r = new RowDefinition();
            r.Height = new GridLength(10, GridUnitType.Star);
            g.RowDefinitions.Add(r);
            //Buat Judul
            r = new RowDefinition();
            r.Height = new GridLength(2, GridUnitType.Star);
            g.RowDefinitions.Add(r);
            //Buat Gambar
            Image i = new Image();
            try
            {
                i.Source = new BitmapImage(new Uri(imageFolderPath + kode + ".png"));
            }
            catch (FileNotFoundException)
            {
                i.Source = null;
            }
            Grid.SetRow(i, 0);
            Grid.SetColumn(i, 0);


            //Mbuat Text
            Label t = new Label();
            t.Content = namagame;
            t.FontSize = 16;
            t.VerticalAlignment = VerticalAlignment.Bottom;
            t.HorizontalAlignment = HorizontalAlignment.Center;
            t.Foreground = new SolidColorBrush(Colors.White);
            t.FontWeight = FontWeights.Bold;
            Grid.SetRow(t, 1);
            Grid.SetColumn(t, 0);

            g.Children.Add(i);
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
            currentgame = kode;
            //MessageBox.Show(kode);
            string commandText = "select g.name, d.name, p.name, to_char(g.release_date, 'DD MONTH YYYY'), to_char(g.price,'999,999,999.99'), gg.name from game g, developer d, publisher p, genre gg where g.game_id=:a and g.developer_id = d.developer_id and g.publisher_id = p.publisher_id and g.genre_id = gg.genre_id";
            OracleCommand cmd = new OracleCommand(commandText,conn);
            cmd.Parameters.Add(":a", kode);
            conn.Open();
            OracleDataReader rd = cmd.ExecuteReader();
            try
            {
                
                while (rd.Read())
                {
                    lblTitleGame.Text = rd.GetString(0);
                    lblDevGame.Text = rd.GetString(1);
                    lblPubGame.Text = rd.GetString(2);
                    lblDateGame.Text = rd.GetString(3);
                    lblPriceGame.Text = "Rp. "+rd.GetString(4);
                    lblGenreGame.Text = rd.GetString(5);
                }
                imageGame.Source = new BitmapImage(new Uri(imageFolderPath + kode + ".png"));
                conn.Close();
                rd.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
                rd.Close();
            }

            //commandText = "select g.name from game_genre gg, genre g where gg.genre_id=g.genre_id and gg.game_id=:a";
            //cmd = new OracleCommand(commandText, conn);
            //cmd.Parameters.Add(":a", kode);
            //conn.Open();
            //rd = cmd.ExecuteReader();

            //try
            //{
            //    lblGenreGame.Text = "";
            //    int ctr = 0;
            //    while (rd.Read())
            //    {
            //        ctr++;
            //        lblGenreGame.Text += rd.GetString(0)+", ";
            //    }
            //    if (lblGenreGame.Text.Length > 0)
            //    {
            //        lblGenreGame.Text = lblGenreGame.Text.Remove(lblGenreGame.Text.Length - 2, 2);
            //    }
                
            //    conn.Close();
            //    rd.Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    conn.Close();
            //    rd.Close();
            //}

            gretb.Visibility = Visibility.Visible;
            greta.Visibility = Visibility.Hidden;
        }

        private void BtnHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (greta.Visibility == Visibility.Hidden)
            {
                greta.Visibility = Visibility.Visible;
                gretb.Visibility = Visibility.Hidden;
                gretc.Visibility = Visibility.Hidden;
                //generateview();
            }
        }

        public void changeuser(string kode)
        {
            currentuser = kode;
            TBlMember.Text = MainWindow.ambilstring($"SELECT REAL_NAME FROM MEMBER WHERE MEMBER_ID = '{currentuser}'");
            userCart = new List<CartItem>();
        }

        private void BtnCart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gretc.Visibility == Visibility.Hidden)
            {
                greta.Visibility = Visibility.Hidden;
                gretb.Visibility = Visibility.Hidden;
                gretc.Visibility = Visibility.Visible;
            }
        }

        private void updateDGCart()
        {
            subtotal = 0;
            dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Price");
            dt.Columns.Add("Qty");

            for (int i = 0; i < userCart.Count; i++)
            {
                DataRow tambah = dt.NewRow();
                tambah["No"] = i+1;
                tambah["Name"] = userCart[i].getNama();
                tambah["Price"] = userCart[i].getHarga();
                tambah["Qty"] = userCart[i].getJumlah();
                subtotal += userCart[i].getHarga() * userCart[i].getJumlah();

                dt.Rows.Add(tambah);
            }
            cartGrid.ItemsSource = null;
            cartGrid.ItemsSource = dt.DefaultView;
            cartGrid.IsReadOnly = true;
            totcart.Text = "Rp. " + String.Format("{0:N}", subtotal);
            
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Yakin ingin hapus game?", "WARNING",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DataRowView toBeRemoved = (DataRowView)cartGrid.SelectedItem;
                int urutan = Convert.ToInt32(toBeRemoved.Row[0]);
                urutan -= 1;
                userCart.RemoveAt(urutan);
                updateDGCart();
            }
        }


        private void ButttonBuy_Click(object sender, RoutedEventArgs e)
        {
            if (userCart.Count == 0)
            {
                MessageBox.Show("Cart anda masih kosong");
            }
            else
            {
                conn.Open();
                string token = "";
                Random r = new Random();
                for (int i = 0; i < 10; i++)
                {
                    if (r.Next(2) == 1)
                    {
                        token += (char)r.Next(48, 58);
                    }
                    else
                    {
                        token += (char)r.Next(65, 91);
                    }
                    
                }
                OracleCommand com = new OracleCommand($"INSERT INTO TRANSACTION VALUES ('{token}','{currentuser}',TO_DATE('{DateTime.Today.ToString("dd/MM/yyyy")}','dd/mm/yyyy'),'{subtotal}')", conn);
                com.ExecuteNonQuery();
                foreach (CartItem a in userCart)
                {
                    com = new OracleCommand($"INSERT INTO GAME_TRANSACTION VALUES ('{a.getKode()}','{token}',{a.getHarga()},{a.getJumlah()},{a.getJumlah() * a.getHarga()})", conn);
                    com.ExecuteNonQuery();
                }
                conn.Close();
                MessageBox.Show("Transaksi Berhasil :D");
                userCart = new List<CartItem>();
                updateDGCart();
            }
        }

        private void addgame_Click(object sender, RoutedEventArgs e)
        {
            if(jumbeli.Text.Trim(' ').Equals(""))
            {
                MessageBox.Show("Isian tidak boleh kosong");
            }
            else
            {
                jumbeli.Text = jumbeli.Text.Trim(' ');
                bool isAngka = true;
                for (int i = 0; i < jumbeli.Text.Length; i++)
                {
                    if (jumbeli.Text[i] < '0' || jumbeli.Text[i] > '9')
                    {
                        isAngka = false;
                    }
                }
                if (!isAngka)
                {
                    MessageBox.Show("Isian harus berupa angka");
                }
                else
                {
                    int angka = Int32.Parse(jumbeli.Text);
                    if (angka < 1)
                    {
                        MessageBox.Show("Isian tidak boleh 0");
                    }
                    else
                    {
                        int indexgame = -1;
                        for (int i = 0; i < userCart.Count; i++)
                        {
                            if (currentgame.Equals(userCart[i].getKode()))
                            {
                                indexgame = i;
                            }
                        }
                        if (indexgame >= 0)
                        {
                            userCart[indexgame].setAmount(angka);
                            MessageBox.Show("Jumlah item telah terupdate");
                            updateDGCart();
                            greta.Visibility = Visibility.Visible;
                            gretb.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            userCart.Add(new CartItem(currentgame, angka));
                            MessageBox.Show("Berhasil Masuk Cart :D");
                            updateDGCart();
                            greta.Visibility = Visibility.Visible;
                            gretb.Visibility = Visibility.Hidden;
                        }

                    }
                    jumbeli.Text = "";
                }
            }
        }
    }
}
