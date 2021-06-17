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
        DataTable dt, dtBundleList;
        int subtotal;
        List<CartItem> userCart;
        List<string> searchResultKodeGame;
        public HomeWindow(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;
            generateview();
            userCart = new List<CartItem>();
            searchResultKodeGame = new List<string>();
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
                    updateDGCart();
                    btnBund.Visibility = Visibility.Hidden;
                }
            }
            
        }

        private void generateview()
        {
            sptop.Children.Clear();
            spbot.Children.Clear();
            OracleDataAdapter da = new OracleDataAdapter("SELECT GAME_ID,NAME FROM GAME WHERE IS_ACTIVE_GAME = 1 ORDER BY RELEASE_DATE DESC", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //for untuk ngeluarin berapa game
            for (int i = 0; i < 7; i++)
            {
                spbot.Children.Add(generategame(dt.Rows[i][1].ToString(), dt.Rows[i][0].ToString()));
            }
            da = new OracleDataAdapter("SELECT nvl(gt.GAME_ID,g.GAME_ID),g.NAME ,nvl(sum(gt.QTY),0) as \"iya\" FROM GAME_TRANSACTION gt RIGHT JOIN GAME g ON gt.GAME_ID = g.GAME_ID WHERE g.IS_ACTIVE_GAME = 1 GROUP BY gt.GAME_ID,g.NAME,g.GAME_ID ORDER BY \"iya\" DESC,g.NAME ASC", conn);
            dt = new DataTable();
            da.Fill(dt);
            //for untuk ngeluarin berapa game
            for (int i = 0; i < 7; i++)
            {
                sptop.Children.Add(generategame(dt.Rows[i][1].ToString(), dt.Rows[i][0].ToString()));
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
            gretc.Visibility = Visibility.Hidden;
            gretd.Visibility = Visibility.Hidden;
            grete.Visibility = Visibility.Hidden;
        }

        private void BtnHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (greta.Visibility == Visibility.Hidden)
            {
                greta.Visibility = Visibility.Visible;
                gretb.Visibility = Visibility.Hidden;
                gretc.Visibility = Visibility.Hidden;
                gretd.Visibility = Visibility.Hidden;
                grete.Visibility = Visibility.Hidden;
                //generateview();
            }
        }

        public void changeuser(string kode)
        {
            currentuser = kode;
            TBlMember.Text = MainWindow.ambilstring($"SELECT REAL_NAME FROM MEMBER WHERE MEMBER_ID = '{currentuser}'");
            userCart = new List<CartItem>();
            updateDGCart();
        }

        private void BtnCart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gretc.Visibility == Visibility.Hidden)
            {
                greta.Visibility = Visibility.Hidden;
                gretb.Visibility = Visibility.Hidden;
                gretc.Visibility = Visibility.Visible;
                gretd.Visibility = Visibility.Hidden;
                grete.Visibility = Visibility.Hidden;
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
                if (MessageBox.Show("Token Anda adalah "+token+", apakah anda ingin jiplak ke clipboard?", "KONFIRMASI", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Clipboard.SetText(token);
                    MessageBox.Show("Telah terjiplak di clipboard");
                }
                OracleCommand com;
                //com= new OracleCommand($"INSERT INTO TRANSACTION VALUES ('{token}','{currentuser}',TO_DATE('{DateTime.Today.ToString("dd/MM/yyyy")}','dd/mm/yyyy'),'{subtotal}')", conn);
                //com.ExecuteNonQuery();
                foreach (CartItem a in userCart)
                {
                    if (currentuser.Length > 0)
                    {
                        com = new OracleCommand($"INSERT INTO TOKEN_CONTENTS VALUES ('{token}','{a.getKode()}','{currentuser}','{a.getJumlah()}')", conn);
                    }
                    else
                    {
                        com = new OracleCommand($"INSERT INTO TOKEN_CONTENTS VALUES ('{token}','{a.getKode()}',NULL,'{a.getJumlah()}')", conn);
                    }
                    
                    com.ExecuteNonQuery();
                }
                conn.Close();
                userCart = new List<CartItem>();
                updateDGCart();
            }
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(greta.Visibility == Visibility.Visible)
                {
                    greta.Visibility = Visibility.Hidden;
                }
                if (gretb.Visibility == Visibility.Visible)
                {
                    gretb.Visibility = Visibility.Hidden;
                }
                if (gretc.Visibility == Visibility.Visible)
                {
                    gretc.Visibility = Visibility.Hidden;
                }
                if (grete.Visibility == Visibility.Visible)
                {
                    grete.Visibility = Visibility.Hidden;
                }
                gretd.Visibility = Visibility.Visible;

                searchResultKodeGame.Clear();

                string query = searchBox.Text.ToLower();
                resultText.Text = "Showing \"" +query+ "\"";

                OracleCommand cmd = new OracleCommand("select g.name, d.name, p.name, g.game_id from game g, developer d, publisher p where g.developer_id=d.developer_id and g.publisher_id = p.publisher_id and (lower(g.name) like '%"+query+"%' or lower(d.name) like '%"+query+"%' or lower(p.name) like '%"+query+"%')", conn);
                //MessageBox.Show(cmd.CommandText);
                conn.Open();
                OracleDataReader rd = cmd.ExecuteReader();

                DataTable tempDt = new DataTable();
                tempDt.Columns.Add("No");
                tempDt.Columns.Add("Title");
                tempDt.Columns.Add("Developer");
                tempDt.Columns.Add("Publisher");
                int count = 0;
                while (rd.Read())
                {
                    count++;
                    DataRow tambah = tempDt.NewRow();
                    tambah["No"] = count;
                    tambah["Title"] = rd.GetString(0);
                    tambah["Developer"] = rd.GetString(1);
                    tambah["Publisher"] = rd.GetString(2);
                    searchResultKodeGame.Add(rd.GetString(3));

                    tempDt.Rows.Add(tambah);
                }
                conn.Close();
                searchGrid.ItemsSource = null;
                searchGrid.ItemsSource = tempDt.DefaultView;
                searchGrid.IsReadOnly = true;


            }
        }

        private void SearchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string kode = searchResultKodeGame[searchGrid.SelectedIndex];
            currentgame = kode;
            //MessageBox.Show(kode);
            string commandText = "select g.name, d.name, p.name, to_char(g.release_date, 'DD MONTH YYYY'), to_char(g.price,'999,999,999.99'), gg.name from game g, developer d, publisher p, genre gg where g.game_id=:a and g.developer_id = d.developer_id and g.publisher_id = p.publisher_id and g.genre_id = gg.genre_id";
            OracleCommand cmd = new OracleCommand(commandText, conn);
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
                    lblPriceGame.Text = "Rp. " + rd.GetString(4);
                    lblGenreGame.Text = rd.GetString(5);
                }
                imageGame.Source = new BitmapImage(new Uri(imageFolderPath + kode + ".png"));
                conn.Close();
                rd.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                rd.Close();
            }

            gretb.Visibility = Visibility.Visible;
            greta.Visibility = Visibility.Hidden;
            gretc.Visibility = Visibility.Hidden;
            gretd.Visibility = Visibility.Hidden;
            grete.Visibility = Visibility.Hidden;
        }

        private void BtnAddBundle_Click(object sender, RoutedEventArgs e)
        {
            int indexgame = -1;
            for (int i = 0; i < userCart.Count; i++)
            {
                if (dtBundleList.Rows[bundleGrid.SelectedIndex].ItemArray[0].ToString().Equals(userCart[i].getKode()))
                {
                    indexgame = i;
                }
            }
            if (indexgame >= 0)
            {
                userCart[indexgame].setAmount(1);
                MessageBox.Show("Bundle hanya bisa beli 1 tiap transaksi!");
                //updateDGCart();
                //greta.Visibility = Visibility.Visible;
                //gretb.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show(dtBundleList.Rows[bundleGrid.SelectedIndex].ItemArray[0].ToString());
                userCart.Add(new CartItem(dtBundleList.Rows[bundleGrid.SelectedIndex].ItemArray[0].ToString(), 1));
                MessageBox.Show("Berhasil Masuk Cart :D");
                updateDGCart();
                greta.Visibility = Visibility.Visible;
                gretb.Visibility = Visibility.Hidden;
                grete.Visibility = Visibility.Hidden;
            }
        }

        private void BtnBund_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dtBundleList = new DataTable();
            dtBundleList.Columns.Add("Kode");
            dtBundleList.Columns.Add("Name");
            dtBundleList.Columns.Add("Price");
            dtBundleList.Columns.Add("Discount");

            OracleCommand cmd = new OracleCommand("select * from bundle where is_active=1", conn);
            conn.Open();
            OracleDataReader rd = cmd.ExecuteReader();

            try
            {
                while (rd.Read())
                {
                    DataRow tambah = dtBundleList.NewRow();
                    tambah["Kode"] = rd.GetString(0);
                    tambah["Name"] = rd.GetString(1);
                    tambah["Price"] = rd.GetDecimal(2);
                    tambah["Discount"] = rd.GetDecimal(3);

                    dtBundleList.Rows.Add(tambah);
                }
                bundleGrid.ItemsSource = null;
                bundleGrid.ItemsSource = dtBundleList.DefaultView;
                bundleGrid.Columns[0].Visibility = Visibility.Hidden;
                bundleGrid.IsReadOnly = true;
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }

            grete.Visibility = Visibility.Visible;
            greta.Visibility = Visibility.Hidden;
            gretb.Visibility = Visibility.Hidden;
            gretc.Visibility = Visibility.Hidden;
            gretd.Visibility = Visibility.Hidden;


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
