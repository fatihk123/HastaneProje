using System;
namespace Hastane_Proje
{
    public enum AccountRole { Hasta, Doktor, Sekreter }
    public static class AccountService
    {
        public static bool Login(AccountRole role, string identity, string password)
        {
            identity = FormTools.Identity(identity);
            string table, tc, secret;
            switch (role)
            {
                case AccountRole.Hasta: table = "Tbl_Hastalar"; tc = "HastaTc"; secret = "HastaSifre"; break;
                case AccountRole.Doktor: table = "Tbl_Doktorlar"; tc = "DoktorTc"; secret = "DoktorSifre"; break;
                case AccountRole.Sekreter: table = "Tbl_Sekreter"; tc = "SekreterTc"; secret = "SekreterSifre"; break;
                default: throw new ArgumentOutOfRangeException("role");
            }
            var accounts = Database.Query("SELECT " + secret + " FROM " + table + " WHERE " + tc + "=@tc", Database.P("@tc", identity));
            return accounts.Rows.Count == 1 && PasswordHasher.Verify(password, Convert.ToString(accounts.Rows[0][secret]));
        }
    }
}
