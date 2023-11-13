using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Configuration;
using System.Xml.Linq;

namespace CookingSite
{
    public class db
    {
        static SqlConnection conn = null;

        static db()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["dbcon"];
            conn = new SqlConnection(settings.ConnectionString);
            conn.Open();
        }

        public static List<string> GetStringList(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            List<string> list = null;
            DataTable dt = FetchDataTable(query, parameters, storedProcedure);
            if (dt.Rows.Count > 0)
            {
                list = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr[0].ToString());
                }
            }
            dt = null;

            return list;
        }

        public static SqlCommand CreateCommand(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            if (storedProcedure)
                cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }
            }
            cmd.CommandTimeout = 0;
            return cmd;
        }

        public static DataTable FetchDataTable(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(CreateCommand(query, parameters, storedProcedure));
            da.Fill(dt);
            da = null;
            return dt;
        }

        public static DataRow FetchDataRow(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            SqlDataAdapter da = new SqlDataAdapter(CreateCommand(query, parameters, storedProcedure));
            da.Fill(dt);
            if (dt.Rows.Count > 0)
                dr = dt.Rows[0];
            da = null;
            return dr;
        }

        public static void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            SqlCommand cmd = CreateCommand(query, parameters, storedProcedure);
            cmd.ExecuteNonQuery();
        }

        public static int ExecuteIntScalar(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            SqlCommand cmd = CreateCommand(query, parameters, storedProcedure);
            int id = (int)cmd.ExecuteScalar();
            return id;
        }

        public static DateTime ExecuteDateTimeScalar(string query, Dictionary<string, object> parameters = null, bool storedProcedure = false)
        {
            SqlCommand cmd = CreateCommand(query, parameters, storedProcedure);
            DateTime date = (DateTime)cmd.ExecuteScalar();
            return date;
        }
    }
}