using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UserDataDAL.ConnectedLayer
{
    public class UserDataConnectedLayer
    {
        private SqlConnection _sqlConnection = null;

        public UserDataConnectedLayer() { }

        public UserDataConnectedLayer(string connectionString)
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
        
        public void UpdateUserData(string Name, double NewValue)
        {
            string sql = $"UPDATE UserData SET Value = '{NewValue}' WHERE Name = '{Name}'";

            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                command.ExecuteNonQuery();
            }             
        }       

        public double LookUpValue(string Name)
        {
            double value = 0;
            string result = null;

            using (SqlCommand command = new SqlCommand("GetUserDataValueByName", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Value = Name,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                param = new SqlParameter()
                {
                    ParameterName = "@Value",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,                    
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                result = (string)command.Parameters["@Value"].Value;                
                value = double.Parse(result);
            }

            return value;
        }        
    }
}
