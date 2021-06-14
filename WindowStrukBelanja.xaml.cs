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
        public WindowStrukBelanja(OracleConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
        }
    }
}
