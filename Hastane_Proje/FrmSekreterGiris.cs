using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmSekreterGiris : Form
    {
        public FrmSekreterGiris() { InitializeComponent(); }
        private void BtnGiris_Click(object sender, EventArgs e)
        {
            if (!AccountService.Login(AccountRole.Sekreter, MskTc.Text, TxtSifre.Text))
            { MessageBox.Show("Hatalı kimlik numarası veya şifre."); return; }
            using (var detail = new FrmSekreterDetay { stc = MskTc.Text })
            { Hide(); try { detail.ShowDialog(); } finally { Show(); } }
            Close();
        }
    }
}
