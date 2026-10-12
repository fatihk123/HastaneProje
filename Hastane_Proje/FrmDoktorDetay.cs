using System;
using System.Drawing;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmDoktorDetay : Form
    {
        public string tc;
        public FrmDoktorDetay() { InitializeComponent(); }
        private void FrmDoktorDetay_Load(object sender, EventArgs e)
        {
            LblTc.Text = tc; dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            var rows = Database.Query("SELECT DoktorAd,DoktorSoyad FROM Tbl_Doktorlar WHERE DoktorTc=@tc", Database.P("@tc", tc));
            if (rows.Rows.Count == 0) { Close(); return; }
            LblAdSoyad.Text = rows.Rows[0]["DoktorAd"] + " " + rows.Rows[0]["DoktorSoyad"];
            dataGridView1.DataSource = Database.Query("SELECT Randevuid,RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor,RandevuDurum,HastaTc,HastaSikayet FROM Tbl_Randevular WHERE RandevuDoktor=@doctor ORDER BY RandevuTarih,RandevuSaat", Database.P("@doctor", LblAdSoyad.Text));
            FormTools.Grid(dataGridView1);
        }
        private void BtnDuyurular_Click(object sender, EventArgs e) { using (var form = new FrmDuyurular()) form.ShowDialog(this); }
        private void BtnCikis_Click(object sender, EventArgs e) { Close(); }
        private void BtnDuzenle_Click(object sender, EventArgs e) { using (var form = new FrmDoktorBilgiDuzenle { tc = tc }) form.ShowDialog(this); FrmDoktorDetay_Load(sender, e); }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        { if (FormTools.Row(dataGridView1, e)) RchSikayet.Text = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["HastaSikayet"].Value); }
    }
}
