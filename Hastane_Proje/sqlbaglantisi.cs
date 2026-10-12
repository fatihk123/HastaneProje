using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Drawing;


namespace Hastane_Proje
{
    public class sqlbaglantisi
    {

        public  SqlConnection baglanti()
        {
            SqlConnection baglan = new SqlConnection("Data Source=Fatih; Initial Catalog=HastaneProje; Integrated Security = True;");

            baglan.Open();
            return baglan;


        }


    }
}
