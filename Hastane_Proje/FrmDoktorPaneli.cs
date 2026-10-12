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
    public partial class FrmDoktorPaneli : Form
    {
        public FrmDoktorPaneli()
        {
            InitializeComponent();
        }
        sqlbaglantisi bgl = new sqlbaglantisi();

        private void FrmDoktorPaneli_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Doktorlar", bgl.baglanti());
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            //Branşları ComboBox'a Çekme
            SqlCommand brns = new SqlCommand("Select BransAd from Tbl_Branslar ", bgl.baglanti());
            SqlDataReader dr1 = brns.ExecuteReader();
            while (dr1.Read())
            {
                CmbBrans.Items.Add(dr1[0].ToString());
            }
            bgl.baglanti().Close();




        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            SqlCommand dre = new SqlCommand("insert into Tbl_Doktorlar (DoktorAd,DoktorSoyad,DoktorBrans,DoktorTC,DoktorSifre) values (@d1,@d2,@d3,@d4,@d5)", bgl.baglanti());
            dre.Parameters.AddWithValue("@d1", TxtAd.Text);
            dre.Parameters.AddWithValue("@d2", TxtSoyad.Text);
            dre.Parameters.AddWithValue("@d3", CmbBrans.Text);
            dre.Parameters.AddWithValue("@d4", MskTc.Text);
            dre.Parameters.AddWithValue("@d5", TxtSifre.Text);
            dre.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Doktor Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);




        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            TxtAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            TxtSoyad.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            CmbBrans.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            MskTc.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            TxtSifre.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {

            SqlCommand sil = new SqlCommand("Delete From Tbl_Doktorlar where DoktorTC=@d1", bgl.baglanti());
            sil.Parameters.AddWithValue("@d1", MskTc.Text);
            sil.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Doktor Silindi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);



        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            SqlCommand guncelle = new SqlCommand("Update Tbl_Doktorlar set DoktorAd=@d1,DoktorSoyad=@d2,DoktorBrans=@d3,DoktorSifre=@d4 where DoktorTC=@d5", bgl.baglanti());
            guncelle.Parameters.AddWithValue("@d1", TxtAd.Text);
            guncelle.Parameters.AddWithValue("@d2", TxtSoyad.Text);
            guncelle.Parameters.AddWithValue("@d3", CmbBrans.Text);
            guncelle.Parameters.AddWithValue("@d4", TxtSifre.Text);
            guncelle.Parameters.AddWithValue("@d5", MskTc.Text);
            guncelle.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Doktor Bilgileri Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
