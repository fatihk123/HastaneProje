using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmGirisler : Form
    {
        public FrmGirisler() { InitializeComponent(); }
        private void BtnHastaGiris_Click(object sender, EventArgs e) { using (var form = new FrmHastaGiris()) form.ShowDialog(this); }
        private void BtnSekreterGiris_Click(object sender, EventArgs e) { using (var form = new FrmSekreterGiris()) form.ShowDialog(this); }
        private void BtnDoktorGiris_Click(object sender, EventArgs e) { using (var form = new FrmDoktorGiris()) form.ShowDialog(this); }
    }
}
