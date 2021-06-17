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
    /// Interaction logic for WindowReportTransaksi.xaml
    /// </summary>
    public partial class WindowReportTransaksi : Window
    {
        OracleConnection conn;
        TransaksiReport repTransaksi;
        string dataSource, dataUsername, dataPass;
        DateTime start, end;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReportTransaksi.Owner = Window.GetWindow(this);
        }

        public WindowReportTransaksi(OracleConnection conn, string dataSource, string dataUsername, string dataPass, DateTime start, DateTime end)
        {
            InitializeComponent();
            this.conn = conn;
            this.dataSource = dataSource;
            this.dataUsername = dataUsername;
            this.dataPass = dataPass;
            this.start = start;
            this.end = end;
            repTransaksi = new TransaksiReport();
            repTransaksi.SetDatabaseLogon(dataUsername, dataPass, dataSource, "");
            repTransaksi.SetParameterValue("dateStart", start);
            repTransaksi.SetParameterValue("dateEnd", end);
            ReportTransaksi.ViewerCore.ReportSource = repTransaksi;
        }
    }
}
