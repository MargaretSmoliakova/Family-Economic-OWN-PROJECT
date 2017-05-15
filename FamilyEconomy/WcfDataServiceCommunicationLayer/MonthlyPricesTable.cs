using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainWindow.WcfDataServiceReference;

namespace FamilyEconomy.WcfDataServiceCommunicationLayer
{
    public class MonthlyPricesTable
    {
        FamilyEconomicEntities familyEconomicDbContext;
        Uri uri = new Uri("http://localhost/FamilyEconomyDataServices/WcfDataService.svc");

        public MonthlyPricesTable()
        {
            GetFreshData();
        }

        public void Delete(int id)
        {
            GetFreshData();

            var objToDel = from l in familyEconomicDbContext.MonthlyPrices
                           where l.Id == id
                           select l;

            foreach (MonthlyPrice cp in objToDel)
            {
                familyEconomicDbContext.DeleteObject(cp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public void RemoveAll()
        {
            GetFreshData();

            var objToDel = from l in familyEconomicDbContext.MonthlyPrices
                           select l;

            foreach (var c in objToDel)
            {
                familyEconomicDbContext.DeleteObject(c);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public MonthlyPrice GetOneByName(string name)
        {
            GetFreshData();

            MonthlyPrice monthlyPrice = new MonthlyPrice();

            var obj = from l in familyEconomicDbContext.MonthlyPrices
                      where l.Name == name
                      select l;

            foreach (MonthlyPrice mp in obj)
            {
                monthlyPrice.Id = mp.Id;
                monthlyPrice.Name = mp.Name;
                monthlyPrice.Group = mp.Group;
            }

            return monthlyPrice;
        }

        private void GetFreshData()
        {
            familyEconomicDbContext = new FamilyEconomicEntities(uri);
        }
    }
}
