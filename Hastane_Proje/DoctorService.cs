using System;
namespace Hastane_Proje
{
    public static class DoctorService
    {
        public static void Update(string identity, string firstName, string lastName, string branch, string newPassword)
        {
            identity = FormTools.Identity(identity); firstName = FormTools.Required(firstName, "Ad"); lastName = FormTools.Required(lastName, "Soyad"); branch = FormTools.Required(branch, "Branş");
            var hash = string.IsNullOrEmpty(newPassword) ? null : PasswordHasher.Hash(newPassword);
            Database.Execute("SET XACT_ABORT ON; BEGIN TRANSACTION; DECLARE @old nvarchar(201); SELECT @old=DoktorAd+' '+DoktorSoyad FROM Tbl_Doktorlar WHERE DoktorTc=@tc; IF @old IS NULL BEGIN ROLLBACK; THROW 50002, 'Doktor kaydı bulunamadı.', 1; END; UPDATE Tbl_Doktorlar SET DoktorAd=@ad,DoktorSoyad=@soyad,DoktorBrans=@brans,DoktorSifre=COALESCE(@sifre,DoktorSifre) WHERE DoktorTc=@tc; UPDATE Tbl_Randevular SET RandevuDoktor=@ad+' '+@soyad,RandevuBrans=@brans WHERE RandevuDoktor=@old; COMMIT;", Database.P("@tc", identity), Database.P("@ad", firstName), Database.P("@soyad", lastName), Database.P("@brans", branch), Database.P("@sifre", hash));
        }
    }
}
