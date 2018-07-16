using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phase2 {

    public class SqlUtil {

        public SqlDataReader Query(string sql) {

            using (SqlConnection conn = new SqlConnection(JsonGenerator.ConnectionString)) {
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();


                    return cmd.ExecuteReader();
                    //SqlDataReader reader = cmd.ExecuteReader();

                    //while (reader.Read())
                    //    businesses.Add(reader["Name"].ToString(), reader["ID"].ToString());
                }
            }

        }

    }
}
