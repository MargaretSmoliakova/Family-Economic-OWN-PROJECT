using System;
using System.Data;
using System.Data.SqlClient;

namespace MonthlyPricesDAL.ConnectedLayer
{
    public class MonthlyPricesConnectedLayer
    {
        private SqlConnection _sqlConnection = null;

        public MonthlyPricesConnectedLayer() { }

        public MonthlyPricesConnectedLayer(string connectionString)
        {
            OpenConnection(connectionString);
        }

        public void OpenConnection(string connectionString)
        {
            _sqlConnection = new SqlConnection { ConnectionString = connectionString };
            _sqlConnection.Open();
        }

        public void CloseConnection()
        {
            _sqlConnection.Close();
        }       
        
        public void InsertMonthlyPrice(string Name, string Group)
        {
            string sql = "INSERT INTO MonthlyPrices" + $"(Name, [Group]) VALUES ('{Name}', '{Group}')";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void DeleteMonthlyPrice(int Id)
        {
            string sql = $"DELETE FROM MonthlyPrices WHERE Id = '{Id}'";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Извините, сейчас этот объект используется...");
                }
            }
        }

        public void DeleteMonthlyPricesAll()
        {
            string sql = $"DELETE FROM MonthlyPrices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Извините, сейчас этот объект используется...");
                }
            }
        }
        
        public void UpdateMonthlyPriceAll(int Id, string Group)
        {
            string sql = $"UPDATE MonthlyPrices SET [Group] = '{Group}' WHERE Id = '{Id}'";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }
        
        public DataTable GetAllMonthlyPricesAsDataTable()
        {
            DataTable dataTable = new DataTable();

            string sql = "SELECT * FROM MonthlyPrices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();

                dataTable.Load(dataReader);
                dataReader.Close();
            }
            return dataTable;
        }        

        public int LookUpId(string Name)
        {
            int id = -1;

            using (SqlCommand command = new SqlCommand("GetMonthlyPriceId", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 100,
                    Value = Name,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                id = (int)command.Parameters["@Id"].Value;
            }
            return id;
        }

        public string LookUpGroup(string Name)
        {
            string group = null;

            using (SqlCommand command = new SqlCommand("GetMonthlyGroupByName", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 100,
                    Value = Name,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@Group",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                group = (string)command.Parameters["@Group"].Value;
            }

            return group;
        }
    }
}
