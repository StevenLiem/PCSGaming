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
using Oracle.DataAccess.Client;

namespace PCS_Gaming
{
    /// <summary>
    /// Interaction logic for WindowStrukBelanja.xaml
    /// </summary>
    public partial class WindowStrukBelanja : Window
    {
        OracleConnection conn;
        StrukBelanja belanja;
        string dataSource, dataUsername, dataPass, token;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReportStrukBelanja.Owner = Window.GetWindow(this);
        }

        public WindowStrukBelanja(OracleConnection conn, string dataSource, string dataUsername, string dataPass, string token)
        {
            InitializeComponent();
            this.conn = conn;
            this.dataSource = dataSource;
            this.dataUsername = dataUsername;
            this.dataPass = dataPass;
            this.token = token;
            belanja = new StrukBelanja();
            belanja.SetDatabaseLogon(dataUsername, dataPass, dataSource,"");
            belanja.SetParameterValue("ParamTokenTransaksi", token);
            ReportStrukBelanja.ViewerCore.ReportSource = belanja;
        }
    }
}
