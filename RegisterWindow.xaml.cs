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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        OracleConnection conn;
        public RegisterWindow(OracleConnection conn)
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

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            // banyak if :'v
            if (!string.IsNullOrEmpty(TBFirstName.Text) && !string.IsNullOrEmpty(TBLastName.Text) && !string.IsNullOrEmpty(TBUsername.Text) && !string.IsNullOrEmpty(TBPass.Password) && !string.IsNullOrEmpty(TBConPass.Password) && DPBirthDate.SelectedDate != null)
            {
                if (TBPass.Password.Equals(TBConPass.Password))
                {
                    conn.Open();
                    string query = $"select USERNAME from MEMBER where USERNAME='{TBUsername.Text}' and PASSWORD='{TBPass.Password}'";
                    OracleCommand cmd = new OracleCommand(query, conn);
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        query = $"insert into MEMBER(REAL_NAME, USERNAME, PASSWORD, BIRTH_DATE) " +
                                $"values(:RNAME,:USERNAME,:PASSWORD,TO_DATE(:BIRTHDATE,'DD/MM/YYYY'))";
                        cmd = new OracleCommand(query, conn);
                        try
                        {
                            cmd.Parameters.Add(":RNAME", $"{TBFirstName.Text} {TBLastName.Text}");
                            cmd.Parameters.Add(":USER", TBUsername.Text);
                            cmd.Parameters.Add(":PASS", TBPass.Password);
                            cmd.Parameters.Add(":BIRTHDATE", DPBirthDate.DisplayDate.Date.ToShortDateString());

                            cmd.ExecuteNonQuery();

                            reader.Close();
                            MessageBox.Show("Register Success!");
                            clearInput();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Username already taken");
                        reader.Close();
                    }
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Invalid Confirmation Password");
                }
            }
            else
            {
                MessageBox.Show("Please fill all the fields");
            }
        }

        private void clearInput()
        {
            TBFirstName.Text = "";
            TBLastName.Text = "";
            TBUsername.Text = "";
            TBPass.Password = "";
            TBConPass.Password = "";
            DPBirthDate.SelectedDate = null;
        }
    }
}
