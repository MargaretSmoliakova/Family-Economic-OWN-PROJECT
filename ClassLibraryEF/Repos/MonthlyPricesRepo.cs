using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ClassLibraryEF.Models;

namespace ClassLibraryEF.Repos
{
    public class MonthlyPricesRepo : BaseRepo<MonthlyPrice>, IRepo<MonthlyPrice>
    {
        public MonthlyPricesRepo()
        {
            Table = Context.MonthlyPrices;
        }

        public int Delete(int id)
        {
            Context.Entry(new MonthlyPrice() { Id = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public int RemoveAll()
        {
            foreach (MonthlyPrice mp in Table)
            {
                Table.Remove(mp);
            }

            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new MonthlyPrice() { Id = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public MonthlyPrice GetOneByName(string name)
        {
            MonthlyPrice entity = new MonthlyPrice();

            foreach (MonthlyPrice ent in Table.Where(ent => ent.Name == name))
            {
                entity = ent;
            }

            return entity;
        }
    }
}
