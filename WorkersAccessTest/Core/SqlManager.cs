using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WorkersAccessTest.Core {
    public static class SqlManager {
        private const string ConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

        private static SqlConnection _sqlConnection;

        private static bool IsConnected() {
            try {
                _sqlConnection = new SqlConnection(ConnectionString);
            }
            catch (Exception e) {
                return false;
            }

            return true;
        }

        public static List<List<string>> ExecuteAndGet(string command) {
            if (!IsConnected())
                return null;
            List<List<string>> table = new List<List<string>>();
            using (_sqlConnection = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    CommandText = command,
                    Connection = _sqlConnection
                };
                _sqlConnection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    table.Add(new List<string>());
                    for (int i = 0; i < reader.FieldCount; i++) {
                        table[table.Count - 1].Add(reader.GetValue(i).ToString());
                    }
                }

                _sqlConnection.Close();
            }

            return table;
        }

        public static void Execute(string command) {
            if (!IsConnected())
                return;
            using (_sqlConnection = new SqlConnection(ConnectionString)) {
                SqlCommand cmd = new SqlCommand {
                    CommandText = command,
                    Connection = _sqlConnection
                };
                _sqlConnection.Open();
                cmd.ExecuteNonQuery();
                _sqlConnection.Close();
            }
        }
    }
}