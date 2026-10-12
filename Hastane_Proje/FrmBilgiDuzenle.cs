using System;
using System.Linq;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmBilgiDuzenle : Form
    {
        public string tcno;
        public FrmBilgiDuzenle() { InitializeComponent(); TxtSifre.UseSystemPasswordChar = true; MskTc.ReadOnly = true; CmbCinsiyet.DropDownStyle = ComboBoxStyle.DropDownList; }
        private void FrmBilgiDuzenle_Load(object sender, EventArgs e)
        {
            MskTc.Text = tcno;
            var data = Database.Query("SELECT HastaAd,HastaSoyad,HastaTelefon,HastaCinsiyet FROM Tbl_Hastalar WHERE HastaTc=@tc", Database.P("@tc", tcno));
            if (data.Rows.Count == 0) { Close(); return; }
            var row = data.Rows[0]; TxtAd.Text = Convert.ToString(row["HastaAd"]); TxtSoyad.Text = Convert.ToString(row["HastaSoyad"]); MskTelNo.Text = Convert.ToString(row["HastaTelefon"]); CmbCinsiyet.Text = Convert.ToString(row["HastaCinsiyet"]);
            TxtSifre.Clear();
        }
        private void BtnBilgiGuncelle_Click(object sender, EventArgs e)
        {
            var phone = new string(MskTelNo.Text.Where(char.IsDigit).ToArray());
            if (phone.Length != 10) throw new ArgumentException("Telefon numarası 10 rakam olmalıdır.");
            var newPassword = string.IsNullOrEmpty(TxtSifre.Text) ? null : PasswordHasher.Hash(TxtSifre.Text);
            Database.Execute("UPDATE Tbl_Hastalar SET HastaAd=@ad,HastaSoyad=@soyad,HastaTelefon=@tel,HastaCinsiyet=@cinsiyet,HastaSifre=COALESCE(@sifre,HastaSifre) WHERE HastaTc=@tc", Database.P("@ad", FormTools.Required(TxtAd.Text, "Ad")), Database.P("@soyad", FormTools.Required(TxtSoyad.Text, "Soyad")), Database.P("@tel", phone), Database.P("@cinsiyet", FormTools.Required(CmbCinsiyet.Text, "Cinsiyet", 20)), Database.P("@sifre", newPassword), Database.P("@tc", tcno));
            MessageBox.Show("Bilgileriniz güncellendi. Boş bıraktığınız şifre değiştirilmedi."); Close();
        }
    }
}
