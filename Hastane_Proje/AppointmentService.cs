using System;
namespace Hastane_Proje
{
    public static class AppointmentService
    {
        public static bool Book(int id, string identity, string complaint)
        {
            identity = FormTools.Identity(identity);
            complaint = FormTools.Required(complaint, "Şikayet", 2000);
            return Database.Execute("UPDATE Tbl_Randevular SET RandevuDurum=1,HastaTc=@tc,HastaSikayet=@complaint WHERE Randevuid=@id AND RandevuDurum=0 AND (RandevuTarih>CONVERT(date,GETDATE()) OR (RandevuTarih=CONVERT(date,GETDATE()) AND RandevuSaat>CONVERT(time,GETDATE())))", Database.P("@tc", identity), Database.P("@complaint", complaint), Database.P("@id", id)) == 1;
        }
        public static void Create(DateTime date, TimeSpan time, string branch, string doctor)
        {
            if (time < TimeSpan.Zero || time.TotalHours >= 24 || date.Date.Add(time) <= DateTime.Now) throw new ArgumentException("Randevu tarih ve saatini kontrol edin.");
            branch = FormTools.Required(branch, "Branş"); doctor = FormTools.Required(doctor, "Doktor", 201);
            Database.Execute("IF NOT EXISTS (SELECT 1 FROM Tbl_Doktorlar WHERE DoktorAd+' '+DoktorSoyad=@doctor AND DoktorBrans=@branch) THROW 50001, 'Doktor ve branş eşleşmiyor.', 1; INSERT INTO Tbl_Randevular(RandevuTarih,RandevuSaat,RandevuBrans,RandevuDoktor,RandevuDurum) VALUES(@date,@time,@branch,@doctor,0)", Database.P("@date", date.Date), Database.P("@time", time), Database.P("@branch", branch), Database.P("@doctor", doctor));
        }
    }
}
