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
    /// Interaction logic for WindowReportMember.xaml
    /// </summary>
    public partial class WindowReportMember : Window
    {
        OracleConnection conn;
        string dataSource, dataUsername, dataPass, member;
        MemberReport repMember;
        private void ReportMember_Loaded(object sender, RoutedEventArgs e)
        {
            ReportMember.Owner = Window.GetWindow(this);
        }
        public WindowReportMember(OracleConnection conn, string dataSource, string dataUsername, string dataPass, string member)
        {
            InitializeComponent();
            this.conn = conn;
            this.dataSource = dataSource;
            this.dataUsername = dataUsername;
            this.dataPass = dataPass;
            this.member = member;
            repMember = new MemberReport();
            repMember.SetDatabaseLogon(dataUsername, dataPass, dataSource, "");
            repMember.SetParameterValue("idmember", member);
            ReportMember.ViewerCore.ReportSource = repMember;
        }

        
    }
}
