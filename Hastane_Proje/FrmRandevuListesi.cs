using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Hastane_Proje
{
    public partial class FrmRandevuListesi : Form
    {
        public FrmRandevuListesi()
        {
            InitializeComponent();
        }
        sqlbaglantisi bgl = new sqlbaglantisi();

        private void FrmRandevuListesi_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("Select * From Tbl_Randevular", bgl.baglanti());
            dataAdapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;


        }
        public void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
            FrmSekreterDetay frm = new FrmSekreterDetay();
            frm.Show();
            frm.id = dataGridView1.CurrentRow.Cells["RandevuId"].Value.ToString();
            frm.tarih = dataGridView1.CurrentRow.Cells["RandevuTarih"].Value.ToString();
            frm.saat = dataGridView1.CurrentRow.Cells["RandevuSaat"].Value.ToString();
            frm.brans = dataGridView1.CurrentRow.Cells["RandevuBrans"].Value.ToString();
            frm.doktor = dataGridView1.CurrentRow.Cells["RandevuDoktor"].Value.ToString();
            frm.durum = Convert.ToBoolean(dataGridView1.CurrentRow.Cells["RandevuDurum"].Value);

        }

    }
}
