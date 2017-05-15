using System;
using ClassLibraryServices.ServiceContracts;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.Data;

namespace ClassLibraryServices.Implementations
{
    public class UserDatabaseCreation : IUserDatabaseCreation
    {
        private string serverName = @"PC\SQLEXPRESS";

        public string MethodForTest()
        {
            return "test completed!";
        }

        public void CreateUserDataBase(string loginName)
        {            
            Server sourceServer = new Server(serverName);
            Server destinationServer = new Server(serverName);
            sourceServer.ConnectionContext.LoginSecure = true;
            sourceServer.ConnectionContext.Connect();
            destinationServer.ConnectionContext.LoginSecure = true;
            destinationServer.ConnectionContext.Connect();
            Database templateDatabase = sourceServer.Databases["FamilyEconomic_db"];
            Database newDatabase = new Database(destinationServer, loginName);
            //newDatabase.Create();

            // TRANSFER DATABASE


            Transfer tr = new Transfer(templateDatabase);
            tr.CopyAllUsers = true;
            tr.DestinationServer = destinationServer.Name;
            tr.DestinationDatabase = newDatabase.Name;
            tr.TransferData();

            Transfer trsfrDB = new Transfer(templateDatabase);
            trsfrDB.CopyAllTables = true;
            trsfrDB.CopyData = false;
            trsfrDB.Options.DriPrimaryKey = true;
            trsfrDB.Options.DriForeignKeys = true;
            trsfrDB.Options.DriAllConstraints = true;
            trsfrDB.DestinationServer = destinationServer.Name;
            trsfrDB.DestinationDatabase = newDatabase.Name;
            trsfrDB.TransferData();

            // WORKING WITH USERDATA TABLE             

            newDatabase = destinationServer.Databases[loginName];
            newDatabase.Tables["UserData"].Drop();

            trsfrDB.CopyAllObjects = false;
            trsfrDB.CopyAllTables = false;
            trsfrDB.CopyData = true;
            trsfrDB.Options.DriPrimaryKey = true;
            trsfrDB.Options.DriForeignKeys = true;
            trsfrDB.Options.DriAllConstraints = true;

            trsfrDB.ObjectList.Add(templateDatabase.Tables["UserData"]);


            trsfrDB.TransferData();    
        }

        public void CreateUserDataBaseUsingScript(string loginName)
        {
            String str;
            SqlConnection myConn = new SqlConnection(@"Server=PC\SQLEXPRESS;Integrated security=SSPI;database=master");

            str ="CREATE DATABASE MyDatabase ON PRIMARY " +
                "(NAME = MyDatabase_Data, " +
                @"FILENAME = 'C:\MyDatabaseData.mdf', " +
                "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = MyDatabase_Log, " +
                @"FILENAME = 'C:\MyDatabaseLog.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                //MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }        
    }
}
