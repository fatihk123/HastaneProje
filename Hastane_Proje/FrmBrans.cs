using System;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public partial class FrmBrans : Form
    {
        public FrmBrans() { InitializeComponent(); }
        private void RefreshRows() { dataGridView1.DataSource = Database.Query("SELECT Bransid,BransAd FROM Tbl_Branslar ORDER BY BransAd"); FormTools.Grid(dataGridView1); }
        private void FrmBrans_Load(object sender, EventArgs e) { RefreshRows(); }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        { if (FormTools.Row(dataGridView1, e)) { Txtid.Text = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["Bransid"].Value); TxtBrans.Text = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells["BransAd"].Value); } }
        private int SelectedId() { int id; if (!int.TryParse(Txtid.Text, out id)) throw new ArgumentException("Listeden bir branş seçin."); return id; }
        private void BtnEkle_Click(object sender, EventArgs e)
        { Database.Execute("INSERT INTO Tbl_Branslar(BransAd) VALUES(@name)", Database.P("@name", FormTools.Required(TxtBrans.Text, "Branş"))); RefreshRows(); MessageBox.Show("Branş eklendi."); }
        private void BtnSil_Click(object sender, EventArgs e)
        { var count = Database.Execute("DELETE FROM Tbl_Branslar WHERE Bransid=@id", Database.P("@id", SelectedId())); if (count == 1) { RefreshRows(); Txtid.Clear(); TxtBrans.Clear(); MessageBox.Show("Branş silindi."); } }
        private void BtnGuncelle_Click(object sender, EventArgs e)
        { Database.Execute("UPDATE Tbl_Branslar SET BransAd=@name WHERE Bransid=@id", Database.P("@name", FormTools.Required(TxtBrans.Text, "Branş")), Database.P("@id", SelectedId())); RefreshRows(); MessageBox.Show("Branş güncellendi."); }
    }
}
