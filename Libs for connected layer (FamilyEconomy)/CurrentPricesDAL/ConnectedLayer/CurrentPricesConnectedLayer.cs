using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrentPricesDAL.ConnectedLayer
{
    public class CurrentPricesConnectedLayer
    {
        private SqlConnection _sqlConnection = null;

        public CurrentPricesConnectedLayer() { }

        public CurrentPricesConnectedLayer(string connectionString)
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

        public void InsertCurrentMonthlyPrice (int Id_monthly_prices, double Price)
        {
            string priceString = Price.ToString();

            string sql = "INSERT INTO CurrentPrices" + $"(Id_monthly_prices, Price, Check_box) VALUES ('{Id_monthly_prices}', '{priceString}', 0)";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void InsertCurrentPermanentPrice(int Id_permanent_prices, double Price)
        {
            string priceString = Price.ToString();

            string sql = "INSERT INTO CurrentPrices" + $"(Id_permanent_prices, Price, Check_box) VALUES ('{Id_permanent_prices}', '{priceString}', 0)";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }        

        public void DeleteCurrentPrice(int Id)
        {
            string sql = $"DELETE FROM CurrentPrices WHERE Id = '{Id}'";

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

        public void DeleteCurrentPricesAll()
        {
            string sql = $"DELETE FROM CurrentPrices";

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

        public void AddAllPermanentPricesAfterDelete()
        {
           
            string sql = "INSERT INTO CurrentPrices" + $"(Id_permanent_prices, Price) SELECT Id, Price FROM PermanentPrices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCurrentPricePermanentPrice(int Id, double NewPrice)
        {
            string newPriceString = NewPrice.ToString();

            string sql = $"UPDATE CurrentPrices SET Price = '{newPriceString}' WHERE Id_permanent_prices = {Id}";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCurrentPriceMonthlyPrice(int Id, double NewPrice)
        {
            string newPriceString = NewPrice.ToString();

            string sql = $"UPDATE CurrentPrices SET Price = '{newPriceString}' WHERE Id_monthly_prices = {Id}";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCurrentPriceCheckBox(int Id, int Check_box)
        {
            string sql = $"UPDATE CurrentPrices SET Check_box = '{Check_box}' WHERE Id = {Id}";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void UpdateCurrentPriceCheckBoxByName(string Name, int Check_box)
        {
            int id = 0;

            try
            {
                id = LookUpIdMonthly(Name);
            }
            catch (InvalidCastException ex)
            {
                id = LookUpIdPermanent(Name);
            }

            UpdateCurrentPriceCheckBox(id, Check_box);
        }

        public DataTable GetAllCurrentPricesAsDataTable()
        {
            DataTable dataTable = new DataTable();

            string sql = "SELECT Check_box, PermanentPrices.Name, CurrentPrices.Price FROM CurrentPrices INNER JOIN PermanentPrices ON PermanentPrices.Id = CurrentPrices.Id_permanent_prices UNION SELECT Check_box, MonthlyPrices.Name, Price FROM CurrentPrices INNER JOIN MonthlyPrices ON MonthlyPrices.Id = CurrentPrices.Id_monthly_prices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();

                dataTable.Load(dataReader);
                dataReader.Close();
            }
            return dataTable;
        }

        public DataTable GetMonthlyCurrentPricesAsDataTable()
        {
            DataTable dataTable = new DataTable();

            string sql = "SELECT Check_box, MonthlyPrices.Name, Price FROM CurrentPrices INNER JOIN MonthlyPrices ON MonthlyPrices.Id = CurrentPrices.Id_monthly_prices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();

                dataTable.Load(dataReader);
                dataReader.Close();
            }
            return dataTable;
        }

        public DataTable GetPermanentCurrentPricesAsDataTable()
        {
            DataTable dataTable = new DataTable();

            string sql = "SELECT Check_box, PermanentPrices.Name, CurrentPrices.Price FROM CurrentPrices INNER JOIN PermanentPrices ON PermanentPrices.Id = CurrentPrices.Id_permanent_prices";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();

                dataTable.Load(dataReader);
                dataReader.Close();
            }
            return dataTable;
        }

        public int LookUpIdPermanent(string Name)
        {           
            int id = -1;

            using (SqlCommand command = new SqlCommand("GetCurrentIdPermanent", _sqlConnection))
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

        public int LookUpIdMonthly(string Name)
        {
            int id = -1;

            using (SqlCommand command = new SqlCommand("GetCurrentIdMonthly", _sqlConnection))
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

        public double LookUpPrice(int Id)
        {
            double price = 0;

            using (SqlCommand command = new SqlCommand("GetCurrentPriceById", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Value = Id,
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

        public int LookUpPermanentId(int Id)
        {
            int idToReturn = -1;

            using (SqlCommand command = new SqlCommand("GetCurrentPermanentId", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Value = Id,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@Id_permanent_prices",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                idToReturn = (int)command.Parameters["@Id_permanent_prices"].Value;
            }

            return idToReturn;
        }

        public int LookUpMonthlyId(int Id)
        {
            int idToReturn = -1;

            using (SqlCommand command = new SqlCommand("GetCurrentMonthlyId", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Value = Id,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@Id_monthly_prices",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                idToReturn = (int)command.Parameters["@Id_monthly_prices"].Value;
            }

            return idToReturn;
        }

        public int LookUpCheckBox(int Id)
        {
            int inv = 0;

            string sql = $"Select Check_box From CurrentPrices where Id = {Id}";
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    inv = (int)dataReader["Check_box"];
                }
                dataReader.Close();
            }
            return inv;
        }
    }
}
