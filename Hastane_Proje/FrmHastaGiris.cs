using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmHastaGiris : Form
    {
        public FrmHastaGiris() { InitializeComponent(); }
        private void BtnGiris_Click(object sender, EventArgs e)
        {
            if (!AccountService.Login(AccountRole.Hasta, MskTc.Text, TxtSifre.Text))
            { MessageBox.Show("Hatalı kimlik numarası veya şifre."); return; }
            using (var detail = new FrmHastaDetay { tc = MskTc.Text })
            { Hide(); try { detail.ShowDialog(); } finally { Show(); } }
            Close();
        }
        private void LnkUye_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { using (var form = new FrmHastaKayit()) form.ShowDialog(this); }
    }
}
