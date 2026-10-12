using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Hastane_Proje;

internal static class Checks
{
    static int count;
    static void Check(bool result, string description) { if (!result) throw new Exception("FAILED: " + description); count++; Console.WriteLine("PASS: " + description); }
    static void Throws(Action action, string description) { try { action(); } catch (ArgumentException) { Check(true, description); return; } throw new Exception("FAILED: " + description); }
    [STAThread]
    static int Main(string[] args)
    {
        try
        {
            if (args.Contains("--seed-demo")) { Seed(); return 0; }
            var hash = PasswordHasher.Hash("Demo123!");
            Check(PasswordHasher.Verify("Demo123!", hash), "Correct password accepted");
            Check(!PasswordHasher.Verify("wrong", hash), "Wrong password rejected");
            Check(hash != PasswordHasher.Hash("Demo123!"), "Each password has a random salt");
            Check(!PasswordHasher.Verify("Demo123!", "Demo123!"), "Plaintext password rejected");
            Check(!PasswordHasher.Verify("Demo123!", "PBKDF2$600000$bad$bad"), "Malformed hash rejected");
            Check(!PasswordHasher.Verify("Demo123!", "PBKDF2$2147483647$bad$bad"), "Unbounded iteration count rejected");
            Throws(() => PasswordHasher.Hash("short"), "Short password rejected");
            Check(FormTools.Identity("00000000001") == "00000000001", "Synthetic identity accepted for demo");
            Throws(() => FormTools.Identity("123"), "Short identity rejected");
            Throws(() => FormTools.Identity("abcdefghijk"), "Nonnumeric identity rejected");
            Throws(() => FormTools.Required(" ", "Name"), "Blank required input rejected");
            Throws(() => FormTools.Date("01.01.2020"), "Past appointment date rejected");
            Check(FormTools.Date(DateTime.Today.AddDays(1).ToString("dd.MM.yyyy")) == DateTime.Today.AddDays(1), "Future date parsed");
            Check(FormTools.Time("14:30") == new TimeSpan(14, 30, 0), "Appointment time parsed");
            Throws(() => FormTools.Time("25:00"), "Invalid time rejected");
            Throws(() => AppointmentService.Create(DateTime.Today, TimeSpan.Zero, "Dahiliye", "Demo Doktor"), "Expired time on current day rejected");
            Application.EnableVisualStyles();
            var forms = new Form[] { new FrmGirisler(), new FrmHastaGiris(), new FrmHastaKayit(), new FrmHastaDetay(), new FrmBilgiDuzenle(), new FrmDoktorGiris(), new FrmDoktorDetay(), new FrmDoktorBilgiDuzenle(), new FrmSekreterGiris(), new FrmSekreterDetay(), new FrmDoktorPaneli(), new FrmBrans(), new FrmDuyurular(), new FrmRandevuListesi() };
            foreach (var form in forms) { Check(form.Controls.Count > 0, "Construct " + form.GetType().Name); form.Dispose(); }
            if (args.Contains("--database")) DatabaseChecks();
            if (args.Contains("--render")) Render();
            Console.WriteLine(count + " checks passed."); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
    static void Seed()
    {
        var db = new SqlConnectionStringBuilder(sqlbaglantisi.ConnectionString).InitialCatalog;
        if (!db.StartsWith("HastaneProje", StringComparison.Ordinal)) throw new Exception("Demo seed restricted to HastaneProje databases.");
        if (Convert.ToInt32(Database.Query("SELECT (SELECT COUNT(*) FROM Tbl_Hastalar)+(SELECT COUNT(*) FROM Tbl_Doktorlar)+(SELECT COUNT(*) FROM Tbl_Sekreter)+(SELECT COUNT(*) FROM Tbl_Branslar) AS Total").Rows[0][0]) != 0) { Console.WriteLine("Existing data preserved; seed skipped."); return; }
        var password = PasswordHasher.Hash("Demo123!");
        Database.Execute("SET XACT_ABORT ON;BEGIN TRAN;INSERT INTO Tbl_Branslar(BransAd) VALUES(N'Dahiliye'),(N'Kardiyoloji');INSERT INTO Tbl_Hastalar(HastaAd,HastaSoyad,HastaTc,HastaTelefon,HastaSifre,HastaCinsiyet) VALUES(N'Demo',N'Hasta','00000000001','0000000000',@password,N'Kadın');INSERT INTO Tbl_Doktorlar(DoktorAd,DoktorSoyad,DoktorBrans,DoktorTc,DoktorSifre) VALUES(N'Demo',N'Doktor',N'Dahiliye','00000000002',@password);INSERT INTO Tbl_Sekreter(SekreterAdSoyad,SekreterTc,SekreterSifre) VALUES(N'Demo Sekreter','00000000003',@password);INSERT INTO Tbl_Duyurular(Duyuru) VALUES(N'Bu uygulamadaki hesaplar ve randevular yalnızca demo amaçlıdır.');COMMIT;", Database.P("@password", password));
        AppointmentService.Create(DateTime.Today.AddDays(1), new TimeSpan(14, 0, 0), "Dahiliye", "Demo Doktor");
        AppointmentService.Create(DateTime.Today.AddDays(1), new TimeSpan(14, 30, 0), "Dahiliye", "Demo Doktor");
        Console.WriteLine("Synthetic demo accounts and appointment slots created.");
    }
    static void DatabaseChecks()
    {
        var database = new SqlConnectionStringBuilder(sqlbaglantisi.ConnectionString).InitialCatalog;
        if (database != "HastaneProjeVerification") throw new Exception("Write checks require the dedicated HastaneProjeVerification database.");
        Check(AccountService.Login(AccountRole.Hasta, "00000000001", "Demo123!"), "Patient login");
        Check(AccountService.Login(AccountRole.Doktor, "00000000002", "Demo123!"), "Doctor login");
        Check(AccountService.Login(AccountRole.Sekreter, "00000000003", "Demo123!"), "Secretary login");
        Check(!AccountService.Login(AccountRole.Hasta, "00000000001", "wrong"), "Incorrect patient login");
        Check(Database.Query("SELECT * FROM Tbl_Hastalar WHERE HastaTc=@tc", Database.P("@tc", "' OR 1=1 --")).Rows.Count == 0, "Identity SQL injection rejected");
        var slot = Database.Query("SELECT TOP(1) Randevuid FROM Tbl_Randevular WHERE RandevuDurum=0 ORDER BY Randevuid");
        int id = Convert.ToInt32(slot.Rows[0][0]);
        try
        {
            Check(AppointmentService.Book(id, "00000000001", "Synthetic test complaint"), "Book available appointment");
            Check(!AppointmentService.Book(id, "00000000001", "Second test"), "Second booking rejected atomically");
            var row = Database.Query("SELECT * FROM Tbl_Randevular WHERE Randevuid=@id", Database.P("@id", id)).Rows[0];
            Check(Convert.ToString(row["HastaSikayet"]) == "Synthetic test complaint", "Second booking does not overwrite complaint");
        }
        finally { Database.Execute("UPDATE Tbl_Randevular SET RandevuDurum=0,HastaTc=NULL,HastaSikayet=NULL WHERE Randevuid=@id", Database.P("@id", id)); }
        var before = Database.Query("SELECT DoktorSifre FROM Tbl_Doktorlar WHERE DoktorTc='00000000002'").Rows[0][0];
        DoctorService.Update("00000000002", "Demo", "Updated Doktor", "Dahiliye", "");
        Check(Convert.ToString(before) == Convert.ToString(Database.Query("SELECT DoktorSifre FROM Tbl_Doktorlar WHERE DoktorTc='00000000002'").Rows[0][0]), "Empty profile password preserves hash");
        Check(Database.Query("SELECT * FROM Tbl_Randevular WHERE RandevuDoktor='Demo Updated Doktor'").Rows.Count == 2, "Doctor name change updates appointment ownership");
        DoctorService.Update("00000000002", "Demo", "Doktor", "Dahiliye", "");
        bool duplicate = false;
        try { AppointmentService.Create(DateTime.Today.AddDays(1), new TimeSpan(14, 0, 0), "Dahiliye", "Demo Doktor"); }
        catch (SqlException error) { duplicate = error.Number == 2601 || error.Number == 2627; }
        Check(duplicate, "Duplicate appointment slot rejected");
        bool branchBlocked = false;
        try { Database.Execute("DELETE FROM Tbl_Branslar WHERE BransAd=N'Dahiliye'"); } catch (SqlException error) { branchBlocked = error.Number == 547; }
        Check(branchBlocked, "Branch with doctor and appointments cannot be deleted");
        using (var patient = new FrmHastaDetay { tc = "00000000001" })
        {
            Load(patient, "FrmHastaDetay_Load");
            Check(Field<ComboBox>(patient, "CmbBrans").Items.Count == 2, "Patient branch options load");
            Check(Field<DataGridView>(patient, "dataGridView1").ReadOnly, "Patient history is readonly");
        }
        using (var doctor = new FrmDoktorBilgiDuzenle { tc = "00000000002" })
        {
            Load(doctor, "FrmDoktorBilgiDuzenle_Load");
            Check(Field<TextBox>(doctor, "TxtSifre").Text == "", "Doctor password is never loaded into form");
            Check(Field<ComboBox>(doctor, "CmbBrans").Text == "Dahiliye", "Doctor branch loads by column name");
        }
        using (var secretary = new FrmSekreterDetay { stc = "00000000003", id = Convert.ToString(id), tarih = DateTime.Today.AddDays(1).ToString("dd.MM.yyyy"), saat = "14:00", brans = "Dahiliye", doktor = "Demo Doktor" })
        {
            Load(secretary, "FrmSekreterDetay_Load");
            Check(Field<TextBox>(secretary, "Txtid").Text == Convert.ToString(id), "Appointment edit selection loads before display");
            Check(Field<ComboBox>(secretary, "CmbDoktor").Text == "Demo Doktor", "Selected appointment doctor restored");
        }
        bool missingDoctor = false;
        try { AppointmentService.Create(DateTime.Today.AddDays(2), new TimeSpan(15, 0, 0), "Dahiliye", "Missing Doctor"); } catch (SqlException error) { missingDoctor = error.Number == 50001; }
        Check(missingDoctor, "Unassigned doctor rejected");
    }
    static T Field<T>(Form form, string name) { return (T)form.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(form); }
    static void Load(Form form, string method) { form.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(form, new object[] { form, EventArgs.Empty }); }
    static void Prepare(Control control)
    {
        var handle = control.Handle; foreach (Control child in control.Controls) Prepare(child);
    }
    static void Render()
    {
        var output = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "docs", "screenshots")); Directory.CreateDirectory(output);
        using (var form = new FrmGirisler()) using (var image = new Bitmap(form.Width, form.Height)) { Prepare(form); form.DrawToBitmap(image, new Rectangle(0, 0, form.Width, form.Height)); image.Save(Path.Combine(output, "giris.png")); }
        using (var form = new FrmHastaKayit()) using (var image = new Bitmap(form.Width, form.Height)) { Prepare(form); form.DrawToBitmap(image, new Rectangle(0, 0, form.Width, form.Height)); image.Save(Path.Combine(output, "hasta-kayit.png")); }
    }
}
