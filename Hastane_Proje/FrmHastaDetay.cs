using System;
using System.Data;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmHastaDetay : Form
    {
        public string tc;
        public FrmHastaDetay() { InitializeComponent(); }
        private void History()
        {
            dataGridView1.DataSource = Database.Query("SELECT Randevuid,RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor,HastaSikayet FROM Tbl_Randevular WHERE HastaTc=@tc ORDER BY RandevuTarih DESC,RandevuSaat", Database.P("@tc", tc));
            FormTools.Grid(dataGridView1);
        }
        private void Available()
        {
            dataGridView2.DataSource = Database.Query("SELECT Randevuid,RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor FROM Tbl_Randevular WHERE RandevuBrans=@brans AND RandevuDoktor=@doktor AND RandevuDurum=0 AND (RandevuTarih>CONVERT(date,GETDATE()) OR (RandevuTarih=CONVERT(date,GETDATE()) AND RandevuSaat>CONVERT(time,GETDATE()))) ORDER BY RandevuTarih,RandevuSaat", Database.P("@brans", CmbBrans.Text), Database.P("@doktor", CmbDoktor.Text));
            FormTools.Grid(dataGridView2);
            TxtId.Clear();
        }
        private void FrmHastaDetay_Load(object sender, EventArgs e)
        {
            LblTc.Text = tc;
            var patient = Database.Query("SELECT HastaAd,HastaSoyad FROM Tbl_Hastalar WHERE HastaTc=@tc", Database.P("@tc", tc));
            if (patient.Rows.Count == 0) { Close(); return; }
            LblAdSoyad.Text = patient.Rows[0]["HastaAd"] + " " + patient.Rows[0]["HastaSoyad"];
            History();
            CmbBrans.DropDownStyle = ComboBoxStyle.DropDownList; CmbDoktor.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbBrans.Items.Clear();
            foreach (DataRow row in Database.Query("SELECT BransAd FROM Tbl_Branslar ORDER BY BransAd").Rows) CmbBrans.Items.Add(row[0]);
        }
        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            CmbDoktor.Items.Clear(); CmbDoktor.SelectedIndex = -1;
            foreach (DataRow row in Database.Query("SELECT DoktorAd+' '+DoktorSoyad FROM Tbl_Doktorlar WHERE DoktorBrans=@brans ORDER BY DoktorAd,DoktorSoyad", Database.P("@brans", CmbBrans.Text)).Rows) CmbDoktor.Items.Add(row[0]);
            Available();
        }
        private void CmbDoktor_SelectedIndexChanged(object sender, EventArgs e) { Available(); }
        private void LnkBilgiDuzenle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { using (var form = new FrmBilgiDuzenle { tcno = tc }) form.ShowDialog(this); FrmHastaDetay_Load(sender, e); }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        { if (FormTools.Row(dataGridView2, e)) TxtId.Text = Convert.ToString(dataGridView2.Rows[e.RowIndex].Cells["Randevuid"].Value); }
        private void BtnRandevuAl_Click(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(TxtId.Text, out id)) throw new ArgumentException("Listeden bir randevu seçin.");
            if (!AppointmentService.Book(id, tc, RchSikayet.Text)) { MessageBox.Show("Bu randevu artık uygun değil. Güncel listeden başka bir saat seçin."); Available(); return; }
            MessageBox.Show("Randevunuz alındı.", "Bilgi");
            RchSikayet.Clear(); History(); Available();
        }
    }
}
