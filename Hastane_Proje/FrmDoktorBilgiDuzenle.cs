using System;
using System.Data;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmDoktorBilgiDuzenle : Form
    {
        public string tc;
        public FrmDoktorBilgiDuzenle() { InitializeComponent(); MskTc.ReadOnly = true; TxtSifre.UseSystemPasswordChar = true; CmbBrans.DropDownStyle = ComboBoxStyle.DropDownList; }
        private void FrmDoktorBilgiDuzenle_Load(object sender, EventArgs e)
        {
            MskTc.Text = tc; CmbBrans.Items.Clear();
            foreach (DataRow row in Database.Query("SELECT BransAd FROM Tbl_Branslar ORDER BY BransAd").Rows) CmbBrans.Items.Add(row[0]);
            var rows = Database.Query("SELECT DoktorAd,DoktorSoyad,DoktorBrans FROM Tbl_Doktorlar WHERE DoktorTc=@tc", Database.P("@tc", tc));
            if (rows.Rows.Count == 0) { Close(); return; }
            var doctor = rows.Rows[0]; TxtAd.Text = Convert.ToString(doctor["DoktorAd"]); TxtSoyad.Text = Convert.ToString(doctor["DoktorSoyad"]); CmbBrans.Text = Convert.ToString(doctor["DoktorBrans"]); TxtSifre.Clear();
        }
        private void BtnBilgiGuncelle_Click(object sender, EventArgs e)
        { DoctorService.Update(tc, TxtAd.Text, TxtSoyad.Text, CmbBrans.Text, TxtSifre.Text); MessageBox.Show("Bilgileriniz güncellendi. Boş bırakılan şifre değiştirilmedi."); Close(); }
    }
}
