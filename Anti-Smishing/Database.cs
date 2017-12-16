using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;

namespace Anti_Smishing
{
    class Database
    {
        public static string TrueURL;
        private static string sqlinfo = "Server=192.168.144.33;Port=3306;database=antismishing;User Id=App;Password=1123;charset=utf8";
        //db 연결 설정


        //DB에서 URL 값 가져오기
        public static string GetURL(string ScanId)
        {
            //db 통신간에 사용할때 문자 인코딩 방식은 I18N 사용
            new I18N.West.CP1250();
            MySqlConnection antisql = new MySqlConnection(sqlinfo);

            antisql.Open();
            MySqlCommand sqlcomd = new MySqlCommand("select URL from antismishing where ScanId='" + ScanId + "'", antisql);
            TrueURL = sqlcomd.ExecuteScalar().ToString();
            antisql.Close();

            return TrueURL;
        }

        //ScanId 등록하기
        public static void PostScanId(string ScanId)
        {
            new I18N.West.CP1250();

            string temp;
            MySqlConnection antisql = new MySqlConnection(sqlinfo);

            antisql.Open();
            MySqlCommand sqlcomd1 = new MySqlCommand("select ScanId from antismishing where ScanId='" + ScanId + "'", antisql);
            temp = sqlcomd1.ExecuteScalar().ToString();

            if (ScanId==null)
            {
                MySqlCommand sqlcomd2 = new MySqlCommand("insert into antismishing (" + ScanId + ") VALUES('" + ScanId + "')", antisql);
            }
                        
            antisql.Close();

        }
        
    }
}