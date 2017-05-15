using System.Data.SqlClient;
using System;
using System.Data;
using System.Collections.Generic;

namespace PermanentPricesDAL
{
    public class PermanentPricesConnectedLayer
    {
        private SqlConnection _sqlConnection = null;

        public PermanentPricesConnectedLayer() { }

        public PermanentPricesConnectedLayer(string connectionString)
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

        public void InsertPermanentPrice(string Name, double Price)
        {
            string priceString = Price.ToString();

            string sql = "INSERT INTO PermanentPrices" + $"(Name, Price) VALUES ('{Name}', '{priceString}')";
               
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void InsertPermanentPrice(string Name, double Price, string Group)
        {
            string priceString = Price.ToString();

            string sql = "INSERT INTO PermanentPrices" + $"(Name, Price, [Group]) VALUES ('{Name}', '{priceString}', '{Group}')";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void DeletePermanentPrice(int Id)
        {
            string sql = $"DELETE FROM PermanentPrices WHERE Id = '{Id}'";

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

        public void UpdatePermanentPricePrice(int Id, double NewPrice)
        {
            string newPriceString = NewPrice.ToString();

            string sql = $"UPDATE PermanentPrices SET Price = '{newPriceString}' WHERE Id = {Id}";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void UpdatePermanentPriceAll(int Id, double NewPrice, string Group)
        {
            string newPriceString = NewPrice.ToString();

            string sql = $"UPDATE PermanentPrices SET Price = '{newPriceString}', [Group] = '{Group}' WHERE Id = {Id}";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetAllPermanentPricesAsDataTable()
        {
            DataTable dataTable = new DataTable();

            string sql = "SELECT * FROM PermanentPrices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();

                dataTable.Load(dataReader);
                dataReader.Close();
            }
            return dataTable;
        }

        public List<string> GetAllPermanentPricesNames()
        {
            List<string> list = new List<string>();
           
            string sql = "SELECT * FROM PermanentPrices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    list.Add((string)dataReader["Name"]);
                }
            }

            return list;
        }

        public double LookUpPrice(int id)
        {
            double price = 0;

            using (SqlCommand command = new SqlCommand("GetPriceById", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Value = id,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@price",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                price = double.Parse(((string)command.Parameters["@price"].Value).Replace('.', ','));
            }
            return price;
        }

        public double LookUpPrice(string Name)
        {
            double price = 0;

            using (SqlCommand command = new SqlCommand("GetPriceByName", _sqlConnection))
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
                    ParameterName = "@Price",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                price = double.Parse(((string)command.Parameters["@Price"].Value).Replace('.', ','));
            }

            return price;
        }

        public int LookUpId(string Name)
        {
            int id = -1;

            using (SqlCommand command = new SqlCommand("GetId", _sqlConnection))
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

            using (SqlCommand command = new SqlCommand("GetPermanentGroupByName", _sqlConnection))
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
