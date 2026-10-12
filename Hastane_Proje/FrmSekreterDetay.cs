using System;
using System.Data;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmSekreterDetay : Form
    {
        public string stc;
        public string id, tarih, saat, brans, doktor, tc;
        public bool durum;
        public FrmSekreterDetay() { InitializeComponent(); CmbBrans.DropDownStyle = ComboBoxStyle.DropDownList; CmbDoktor.DropDownStyle = ComboBoxStyle.DropDownList; }
        private void FrmSekreterDetay_Load(object sender, EventArgs e)
        {
            LblTc.Text = stc;
            var secretary = Database.Query("SELECT SekreterAdSoyad FROM Tbl_Sekreter WHERE SekreterTc=@tc", Database.P("@tc", stc));
            if (secretary.Rows.Count == 0) { Close(); return; }
            LblAdSoyad.Text = Convert.ToString(secretary.Rows[0][0]);
            dataGridView1.DataSource = Database.Query("SELECT Bransid,BransAd FROM Tbl_Branslar ORDER BY BransAd"); FormTools.Grid(dataGridView1);
            dataGridView2.DataSource = Database.Query("SELECT DoktorAd+' '+DoktorSoyad AS Doktorlar,DoktorBrans FROM Tbl_Doktorlar ORDER BY DoktorAd,DoktorSoyad"); FormTools.Grid(dataGridView2);
            CmbBrans.Items.Clear();
            foreach (DataRow row in Database.Query("SELECT BransAd FROM Tbl_Branslar ORDER BY BransAd").Rows) CmbBrans.Items.Add(row[0]);
            if (!string.IsNullOrEmpty(id)) button1_Click(sender, e);
        }
        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            CmbDoktor.Items.Clear();
            foreach (DataRow row in Database.Query("SELECT DoktorAd+' '+DoktorSoyad FROM Tbl_Doktorlar WHERE DoktorBrans=@brans ORDER BY DoktorAd,DoktorSoyad", Database.P("@brans", CmbBrans.Text)).Rows) CmbDoktor.Items.Add(row[0]);
        }
        private void BtnKaydet_Click(object sender, EventArgs e)
        { AppointmentService.Create(FormTools.Date(MskTarih.Text), FormTools.Time(MskSaat.Text), CmbBrans.Text, CmbDoktor.Text); MessageBox.Show("Randevu saati oluşturuldu."); }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void button1_Click(object sender, EventArgs e)
        { Txtid.Text = id; MskTarih.Text = tarih; MskSaat.Text = saat; CmbBrans.Text = brans; CmbDoktor.Text = doktor; MskTc.Text = tc; ChkDurum.Checked = durum; }
        private void button2_Click(object sender, EventArgs e) { using (var form = new FrmDuyurular()) form.ShowDialog(this); }
        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            int selectedId; if (!int.TryParse(Txtid.Text, out selectedId)) throw new ArgumentException("Randevu listesinden bir kayıt seçin.");
            var identity = ChkDurum.Checked ? FormTools.Identity(MskTc.Text) : null;
            int updated = Database.Execute("IF NOT EXISTS(SELECT 1 FROM Tbl_Doktorlar WHERE DoktorAd+' '+DoktorSoyad=@doctor AND DoktorBrans=@branch) THROW 50001,'Doktor ve branş eşleşmiyor.',1; UPDATE Tbl_Randevular SET RandevuTarih=@date,RandevuSaat=@time,RandevuBrans=@branch,RandevuDoktor=@doctor,RandevuDurum=@status,HastaTc=@tc,HastaSikayet=CASE WHEN @status=0 OR HastaTc<>@tc THEN NULL ELSE HastaSikayet END WHERE Randevuid=@id", Database.P("@date", FormTools.Date(MskTarih.Text)), Database.P("@time", FormTools.Time(MskSaat.Text)), Database.P("@branch", FormTools.Required(CmbBrans.Text, "Branş")), Database.P("@doctor", FormTools.Required(CmbDoktor.Text, "Doktor", 201)), Database.P("@status", ChkDurum.Checked), Database.P("@tc", identity), Database.P("@id", selectedId));
            if (updated != 1) { MessageBox.Show("Randevu kaydı bulunamadı."); return; }
            MessageBox.Show("Randevu güncellendi."); Close();
        }
        private void BtnDuyuruOlustur_Click(object sender, EventArgs e)
        { Database.Execute("INSERT INTO Tbl_Duyurular(Duyuru) VALUES(@text)", Database.P("@text", FormTools.Required(RchDuyuru.Text, "Duyuru", 2000))); RchDuyuru.Clear(); MessageBox.Show("Duyuru oluşturuldu."); }
        private void BtnDoktorPaneli_Click(object sender, EventArgs e) { using (var form = new FrmDoktorPaneli()) form.ShowDialog(this); FrmSekreterDetay_Load(sender, e); }
        private void BtnBransPaneli_Click(object sender, EventArgs e) { using (var form = new FrmBrans()) form.ShowDialog(this); FrmSekreterDetay_Load(sender, e); }
        private void BtnRandevuListe_Click(object sender, EventArgs e) { using (var form = new FrmRandevuListesi { SekreterTc = stc }) form.ShowDialog(this); }
    }
}
