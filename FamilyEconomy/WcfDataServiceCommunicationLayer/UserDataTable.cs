using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainWindow.WcfDataServiceReference;

namespace FamilyEconomy.WcfDataServiceCommunicationLayer
{
    public class UserDataTable
    {
        FamilyEconomicEntities familyEconomicDbContext;
        Uri uri = new Uri("http://localhost/FamilyEconomyDataServices/WcfDataService.svc");

        string spentTodayDBColumnName = "spentToday";
        string limitDBColumnName = "limit";
        string budgetDBColumnName = "budget";
        string salaryDBColumnName = "salary";

        public UserDataTable()
        {
            GetFreshData();
        }

        public void Delete(int id)
        {
            GetFreshData();

            var objToDel = from l in familyEconomicDbContext.UserDatas
                           where l.Id == id
                           select l;

            foreach (UserData pp in objToDel)
            {
                familyEconomicDbContext.DeleteObject(pp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public UserData GetOneByName(string name)
        {
            GetFreshData();

            UserData userData = new UserData();

            var obj = from l in familyEconomicDbContext.UserDatas
                      where l.Name == name
                      select l;

            foreach (UserData ud in obj)
            {
                userData.Id = ud.Id;
                userData.Name = ud.Name;
                userData.Value = ud.Value;
            }

            return userData;
        }

        public void ChangeValue(string name, string value)
        {
            GetFreshData();

            var objToChange = from l in familyEconomicDbContext.UserDatas
                              where l.Name == name
                              select l;


            foreach (UserData pp in objToChange)
            {
                pp.Value = value;

                familyEconomicDbContext.UpdateObject(pp);
            }

            familyEconomicDbContext.SaveChanges();

        }

        public List<UserData> GetAll()
        {
            GetFreshData();

            List<UserData> list = new List<UserData>();

            var obj = from l in familyEconomicDbContext.UserDatas
                      select l;

            foreach(UserData ud in obj)
            {
                list.Add(ud);
            }

            return list;
        }

        public void SetToZeroAll()
        {
            GetFreshData();

            var obj = from l in familyEconomicDbContext.UserDatas
                      where l.Name == spentTodayDBColumnName || l.Name == limitDBColumnName || l.Name == budgetDBColumnName || l.Name == salaryDBColumnName
                      select l;

            foreach (UserData ud in obj)
            {
                ud.Value = "0";

                familyEconomicDbContext.UpdateObject(ud);
                familyEconomicDbContext.SaveChanges();
            }
        }

        private void GetFreshData()
        {
            familyEconomicDbContext = new FamilyEconomicEntities(uri);
        }
    }
}
