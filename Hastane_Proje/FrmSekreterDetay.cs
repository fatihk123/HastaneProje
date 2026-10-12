using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Hastane_Proje
{
    public partial class FrmSekreterDetay : Form
    {
        public FrmSekreterDetay()
        {
            InitializeComponent();
        }
        sqlbaglantisi bgl = new sqlbaglantisi();
        public string stc;
        public string id,tarih,saat,brans,doktor,tc;
        public bool durum;

        private void FrmSekreterDetay_Load(object sender, EventArgs e)
        {
            LblTc.Text = stc;
            SqlCommand komut = new SqlCommand("Select SekreterAdSoyad from Tbl_Sekreter where SekreterTc=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", LblTc.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdSoyad.Text = dr[0].ToString();
            }
            bgl.baglanti().Close();

            //Branşları DataGrid'e Çekme

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Branslar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            //Doktorları DataGrid'e Çekme
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter("Select (DoktorAd +' '+ DoktorSoyad)as 'Doktorlar' ,DoktorBrans From Tbl_Doktorlar", bgl.baglanti());
            da2.Fill(dt2);
            dataGridView2.DataSource = dt2;


            //Branşları ComboBox'a Çekme
            SqlCommand brns = new SqlCommand("Select BransAd from Tbl_Branslar ", bgl.baglanti());
            SqlDataReader dr1 = brns.ExecuteReader();
            while (dr1.Read())
            {
                CmbBrans.Items.Add(dr1[0].ToString());

            }
            bgl.baglanti().Close();


        }

        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Doktorları ComboBox'a Çekme
            CmbDoktor.Items.Clear();
            SqlCommand doktor = new SqlCommand("Select (DoktorAd+' '+DoktorSoyad) From Tbl_Doktorlar where DoktorBrans=@p1 " , bgl.baglanti());
            doktor.Parameters.AddWithValue("@p1", CmbBrans.Text);
            SqlDataReader dr2 = doktor.ExecuteReader();
            while (dr2.Read())
            {
                CmbDoktor.Items.Add(dr2[0].ToString());


            }
            bgl.baglanti().Close();

        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {

            SqlCommand komutkaydet = new SqlCommand("Insert Into Tbl_Randevular (RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor) Values (@p1,@p2,@p3,@p4)", bgl.baglanti());
            komutkaydet.Parameters.AddWithValue("@p1", MskTarih.Text);
            komutkaydet.Parameters.AddWithValue("@p2", MskSaat.Text);
            komutkaydet.Parameters.AddWithValue("@p3", CmbBrans.Text);
            komutkaydet.Parameters.AddWithValue("@p4", CmbDoktor.Text);
            komutkaydet.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Randevu Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Txtid.Text = id;
            MskTarih.Text = tarih;
            MskSaat.Text = saat;
            CmbBrans.Text = brans;
            CmbDoktor.Text = doktor;
            MskTc.Text = tc;
            ChkDurum.Checked = durum;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmDuyurular frd = new FrmDuyurular();
            frd.Show();
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {

            SqlCommand komutguncelle = new SqlCommand("Update Tbl_Randevular set RandevuTarih=@p1,RandevuSaat=@p2,RandevuBrans=@p3,RandevuDoktor=@p4,RandevuDurum=@p5 where Randevuid=@p6", bgl.baglanti());
            komutguncelle.Parameters.AddWithValue("@p1", MskTarih.Text);
            komutguncelle.Parameters.AddWithValue("@p2", MskSaat.Text);
            komutguncelle.Parameters.AddWithValue("@p3", CmbBrans.Text);
            komutguncelle.Parameters.AddWithValue("@p4", CmbDoktor.Text);
            komutguncelle.Parameters.AddWithValue("@p5", ChkDurum.Checked);
            komutguncelle.Parameters.AddWithValue("@p6", Txtid.Text);
            komutguncelle.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Randevu Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MskTarih.Clear();
            MskSaat.Clear();
            CmbBrans.Text = "";
            CmbDoktor.Text = "";
            MskTc.Clear();
            ChkDurum.Checked = false;
            Txtid.Clear();
            CmbBrans.Items.Clear();
            CmbDoktor.Items.Clear();


        }

        private void BtnDuyuruOlustur_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("Insert Into Tbl_Duyurular (Duyuru) Values (@p1)", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", RchDuyuru.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Duyuru Oluşturuldu", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RchDuyuru.Clear();

        }

        private void BtnDoktorPaneli_Click(object sender, EventArgs e)
        {
            FrmDoktorPaneli drp = new FrmDoktorPaneli();
            drp.Show();

        }

        private void BtnBransPaneli_Click(object sender, EventArgs e)
        {
            FrmBrans frmBrans = new FrmBrans();
            frmBrans.Show();

        }

        private void BtnRandevuListe_Click(object sender, EventArgs e)
        {
            FrmRandevuListesi frmRandevuListesi = new FrmRandevuListesi();
            frmRandevuListesi.Show();

        }
    }
}
