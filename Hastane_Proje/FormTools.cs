using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
namespace Hastane_Proje
{
    public static class FormTools
    {
        public static string Identity(string text)
        {
            text = (text ?? "").Trim();
            if (text.Length != 11 || !text.All(c => c >= '0' && c <= '9')) throw new ArgumentException("Kimlik numarası 11 rakam olmalıdır.");
            return text;
        }
        public static string Required(string text, string label, int maxLength = 100)
        {
            text = (text ?? "").Trim();
            if (text.Length == 0 || text.Length > maxLength) throw new ArgumentException(label + " alanını kontrol edin.");
            return text;
        }
        public static DateTime Date(string text)
        {
            DateTime result;
            if (!DateTime.TryParse(text, CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out result) || result.Date < DateTime.Today)
                throw new ArgumentException("Geçerli ve geçmişte olmayan bir tarih seçin.");
            return result.Date;
        }
        public static TimeSpan Time(string text)
        {
            TimeSpan result;
            if (!TimeSpan.TryParseExact(text, @"hh\:mm", CultureInfo.InvariantCulture, out result) || result.TotalHours >= 24)
                throw new ArgumentException("Geçerli bir saat girin (örn. 14:30).");
            return result;
        }
        public static void Grid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            if (grid.Columns.Contains("RandevuTarih")) grid.Columns["RandevuTarih"].DefaultCellStyle.Format = "dd.MM.yyyy";
            if (grid.Columns.Contains("RandevuSaat")) grid.Columns["RandevuSaat"].DefaultCellStyle.Format = @"hh\:mm";
            foreach (DataGridViewColumn column in grid.Columns)
                if (column.Name.EndsWith("Sifre", StringComparison.OrdinalIgnoreCase)) column.Visible = false;
        }
        public static bool Row(DataGridView grid, DataGridViewCellEventArgs e) { return e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count && !grid.Rows[e.RowIndex].IsNewRow; }
    }
}
