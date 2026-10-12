using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmRandevuListesi : Form
    {
        public string SekreterTc;
        public FrmRandevuListesi() { InitializeComponent(); }
        private void FrmRandevuListesi_Load(object sender, EventArgs e)
        { dataGridView1.DataSource = Database.Query("SELECT Randevuid,RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor,RandevuDurum,HastaTc,HastaSikayet FROM Tbl_Randevular ORDER BY RandevuTarih,RandevuSaat"); FormTools.Grid(dataGridView1); }
        public void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FormTools.Row(dataGridView1, e)) return;
            var row = dataGridView1.Rows[e.RowIndex];
            using (var form = new FrmSekreterDetay
            {
                stc = SekreterTc,
                id = Convert.ToString(row.Cells["Randevuid"].Value),
                tarih = Convert.ToDateTime(row.Cells["RandevuTarih"].Value).ToString("dd.MM.yyyy"),
                saat = ((TimeSpan)row.Cells["RandevuSaat"].Value).ToString(@"hh\:mm"),
                brans = Convert.ToString(row.Cells["RandevuBrans"].Value),
                doktor = Convert.ToString(row.Cells["RandevuDoktor"].Value),
                tc = Convert.ToString(row.Cells["HastaTc"].Value),
                durum = Convert.ToBoolean(row.Cells["RandevuDurum"].Value)
            }) form.ShowDialog(this);
            FrmRandevuListesi_Load(sender, e);
        }
    }
}
