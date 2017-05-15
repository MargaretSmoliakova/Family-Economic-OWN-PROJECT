using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplicationForTest.ServiceReference1;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using ConsoleApplicationForTest.ServiceReferenceNOTdata;
using System.Data.SqlClient;
using System.Data;

namespace ConsoleApplicationForTest
{
    class Program
    {
        static Uri uri = new Uri("http://localhost/FamilyEconomyDataServices/WcfDataService.svc");
        static FamilyEconomicEntities familyEconomicDbContext;

        static void Main(string[] args)
        {            
            Console.WriteLine("Started...!\n\n");
            familyEconomicDbContext = new FamilyEconomicEntities(uri);





            string str;
            SqlConnection myConn = new SqlConnection(@"Server=localhost;Integrated security=SSPI;database=master");

            str = "CREATE DATABASE MyDatabase";

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



            using (UserDatabaseCreationClient client = new UserDatabaseCreationClient())
            {
                client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 5, 0);

                Console.WriteLine(client.MethodForTest());
                //client.CreateUserDataBase("FamilyEconomicTemplate_db");
                //client.CreateUserDataBaseUsingScript("ddddd");              
                Console.WriteLine(client.MethodForTest());
            }           

           
            Console.WriteLine("Done!");
            
         








            Console.WriteLine("\n\nFinished...!");
            Console.ReadLine();
        }

        static void ChangeValue(string name, string value)
        {
            var objToChange = from l in familyEconomicDbContext.UserDatas
                              where l.Name == name
                              select l;

            foreach (UserData pp in objToChange)
            {
                pp.Value = value;

                familyEconomicDbContext.UpdateObject(pp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        static void GetData(string name)
        {
            var obj = from l in familyEconomicDbContext.UserDatas
                      where l.Name == name
                      select l;

            foreach (UserData pp in obj)
            {
                Console.WriteLine(pp.Name);
            }
        }

        static void CreateUserDataBase(string loginName)
        {
            string serverName = @"PC\SQLEXPRESS";

            Server sourceServer = new Server(serverName);
            Server destinationServer = new Server(serverName);
            sourceServer.ConnectionContext.LoginSecure = true;
            sourceServer.ConnectionContext.Connect();
            destinationServer.ConnectionContext.LoginSecure = true;
            destinationServer.ConnectionContext.Connect();
            Database templateDatabase = sourceServer.Databases["FamilyEconomic_db"];
            Database newDatabase = new Database(destinationServer, loginName);
            newDatabase.Create();

            // TRANSFER DATABASE

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
    }
}
