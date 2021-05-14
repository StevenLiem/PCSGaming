using Microsoft.Win32;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.IO;

namespace PCS_Gaming
{
    /// <summary>
    /// Interaction logic for MasterGame.xaml
    /// </summary>
    public partial class MasterGame : Window
    {
        OracleConnection conn;
        DataTable dtGame;
        List<string> devID, pubID, genID;
        string query, selectedID;
        string imageFolderPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 10) + "Images\\";
        int selectedStock;
        BitmapImage gambar;
        public MasterGame(OracleConnection conn)
        {
            InitializeComponent();

            this.conn = conn;

            fillComboBox();
            reloadDG();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
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


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //cek apakah ada data yg lagi dipilih
            if (!string.IsNullOrEmpty(selectedID))
            {
                return;
            }

            // brute force :v dari tugas praktikum
            // mending nanti bikin jadi function di oracle
            if (!string.IsNullOrWhiteSpace(TBTitle.Text) && TBTitle.Text.Length >= 2)
            {
                string init = "";
                if (TBTitle.Text.IndexOf(" ") > 0 && TBTitle.Text.IndexOf(" ") < TBTitle.Text.Length - 1)
                {
                    init = TBTitle.Text.Substring(0, 1) + TBTitle.Text.Substring(TBTitle.Text.IndexOf(" ") + 1, 1);
                }
                else
                {
                    init = TBTitle.Text.Substring(0, 2);
                }
                init = init.ToUpper();

                conn.Open();
                query = $"select max(GAME_ID) from GAME where GAME_ID like '{init}%'";
                OracleCommand cmd = new OracleCommand(query, conn);
                try
                {
                    string ctr = cmd.ExecuteScalar().ToString();
                    if (string.IsNullOrEmpty(ctr))
                    {
                        init += "001";
                    }
                    else
                    {
                        string ctr2 = Int32.Parse(ctr.Substring(2, 3).TrimStart('0')) + 1 + "";
                        init += ctr2.PadLeft(3, '0');
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                conn.Close();

                TBIDGame.Text = init;
            }
            else
            {
                TBIDGame.Text = "";
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
                               $"GENRE_ID='{genID[CBGenre.SelectedIndex]}', "+
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

                if(row["Is Active"].ToString() == "1")
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
                catch (FileNotFoundException f)
                {
                    imagePreview.Source = null;
                }
                
            }

            ButtonClear.Content = "Cancel";
            ButtonClear.ToolTip = "Cancel Selection";
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
                    if (rbActive.IsChecked==true)
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

        void saveimage(string namagambar)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(gambar));
            using (var fileStream = new System.IO.FileStream(imageFolderPath + namagambar + ".png", System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }

        }

    }
}
