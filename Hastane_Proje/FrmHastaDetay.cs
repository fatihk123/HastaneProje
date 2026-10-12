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
using System.Data.Common;


namespace Hastane_Proje
{

    public partial class FrmHastaDetay : Form
    {
        sqlbaglantisi bgln = new sqlbaglantisi();
        public FrmHastaDetay()
        {
            InitializeComponent();
        }
        public string tc;
        private void FrmHastaDetay_Load(object sender, EventArgs e)
        {


            //Ad Soyad Çekme
            LblTc.Text = tc;
            SqlCommand komut = new SqlCommand("Select HastaAd,HastaSoyad From Tbl_Hastalar where HastaTc=@p1", bgln.baglanti());
            komut.Parameters.AddWithValue("@p1", tc);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdSoyad.Text = dr[0] + " " + dr[1];

            }
            bgln.baglanti().Close();

            //Geçmiş Randevu Listesi
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular where HastaTc='" + tc + "'", bgln.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            //Branşları Çekme comboboxa
            SqlCommand komut2 = new SqlCommand("Select BransAd From Tbl_Branslar", bgln.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                CmbBrans.Items.Add(dr2[0]);
            }
            bgln.baglanti().Close();

            //Doktorları Çekme comboboxa
            SqlCommand komut3 = new SqlCommand("Select DoktorAd from Tbl_Doktorlar", bgln.baglanti());
            SqlDataReader dr3 = komut3.ExecuteReader();
            while (dr3.Read())
            {
                CmbDoktor.Items.Add(dr3[0]);
            }


        }

        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            CmbDoktor.Items.Clear();
            SqlCommand komut4 = new SqlCommand("select DoktorAd,DoktorSoyad from Tbl_Doktorlar where DoktorBrans=@p1", bgln.baglanti());
            komut4.Parameters.AddWithValue("@p1", CmbBrans.Text);
            SqlDataReader dr4 = komut4.ExecuteReader();
            while (dr4.Read())
            {
                CmbDoktor.Items.Add(dr4[0] + " " + dr4[1]);

            }
            bgln.baglanti().Close();


        }

        private void CmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular where RandevuBrans='" + CmbBrans.Text + "'" + "and Randevudoktor='"+CmbDoktor.Text+"'and RandevuDurum=0 ", bgln.baglanti());
            da.Fill(dt);
            dataGridView2.DataSource = dt;
            

        }

        private void LnkBilgiDuzenle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmBilgiDuzenle frm = new FrmBilgiDuzenle();
            frm.tcno=LblTc.Text;
            frm.Show();

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int secilen = dataGridView2.SelectedCells[0].RowIndex;
            TxtId.Text = dataGridView2.Rows[secilen].Cells[0].Value.ToString();



        }

        private void BtnRandevuAl_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand("Update Tbl_Randevular set RandevuDurum=1,HastaTc=@p1,HastaSikayet=@p2 where Randevuid=@p3", bgln.baglanti());
            komut.Parameters.AddWithValue("@p1", LblTc.Text);
            komut.Parameters.AddWithValue("@p2", RchSikayet.Text);
            komut.Parameters.AddWithValue("@p3", TxtId.Text);
            komut.ExecuteNonQuery();
            MessageBox.Show("Randevunuz Alınmıştır", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            bgln.baglanti().Close();
            RchSikayet.Clear();
            CmbBrans.Items.Clear();
            CmbDoktor.Items.Clear();
        }
    }
}
