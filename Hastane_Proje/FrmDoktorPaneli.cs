using System;
using System.Data;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmDoktorPaneli : Form
    {
        public FrmDoktorPaneli() { InitializeComponent(); TxtSifre.UseSystemPasswordChar = true; CmbBrans.DropDownStyle = ComboBoxStyle.DropDownList; }
        private void RefreshRows()
        { dataGridView1.DataSource = Database.Query("SELECT Doktorid,DoktorAd,DoktorSoyad,DoktorBrans,DoktorTc FROM Tbl_Doktorlar ORDER BY DoktorAd,DoktorSoyad"); FormTools.Grid(dataGridView1); }
        private void ClearFields() { TxtAd.Clear(); TxtSoyad.Clear(); MskTc.Clear(); TxtSifre.Clear(); MskTc.ReadOnly = false; }
        private void FrmDoktorPaneli_Load(object sender, EventArgs e)
        { RefreshRows(); CmbBrans.Items.Clear(); foreach (DataRow row in Database.Query("SELECT BransAd FROM Tbl_Branslar ORDER BY BransAd").Rows) CmbBrans.Items.Add(row[0]); }
        private void BtnEkle_Click(object sender, EventArgs e)
        {
            Database.Execute("INSERT INTO Tbl_Doktorlar(DoktorAd,DoktorSoyad,DoktorBrans,DoktorTc,DoktorSifre) VALUES(@ad,@soyad,@brans,@tc,@sifre)", Database.P("@ad", FormTools.Required(TxtAd.Text, "Ad")), Database.P("@soyad", FormTools.Required(TxtSoyad.Text, "Soyad")), Database.P("@brans", FormTools.Required(CmbBrans.Text, "Branş")), Database.P("@tc", FormTools.Identity(MskTc.Text)), Database.P("@sifre", PasswordHasher.Hash(TxtSifre.Text)));
            MessageBox.Show("Doktor eklendi."); ClearFields(); RefreshRows();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!FormTools.Row(dataGridView1, e)) return;
            var row = dataGridView1.Rows[e.RowIndex]; TxtAd.Text = Convert.ToString(row.Cells["DoktorAd"].Value); TxtSoyad.Text = Convert.ToString(row.Cells["DoktorSoyad"].Value); CmbBrans.Text = Convert.ToString(row.Cells["DoktorBrans"].Value); MskTc.Text = Convert.ToString(row.Cells["DoktorTc"].Value); MskTc.ReadOnly = true; TxtSifre.Clear();
        }
        private void BtnSil_Click(object sender, EventArgs e)
        {
            var identity = FormTools.Identity(MskTc.Text);
            int deleted = Database.Execute("DELETE FROM Tbl_Doktorlar WHERE DoktorTc=@tc AND NOT EXISTS (SELECT 1 FROM Tbl_Randevular WHERE RandevuDoktor=DoktorAd+' '+DoktorSoyad)", Database.P("@tc", identity));
            if (deleted != 1) { MessageBox.Show("Doktor bulunamadı veya doktora bağlı randevular var."); return; }
            MessageBox.Show("Doktor silindi."); ClearFields(); RefreshRows();
        }
        private void BtnGuncelle_Click(object sender, EventArgs e)
        { DoctorService.Update(MskTc.Text, TxtAd.Text, TxtSoyad.Text, CmbBrans.Text, TxtSifre.Text); MessageBox.Show("Doktor bilgileri güncellendi."); ClearFields(); RefreshRows(); }
    }
}
