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
    public partial class FrmBilgiDuzenle : Form
    {
        public FrmBilgiDuzenle()
        {
            InitializeComponent();
        }
        sqlbaglantisi bgl = new sqlbaglantisi();
        public string tcno;



        private void FrmBilgiDuzenle_Load(object sender, EventArgs e)
        {

            MskTc.Text = tcno;
            SqlCommand komut = new SqlCommand("Select * From Tbl_Hastalar where HastaTc=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1",tcno);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())

            {
                TxtAd.Text = dr[1].ToString();
                TxtSoyad.Text = dr[2].ToString();
                MskTelNo.Text = dr[4].ToString();
                TxtSifre.Text = dr[5].ToString();
                CmbCinsiyet.Text = dr[6].ToString();


            }
            bgl.baglanti().Close();

        }
        private void BtnBilgiGuncelle_Click(object sender, EventArgs e)
        {

            SqlCommand cmd = new SqlCommand("UPDATE Tbl_Hastalar SET HastaAd = @p1, HastaSoyad = @p2, HastaTelefon = @p3, HastaSifre = @p4, HastaCinsiyet = @p5 WHERE HastaTc =@p6",bgl.baglanti());
            cmd.Parameters.AddWithValue("@p1", TxtAd.Text);
            cmd.Parameters.AddWithValue("@p2", TxtSoyad.Text);
            cmd.Parameters.AddWithValue("@p3", MskTelNo.Text);
            cmd.Parameters.AddWithValue("@p4", TxtSifre.Text);
            cmd.Parameters.AddWithValue("@p5", CmbCinsiyet.Text);
            cmd.Parameters.AddWithValue("p6", MskTc.Text);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Bilgileriniz Güncellendi","İşlem Başarılı",MessageBoxButtons.OK,MessageBoxIcon.Information);




        }




    }
}
