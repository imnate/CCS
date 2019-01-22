using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace OSMVA_UploadHW
{
    public class DBM
    {
        private static SqlConnection sql_conn,db_conn;

        static DBM()
        {
            string Db_main = System.Configuration.ConfigurationManager.ConnectionStrings["DB_main"].ConnectionString;
            sql_conn = new SqlConnection(Db_main);
            string Db_nb = System.Configuration.ConfigurationManager.ConnectionStrings["DB_nb"].ConnectionString;
            db_conn = new SqlConnection(Db_nb);
        }

        public static SqlConnection getconn_main()
        {
            return sql_conn;
        }
        public static SqlConnection getdbconn_nb()
        {
            return db_conn;
        }
       
    }
}