using Microsoft.Win32;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for HomeCashier.xaml
    /// </summary>
    public partial class HomeCashier : Window
    {
        OracleConnection conn;
        DataTable dtGame;
        List<string> devID, pubID, genID, game_id_for_combo, game_id_add_to_bundle;
        string query, selectedID;
        string imageFolderPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 10) + "Images\\";
        int selectedStock, total, total_discount;
        BitmapImage gambar;
        public HomeCashier(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;

            game_id_add_to_bundle = new List<string>();
            total = 0;
            total_discount = 0;
            fillComboBox();
            load_bundle();
            reloadDG();
        }

        private void reloadDG()
        {
            query = $"select GAME_ID as \"ID\", NAME as \"Title\", " +
                    $"(select NAME from DEVELOPER where DEVELOPER_ID=g.DEVELOPER_ID) as \"Developer\", " +
                    $"(select NAME from PUBLISHER where PUBLISHER_ID=g.PUBLISHER_ID) as \"Publisher\", " +
                    $"(select NAME from GENRE where GENRE_ID=g.GENRE_ID) as \"Genre\", " +
                    $"PRICE as \"Price\", STOCK as \"Stock\", IS_ACTIVE_GAME as \"Is Active\" from GAME g";

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(query, conn);
            dtGame = new DataTable();
            da.Fill(dtGame);
            DGGame.ItemsSource = dtGame.DefaultView;
            DGGame.IsReadOnly = true;
            imagePreview.Source = null;
            conn.Close();
        }


        private void fillComboBox()
        {
            devID = new List<string>();
            pubID = new List<string>();
            genID = new List<string>();
            game_id_for_combo = new List<string>();

            conn.Open();

            query = $"select * from DEVELOPER";
            OracleCommand cmd = new OracleCommand(query, conn);
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                devID.Add(reader.GetString(0));
                CBDeveloper.Items.Add(reader.GetString(1));
            }
            reader.Close();

            query = $"select * from PUBLISHER";
            cmd = new OracleCommand(query, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pubID.Add(reader.GetString(0));
                CBPublisher.Items.Add(reader.GetString(1));
            }
            reader.Close();

            query = $"select * from GENRE";
            cmd = new OracleCommand(query, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                genID.Add(reader.GetString(0));
                CBGenre.Items.Add(reader.GetString(1));
            }
            reader.Close();

            query = $"select game_id, name from GAME";
            cmd = new OracleCommand(query, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                game_id_for_combo.Add(reader.GetString(0));
                CBSelectGame.Items.Add(reader.GetString(1));
            }
            reader.Close();

            conn.Close();
        }

        private bool checkInputtedData()
        {
            if (!string.IsNullOrEmpty(TBIDGame.Text) && !string.IsNullOrEmpty(TBTitle.Text) && !string.IsNullOrEmpty(TBStock.Text) && !string.IsNullOrEmpty(TBPrice.Text) && CBDeveloper.SelectedIndex != -1 && CBPublisher.SelectedIndex != -1 && CBGenre.SelectedIndex != -1)
            {
                // cek apakah stock dan harga sudah angka
                int price = 0, stock = 0;
                try
                {
                    price = Int32.Parse(TBPrice.Text);
                    stock = Int32.Parse(TBStock.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid Price/Stock Value");
                    return false;
                }
                if (price < 10000 || stock < 0)
                {
                    MessageBox.Show("Invalid Price/Stock Value");
                    return false;
                }

                return true;
            }
            else
            {
                MessageBox.Show("Pleasse fill all the fields");
                return false;
            }
        }

        void saveimage(string namagambar)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(gambar));
            using (var fileStream = new System.IO.FileStream(imageFolderPath + namagambar + ".png", System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private void ButtonHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            resetgrid();
            GridHome.Visibility = Visibility.Visible;
        }

        private void ButtonMasterGame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            resetgrid();
            GridMasterGame.Visibility = Visibility.Visible;
        }

        private void ButtonMasterBundle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            resetgrid();
            GridMasterBundle.Visibility = Visibility.Visible;
            load_bundle();
        }

        private void ButtonMasterReport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            resetgrid();
            GridMasterReport.Visibility = Visibility.Visible;
        }

        private void DGGame_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ButtonInsert.IsEnabled = false;
            ButtonUpdate.IsEnabled = true;
            ButtonDelete.IsEnabled = true;

            DataGrid dg = sender as DataGrid;
            DataRowView row = dg.SelectedItem as DataRowView;
            if (row != null)
            {
                selectedID = row["ID"].ToString();
                TBTitle.Text = row["Title"].ToString();
                for (int i = 0; i < devID.Count; i++)
                {
                    if (CBDeveloper.Items[i].ToString().Equals(row["Developer"].ToString()))
                    {
                        CBDeveloper.SelectedIndex = i;
                        break;
                    }
                }
                for (int i = 0; i < pubID.Count; i++)
                {
                    if (CBPublisher.Items[i].ToString().Equals(row["Publisher"].ToString()))
                    {
                        CBPublisher.SelectedIndex = i;
                        break;
                    }
                }
                for (int i = 0; i < genID.Count; i++)
                {
                    if (CBGenre.Items[i].ToString().Equals(row["Genre"].ToString()))
                    {
                        CBGenre.SelectedIndex = i;
                        break;
                    }
                }
                TBPrice.Text = row["Price"].ToString();
                TBStock.Text = row["Stock"].ToString();
                selectedStock = Int32.Parse(row["Stock"].ToString());
                TBIDGame.Text = selectedID;

                if (row["Is Active"].ToString() == "1")
                {
                    rbActive.IsChecked = true;
                    rbInactive.IsChecked = false;
                }
                else
                {
                    rbActive.IsChecked = false;
                    rbInactive.IsChecked = true;
                }

                try
                {
                    imagePreview.Source = new BitmapImage(new Uri(imageFolderPath + selectedID + ".png"));
                }
                catch (FileNotFoundException)
                {
                    imagePreview.Source = null;
                }
            }

            ButtonClear.Content = "Cancel";
            ButtonClear.ToolTip = "Cancel Selection";
        }

        private void ButtonAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files (JPG,PNG,GIF)|*.JPG;*.PNG;*.GIF";
            if (fileDialog.ShowDialog() == true)
            {
                gambar = new BitmapImage(new Uri(fileDialog.FileName));
                imagePreview.Source = gambar;
            }
        }

        private void TBTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            //cek apakah ada data yg lagi dipilih
            if (!string.IsNullOrEmpty(selectedID))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(TBTitle.Text) && TBTitle.Text.Length >= 2)
            {
                conn.Open();
                query = $"select GENERATE_GAME_ID('{TBTitle.Text}') from dual";
                OracleCommand cmd = new OracleCommand(query, conn);
                try
                {
                    string kode = cmd.ExecuteScalar().ToString();
                    TBIDGame.Text = kode;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                conn.Close();
            }
            else
            {
                TBIDGame.Text = "";
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ButtonClear.Content = "Clear";
            ButtonClear.ToolTip = "Clear all inputs";

            selectedID = "";
            TBTitle.Text = "";
            CBDeveloper.SelectedIndex = -1;
            CBPublisher.SelectedIndex = -1;
            CBGenre.SelectedIndex = -1;
            TBPrice.Text = "";
            TBStock.Text = "";

            ButtonInsert.IsEnabled = true;
            ButtonUpdate.IsEnabled = false;
            ButtonDelete.IsEnabled = false;
            rbActive.IsChecked = true;
            rbInactive.IsChecked = false;

            imagePreview.Source = null;
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

        private void tokhen_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return && !tokhen.Text.Trim(' ').Equals(""))
            {
                showgames();
            }
        }

        void showgames()
        {
            if(tokhen.Text.Trim(' ').Equals(""))
            {
                MessageBox.Show("Token Mohon Diisi");
            }
            else
            {
                tokhen.Text = tokhen.Text.Trim(' ');
                try
                {
                    OracleDataAdapter da = new OracleDataAdapter($"SELECT ROWNUM as \"NO.\",g.NAME,g.PRICE,t.QTY FROM TOKEN_CONTENTS t, GAME g WHERE TOKEN_ID='{tokhen.Text}' and t.GAME_ID = g.GAME_ID", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    inicart.ItemsSource = dt.AsDataView();
                    resetgrid();
                    GridCart.Visibility = Visibility.Visible;
                    MessageBox.Show(dt.Rows.Count+"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                
            }
        }

        private void load_bundle()
        {
            DGBundle.Items.Clear();
            string gen_id;
            query = $"select bundle_id as \"Bundle_ID\", name as \"Name\" from BUNDLE";

            conn.Open();
            OracleDataAdapter dab = new OracleDataAdapter(query, conn);
            DataTable dtb = new DataTable();
            dab.Fill(dtb);
            gen_id = "BDL";
            if (dtb.Rows.Count >= 9)
            {
                gen_id += (dtb.Rows.Count + 1);
            }
            else
            {
                gen_id += "0" + (dtb.Rows.Count + 1);
            }
            DGBundle.ItemsSource = dtb.DefaultView;
            DGBundle.IsReadOnly = true;
            conn.Close();

            TBIDBundle.Text = gen_id;
        }

        private Boolean cek_string(string x)
        {
            char[] cr = x.ToCharArray();
            bool cek = true;
            for(int i = 0; i < x.Length; i++)
            {
                if(cr[i]<48 || cr[i] > 57)
                {
                    cek = false;
                    break;
                }
            }
            return cek;
        }


        private void TBDisc_TextChanged(object sender, RoutedEventArgs e)
        {
            if (game_id_add_to_bundle.Count > 0)
            {
                total = 0;
                query = $"select game_id, price from GAME";
                OracleDataAdapter dag = new OracleDataAdapter(query, conn);
                DataTable dtg = new DataTable();
                dag.Fill(dtg);
                for (int i = 0; i < game_id_add_to_bundle.Count; i++)
                {
                    for (int j = 0; j < dtg.Rows.Count; j++)
                    {
                        if(dtg.Rows[j].ItemArray[0].ToString() == game_id_add_to_bundle[i])
                        {
                            total += Convert.ToInt32(dtg.Rows[j].ItemArray[1].ToString());
                        }
                    }
                }
                total_discount = total;

                if (TBDiscount.Text != "")
                {
                    if (!cek_string(TBDiscount.Text))
                    {
                        MessageBox.Show("Invalid Discount Value!");
                    }
                    else
                    {
                        total_discount = total * (100 - Convert.ToInt32(TBDiscount.Text)) / 100;
                    }
                }
            }
            else
            {
                total = 0;
                total_discount = 0;
            }
            TBlockTotalPrice.Text = "Total Price : Rp. " + total;
            TBlockPriceAfterDisc.Text = "Total Price After Discount : Rp. " + total_discount;
        }

        private void ButtonAddToBundle_Click(object sender, RoutedEventArgs e)
        {
            if(CBSelectGame.SelectedIndex >=0)
            {
                ListBoxItem addedGame = new ListBoxItem();
                addedGame.Content = CBSelectGame.SelectedItem.ToString();
                LBBundleGame.Items.Add(addedGame);
                game_id_add_to_bundle.Add(game_id_for_combo[CBSelectGame.SelectedIndex]);

                total = 0;
                query = $"select game_id, price from GAME";
                OracleDataAdapter dag = new OracleDataAdapter(query, conn);
                DataTable dtg = new DataTable();
                dag.Fill(dtg);
                for (int i = 0; i < game_id_add_to_bundle.Count; i++)
                {
                    for (int j = 0; j < dtg.Rows.Count; j++)
                    {
                        if (dtg.Rows[j].ItemArray[0].ToString() == game_id_add_to_bundle[i])
                        {
                            total += Convert.ToInt32(dtg.Rows[j].ItemArray[1].ToString());
                        }
                    }
                }
                total_discount = total;

                if (TBDiscount.Text != "")
                {
                    if (!cek_string(TBDiscount.Text))
                    {
                        MessageBox.Show("Invalid Discount Value!");
                    }
                    else
                    {
                        total_discount = total * (100 - Convert.ToInt32(TBDiscount.Text)) / 100;
                    }
                }
                TBlockTotalPrice.Text = "Total Price : Rp. " + total;
                TBlockPriceAfterDisc.Text = "Total Price After Discount : Rp. " + total_discount;
                //string daftargame;
                //daftargame = "";
                //for (int i = 0; i < game_id_add_to_bundle.Count; i++)
                //{
                //    daftargame = daftargame + game_id_add_to_bundle[i] + " ";
                //}
                //MessageBox.Show(daftargame);
            }
        }

        private void ButtonClearBundle_Click(object sender, RoutedEventArgs e)
        {
            total = 0;
            total_discount = 0;
            TBBundleName.Text = "";
            TBDiscount.Text = "";
            TBlockTotalPrice.Text = "Total Price : -";
            TBlockPriceAfterDisc.Text = "Total Price After Discount : -";
            CBSelectGame.Text = "";
            LBBundleGame.Items.Clear();
            game_id_add_to_bundle.Clear();
        }

        void resetgrid()
        {
            GridHome.Visibility = Visibility.Hidden;
            GridCart.Visibility = Visibility.Hidden;
            GridMasterBundle.Visibility = Visibility.Hidden;
            GridMasterGame.Visibility = Visibility.Hidden;
            GridMasterReport.Visibility = Visibility.Hidden;
        }

        private void ButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            bool valid = checkInputtedData();

            if (valid)
            {
                conn.Open();
                query = "insert into GAME(GAME_ID, DEVELOPER_ID, PUBLISHER_ID, GENRE_ID, NAME, RELEASE_DATE, PRICE, STOCK, IS_ACTIVE_GAME) " +
                        "values(:ID,:DEVELOPER,:PUBLISHER,:GENRE,:TITLE,CURRENT_DATE,:PRICE,:STOCK,:IS_ACTIVE)";
                OracleCommand cmd = new OracleCommand(query, conn);
                try
                {
                    cmd.Parameters.Add(":ID", TBIDGame.Text);
                    cmd.Parameters.Add(":DEVELOPER", devID[CBDeveloper.SelectedIndex]);
                    cmd.Parameters.Add(":PUBLISHER", pubID[CBPublisher.SelectedIndex]);
                    cmd.Parameters.Add(":GENRE", genID[CBGenre.SelectedIndex]);
                    cmd.Parameters.Add(":TITLE", TBTitle.Text);
                    cmd.Parameters.Add(":PRICE", Int32.Parse(TBPrice.Text));
                    cmd.Parameters.Add(":STOCK", Int32.Parse(TBStock.Text));
                    if (rbActive.IsChecked == true)
                    {
                        cmd.Parameters.Add(":IS_ACTIVE", 1);
                    }
                    else
                    {
                        cmd.Parameters.Add(":IS_ACTIVE", 0);
                    }

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                conn.Close();

                if (gambar != null)
                {
                    saveimage(TBIDGame.Text);
                }
                ButtonClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                reloadDG();
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool valid = checkInputtedData();

            if (valid)
            {
                conn.Open();
                int active = 0;
                if (rbActive.IsChecked == true)
                {
                    active = 1;
                }
                string query = $"update GAME set NAME='{TBTitle.Text}', " +
                               $"PRICE={TBPrice.Text}, STOCK={TBStock.Text}, " +
                               $"DEVELOPER_ID='{devID[CBDeveloper.SelectedIndex]}', " +
                               $"PUBLISHER_ID='{pubID[CBPublisher.SelectedIndex]}', " +
                               $"GENRE_ID='{genID[CBGenre.SelectedIndex]}', " +
                               $"IS_ACTIVE_GAME={active} where GAME_ID='{selectedID}'";
                OracleCommand cmd = new OracleCommand(query, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    if (gambar != null)
                    {
                        saveimage(TBIDGame.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                conn.Close();

                ButtonClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                reloadDG();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            // Hanya bisa delete jika stok kosong
            if (selectedStock <= 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    conn.Open();
                    string query = $"delete from GAME where GAME_ID='{selectedID}'";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    conn.Close();

                    ButtonClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    reloadDG();

                    MessageBox.Show("Game Deleted Successfully");
                }
            }
            else
            {
                MessageBox.Show("Can't Delete Game");
            }
        }
    }
}
