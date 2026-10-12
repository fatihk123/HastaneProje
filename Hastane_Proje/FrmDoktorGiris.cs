using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmDoktorGiris : Form
    {
        public FrmDoktorGiris() { InitializeComponent(); }
        private void BtnGiris_Click(object sender, EventArgs e)
        {
            if (!AccountService.Login(AccountRole.Doktor, MskTc.Text, TxtSifre.Text))
            { MessageBox.Show("Hatalı kimlik numarası veya şifre."); return; }
            using (var detail = new FrmDoktorDetay { tc = MskTc.Text })
            { Hide(); try { detail.ShowDialog(); } finally { Show(); } }
            Close();
        }
    }
}
