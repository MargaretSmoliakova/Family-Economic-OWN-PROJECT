using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyEconomy;
using MainWindow.WcfDataServiceReference;
using FamilyEconomy.WcfDataServiceCommunicationLayer.AdditionalClasses;

namespace FamilyEconomy.WcfDataServiceCommunicationLayer
{
    public class CurrentPricesTable
    {
        FamilyEconomicEntities familyEconomicDbContext;
        Uri uri = new Uri("http://localhost/FamilyEconomyDataServices/WcfDataService.svc");

        public CurrentPricesTable()
        {
            GetFreshData();
        }
                        
        public void DeleteCurrentPriceWithDependencies(string name)
        {
            GetFreshData();

            var objToDel = from l in familyEconomicDbContext.CurrentPrices.Expand("MonthlyPrice,PermanentPrice")
                           where l.MonthlyPrice.Name == name || l.PermanentPrice.Name == name
                           select l;

            foreach (CurrentPrice cp in objToDel)
            {
                if (cp.MonthlyPrice != null) familyEconomicDbContext.DeleteObject(cp.MonthlyPrice);
                else if (cp.PermanentPrice != null) familyEconomicDbContext.DeleteObject(cp.PermanentPrice);
            }

            familyEconomicDbContext.SaveChanges();
        }

        public void ResetAll()
        {
            GetFreshData();

            var objToDel = from l in familyEconomicDbContext.MonthlyPrices
                           select l;

            foreach (var c in objToDel)
            {
                familyEconomicDbContext.DeleteObject(c);
                familyEconomicDbContext.SaveChanges();
            }

            var objToChange = from l in familyEconomicDbContext.PermanentPrices.Expand("CurrentPrices")                           
                              select l;


            foreach (PermanentPrice pp in objToChange)
            {
                CurrentPrice cp = pp.CurrentPrices.FirstOrDefault();
                cp.Price = pp.Price;              

                familyEconomicDbContext.UpdateObject(cp);                
            }

            familyEconomicDbContext.SaveChanges();

        }

        public List<CurrentPriceUserFriendlyView> GetAllUserFriendlyView()
        {
            GetFreshData();

            List<List<CurrentPriceUserFriendlyView>> list = new List<List<CurrentPriceUserFriendlyView>>();
            List<CurrentPriceUserFriendlyView> listToReturn = new List<CurrentPriceUserFriendlyView>();

            list.Add(GetMonthlyUserFriendlyView());
            list.Add(GetPermanentUserFriendlyView());

            foreach (List<CurrentPriceUserFriendlyView> lcp in list)
            {
                foreach (CurrentPriceUserFriendlyView fv in lcp)
                {
                    listToReturn.Add(fv);
                }
            }

            return listToReturn;
        }

        public List<CurrentPriceUserFriendlyView> GetMonthlyUserFriendlyView()
        {
            GetFreshData();

            List<CurrentPriceUserFriendlyView> list = new List<CurrentPriceUserFriendlyView>();

            var monthlyPrices = from p in familyEconomicDbContext.MonthlyPrices.Expand("CurrentPrices")
                                select p;

            foreach (MonthlyPrice mp in monthlyPrices)
            {
                CurrentPriceUserFriendlyView fv = new CurrentPriceUserFriendlyView
                {
                    Name = mp.Name,
                    Price = mp.CurrentPrices.FirstOrDefault().Price,
                    Check_box = mp.CurrentPrices.FirstOrDefault().Check_box,
                    Consideration = mp.CurrentPrices.FirstOrDefault().Consideration
                };

                list.Add(fv);

            }

            return list;
        }

        public List<CurrentPriceUserFriendlyView> GetPermanentUserFriendlyView()
        {
            GetFreshData();

            List<CurrentPriceUserFriendlyView> list = new List<CurrentPriceUserFriendlyView>();
            
            var permanentPrices = from p in familyEconomicDbContext.PermanentPrices.Expand("CurrentPrices")
                                  select p;
            
            foreach (PermanentPrice pp in permanentPrices)
            {
                CurrentPriceUserFriendlyView fv = new CurrentPriceUserFriendlyView
                {
                    Name = pp.Name,
                    Price = pp.CurrentPrices.FirstOrDefault().Price,
                    Check_box = pp.CurrentPrices.FirstOrDefault().Check_box,
                    Consideration = pp.CurrentPrices.FirstOrDefault().Consideration
                };

                list.Add(fv);
            }

            return list;
        }

        public CurrentPrice GetOneByName(string name)
        {
            GetFreshData();

            CurrentPrice objToReturn = new CurrentPrice();

            var obj = from l in familyEconomicDbContext.CurrentPrices
                           where l.MonthlyPrice.Name == name ||
                           l.PermanentPrice.Name == name
                           select new
                           {
                               Id = l.Id,
                               Id_monthly_prices = l.Id_monthly_prices,
                               Id_permanent_prices = l.Id_permanent_prices,
                               Price = l.Price,
                               Check_box = l.Check_box,
                               Consideration = l.Consideration
                           };

            foreach (var cp in obj)
            {
                objToReturn.Id = cp.Id;
                objToReturn.Id_monthly_prices = cp.Id_monthly_prices;
                objToReturn.Id_permanent_prices = cp.Id_permanent_prices;
                objToReturn.Price = cp.Price;
                objToReturn.Check_box = cp.Check_box;
                objToReturn.Consideration = cp.Consideration;
            }

            return objToReturn;
        }

        public void ChangeCheckBox(string name, int checkBoxValue)
        {
            GetFreshData();

            var objToChange = from l in familyEconomicDbContext.CurrentPrices
                              where l.MonthlyPrice.Name == name || l.PermanentPrice.Name == name
                              select l;


            foreach (CurrentPrice cp in objToChange)
            {
                cp.Check_box = checkBoxValue;

                familyEconomicDbContext.UpdateObject(cp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public void ChangeConsideration(string name, int considerationValue)
        {
            GetFreshData();

            var objToChange = from l in familyEconomicDbContext.CurrentPrices
                              where l.MonthlyPrice.Name == name || l.PermanentPrice.Name == name
                              select l;

            foreach (CurrentPrice cp in objToChange)
            {
                cp.Consideration = considerationValue;
                
                familyEconomicDbContext.UpdateObject(cp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public void ChangeCurrentPriceWithDependencies(string oldname, string name, string price, string group, int? checkBoxValue, int? considerationValue)
        {
            GetFreshData();

            var objToChange = from l in familyEconomicDbContext.CurrentPrices.Expand("MonthlyPrice,PermanentPrice")
                              where l.MonthlyPrice.Name == oldname || l.PermanentPrice.Name == oldname
                              select l;

            foreach (CurrentPrice cp in objToChange)
            {
                cp.Price = price;
                cp.Check_box = checkBoxValue;
                cp.Consideration = considerationValue;

                if (cp.MonthlyPrice != null)
                {
                    cp.MonthlyPrice.Name = name;
                    cp.MonthlyPrice.Group = group;

                    familyEconomicDbContext.UpdateObject(cp.MonthlyPrice);
                }
                else if (cp.PermanentPrice != null)
                {
                    cp.PermanentPrice.Name = name;
                    cp.PermanentPrice.Group = group;                    

                    familyEconomicDbContext.UpdateObject(cp.PermanentPrice);
                }


                familyEconomicDbContext.UpdateObject(cp);
                familyEconomicDbContext.SaveChanges();
            }
        }

        public string CheckWhetherExistsMonthlyPriceByName(string name)
        {
            GetFreshData();

            string mpName = null;

            var obj = from l in familyEconomicDbContext.MonthlyPrices
                      where l.Name == name
                      select l;

            foreach (var p in obj)
            {                
                mpName = p.Name;                
            }

            return mpName; 
        }

        public string CheckWhetherExistsPermanentPriceByName(string name)
        {
            GetFreshData();

            string ppName = null;

            var obj = from l in familyEconomicDbContext.PermanentPrices
                      where l.Name == name
                      select l;

            foreach (var p in obj)
            {
                ppName = p.Name;
            }

            return ppName;          
        }

        public void AddCurrentAndDependentMonthlyPrice(string name, string price, string group)
        {
            GetFreshData();

            MonthlyPrice mp = new MonthlyPrice();
            mp.Name = name;
            mp.Group = group;

            familyEconomicDbContext.AddObject("MonthlyPrices", mp);
            familyEconomicDbContext.SaveChanges();

            var newMonthlyPrice = (from mmm in familyEconomicDbContext.MonthlyPrices
                                   where mmm.Name == name
                                   select mmm).Single();

            CurrentPrice cp = new CurrentPrice();
            cp.Id_monthly_prices = newMonthlyPrice.Id;
            cp.Price = price;
            cp.Check_box = 0;
            cp.Consideration = 0;

            familyEconomicDbContext.AddRelatedObject(newMonthlyPrice, "CurrentPrices", cp);
            familyEconomicDbContext.SaveChanges();

            newMonthlyPrice.CurrentPrices.Add(cp);
            cp.MonthlyPrice = newMonthlyPrice;

            familyEconomicDbContext.SaveChanges();
        }

        public void AddCurrentAndDependentPermanentPrice(string name, string price, string group)
        {
            GetFreshData();

            PermanentPrice pp = new PermanentPrice();
            pp.Name = name;
            pp.Price = price;
            pp.Group = group;            

            familyEconomicDbContext.AddObject("PermanentPrices", pp);
            familyEconomicDbContext.SaveChanges();

            var newPermanentPrice = (from ppp in familyEconomicDbContext.PermanentPrices
                                     where ppp.Name == name
                                     select ppp).Single();

            CurrentPrice cp = new CurrentPrice();
            cp.Id_permanent_prices = newPermanentPrice.Id;
            cp.Price = newPermanentPrice.Price;
            cp.Check_box = 0;
            cp.Consideration = 0;

            familyEconomicDbContext.AddRelatedObject(newPermanentPrice, "CurrentPrices", cp);
            familyEconomicDbContext.SaveChanges();

            newPermanentPrice.CurrentPrices.Add(cp);
            cp.PermanentPrice = newPermanentPrice;

            familyEconomicDbContext.SaveChanges();
        }

        public CurrentPrice GetCurrentPriceWithDependenciesByName(string name)
        {
            GetFreshData();

            CurrentPrice objToReturn = new CurrentPrice();

            var obj = from l in familyEconomicDbContext.CurrentPrices.Expand("MonthlyPrice,PermanentPrice")
                      where l.MonthlyPrice.Name == name || l.PermanentPrice.Name == name
                      select l;

            foreach (CurrentPrice cp in obj)
            {
                objToReturn.Id = cp.Id;
                objToReturn.Id_monthly_prices = cp.Id_monthly_prices;
                objToReturn.Id_permanent_prices = cp.Id_permanent_prices;
                objToReturn.MonthlyPrice = cp.MonthlyPrice;
                objToReturn.PermanentPrice = cp.PermanentPrice;
                objToReturn.Price = cp.Price;
                objToReturn.Check_box = cp.Check_box;
                objToReturn.Consideration = cp.Consideration;
            }

            return objToReturn;
        }

        private void GetFreshData()
        {
            familyEconomicDbContext = new FamilyEconomicEntities(uri);
        }
        
    }
}
