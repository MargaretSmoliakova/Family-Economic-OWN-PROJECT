using ClassLibraryEF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryEF.Repos
{
    public class CurrentPricesRepo : BaseRepo<CurrentPrice>, IRepo<CurrentPrice>
    {
        public CurrentPricesRepo()
        {
            Table = Context.CurrentPrices;
        }

        public int Delete(int id)
        {
            Context.Entry(new CurrentPrice() { Id = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public int DeleteByName(string name)
        {
            CurrentPrice entityForDelete = new CurrentPrice();
            entityForDelete = GetOneByName(name);

            if (entityForDelete.MonthlyPrice != null)
            {
                using (var repoMonthly = new MonthlyPricesRepo())
                {
                    repoMonthly.Delete(entityForDelete.MonthlyPrice.Id);
                }
            }
            else
            {
                using (var repoPermanent = new PermanentPricesRepo())
                {
                    repoPermanent.Delete(entityForDelete.PermanentPrice.Id);
                }
            }           

            return SaveChanges();
        }

        public int ResetAll()
        {
            using (var repoMonthly = new MonthlyPricesRepo())
            {
                repoMonthly.RemoveAll();
            }

            foreach (CurrentPrice cp in Table)
            {
                Table.Remove(cp);
            }

            using (var repoPermanent = new PermanentPricesRepo())
            {
                foreach (PermanentPrice pEnt in repoPermanent.GetAll())
                {
                    CurrentPrice newCurrent = new CurrentPrice { Id_permanent_prices = pEnt.Id, Price = pEnt.Price, Check_box = 0};                    
                    pEnt.CurrentPrices.Add(newCurrent);
                    repoPermanent.Save(pEnt);
                }
            }          

            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new CurrentPrice() { Id = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public List<CurrentPriceUserFriendlyView> GetAllUserFriendlyView()
        {
            List<CurrentPriceUserFriendlyView> list = new List<CurrentPriceUserFriendlyView>();       

            foreach (CurrentPrice entity in Table)
            { 
                if (entity.Id_monthly_prices != null)
                {
                    list.Add(new CurrentPriceUserFriendlyView { Id = entity.Id, Name = entity.MonthlyPrice.Name, Price = entity.Price, Check_box = entity.Check_box });                        
                }
                else
                {
                    list.Add(new CurrentPriceUserFriendlyView { Id = entity.Id, Name = entity.PermanentPrice.Name, Price = entity.Price, Check_box = entity.Check_box });
                }
            }                
            
            return list;
        }

        public List<CurrentPriceUserFriendlyView> GetMonthlyUserFriendlyView()
        {
            List<CurrentPriceUserFriendlyView> list = new List<CurrentPriceUserFriendlyView>();

            using (var repoCurrent = new CurrentPricesRepo())
            {
                foreach (CurrentPrice entity in repoCurrent.GetAll())
                {                   
                    if (entity.Id_monthly_prices != null)
                    {
                        list.Add(new CurrentPriceUserFriendlyView { Id = entity.Id, Name = entity.MonthlyPrice.Name, Price = entity.Price, Check_box = entity.Check_box });
                    }                    
                }
            }

            return list;
        }

        public List<CurrentPriceUserFriendlyView> GetPermanentUserFriendlyView()
        {
            List<CurrentPriceUserFriendlyView> list = new List<CurrentPriceUserFriendlyView>();
            
            foreach (CurrentPrice entity in Table)
            {
                CurrentPriceUserFriendlyView friendlyView = new CurrentPriceUserFriendlyView();

                if (entity.Id_permanent_prices != null)
                {
                    list.Add(new CurrentPriceUserFriendlyView { Id = entity.Id, Name = entity.PermanentPrice.Name, Price = entity.Price, Check_box = entity.Check_box });
                }
            }            

            return list;
        }
            
        public CurrentPrice GetOneByName(string name)
        {            
            foreach (CurrentPrice entity in Table)
            {
                if (entity.MonthlyPrice != null && entity.MonthlyPrice.Name == name)
                {
                    return entity;
                }
                else if (entity.PermanentPrice != null && entity.PermanentPrice.Name == name)
                {
                    return entity;
                }
            }

            return null;            
        }


    }
}
