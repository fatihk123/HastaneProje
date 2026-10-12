using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmDuyurular : Form
    {
        public FrmDuyurular() { InitializeComponent(); }
        private void FrmDuyurular_Load(object sender, EventArgs e)
        { dataGridView1.DataSource = Database.Query("SELECT Duyuruid,Duyuru FROM Tbl_Duyurular ORDER BY Duyuruid DESC"); FormTools.Grid(dataGridView1); }
    }
}
