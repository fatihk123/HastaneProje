using System;
using System.Linq;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmHastaKayit : Form
    {
        public FrmHastaKayit() { InitializeComponent(); TxtSifre.UseSystemPasswordChar = true; CmbCinsiyet.DropDownStyle = ComboBoxStyle.DropDownList; }
        private void BtnKayitYap_Click(object sender, EventArgs e)
        {
            var phone = new string(MskTelNo.Text.Where(char.IsDigit).ToArray());
            if (phone.Length != 10) throw new ArgumentException("Telefon numarası 10 rakam olmalıdır.");
            Database.Execute("INSERT INTO Tbl_Hastalar(HastaAd,HastaSoyad,HastaTc,HastaTelefon,HastaSifre,HastaCinsiyet) VALUES(@ad,@soyad,@tc,@tel,@sifre,@cinsiyet)",
                Database.P("@ad", FormTools.Required(TxtAd.Text, "Ad")), Database.P("@soyad", FormTools.Required(TxtSoyad.Text, "Soyad")), Database.P("@tc", FormTools.Identity(MskTc.Text)), Database.P("@tel", phone), Database.P("@sifre", PasswordHasher.Hash(TxtSifre.Text)), Database.P("@cinsiyet", FormTools.Required(CmbCinsiyet.Text, "Cinsiyet", 20)));
            MessageBox.Show("Kayıt işlemi tamamlandı. Giriş yapabilirsiniz.", "Bilgi");
            Close();
        }
    }
}
