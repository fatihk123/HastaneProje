using System;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace Hastane_Proje
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, args) =>
            {
                var sql = args.Exception as SqlException;
                string message = args.Exception is ArgumentException ? args.Exception.Message : "İşlem tamamlanamadı. Bilgileri ve bağlantı ayarlarını kontrol edin.";
                if (sql != null)
                {
                    if (sql.Number == 2601 || sql.Number == 2627) message = "Bu kayıt zaten mevcut. Kimlik numarası veya randevu saatini kontrol edin.";
                    else if (sql.Number == 547) message = "Bu kayda bağlı başka kayıtlar var. Önce ilgili kayıtları düzenleyin.";
                    else if (sql.Number == 50001) message = "Seçilen doktor ve branş eşleşmiyor.";
                    else message = "Veritabanı işlemi tamamlanamadı. Kurulum ve bağlantı ayarlarını kontrol edin.";
                }
                MessageBox.Show(message, "İşlem tamamlanamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            Application.Run(new FrmGirisler());
        }
    }
}
