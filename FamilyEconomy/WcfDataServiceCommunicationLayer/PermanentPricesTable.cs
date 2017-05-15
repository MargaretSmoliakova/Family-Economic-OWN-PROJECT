using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainWindow.WcfDataServiceReference;

namespace FamilyEconomy.WcfDataServiceCommunicationLayer
{
    public class PermanentPricesTable
    {
        FamilyEconomicEntities familyEconomicDbContext;
        Uri uri = new Uri("http://localhost/FamilyEconomyDataServices/WcfDataService.svc");

        public PermanentPricesTable()
        {
            GetFreshData();
        }

        public void Delete(int id)
        {
            GetFreshData();

            var objToDel = from l in familyEconomicDbContext.PermanentPrices
                           where l.Id == id
                           select l;

            foreach (PermanentPrice pp in objToDel)
            {
                familyEconomicDbContext.DeleteObject(pp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public PermanentPrice GetOneByName(string name)
        {
            GetFreshData();

            PermanentPrice permanentPrice = new PermanentPrice();

            var obj = from l in familyEconomicDbContext.PermanentPrices
                      where l.Name == name
                      select l;

            foreach(PermanentPrice pp in obj)
            {
                permanentPrice.Id = pp.Id;
                permanentPrice.Name = pp.Name;
                permanentPrice.Price = pp.Price;
                permanentPrice.Group = pp.Group;                
            }

            return permanentPrice;
        }

        public List<string> GetAllNames()
        {
            GetFreshData();

            List<string> list = new List<string>();

            var obj = from l in familyEconomicDbContext.PermanentPrices
                      select l;

            foreach (PermanentPrice pp in obj)
            {
                list.Add(pp.Name);
            }

            return list;
        }

        public void ChangePrice(string name, string price)
        {
            GetFreshData();

            var obj = from l in familyEconomicDbContext.PermanentPrices
                      where l.Name == name
                      select l;

            foreach (PermanentPrice pp in obj)
            {
                pp.Price = price;

                familyEconomicDbContext.UpdateObject(pp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        private void GetFreshData()
        {
            familyEconomicDbContext = new FamilyEconomicEntities(uri);
        }
    }
}
