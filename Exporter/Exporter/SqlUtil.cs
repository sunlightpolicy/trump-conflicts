using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phase2 {

    public class SqlUtil {

        public static SqlDataReader Query(string sql) {

            SqlDataReader reader = null;
            using (SqlCommand command = new SqlConnection(JsonGenerator.ConnectionString).CreateCommand()) {
                command.CommandText = sql.Replace("''''", "''"); // if they doubled the ticks twice

                try {
                    command.Connection.Open();
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex) {
                    ex.Data.Add("SQL", sql + " SQL ERROR: " + ex.Message);
                    throw ex;
                }
            }
            return reader;
        }

    }
}
