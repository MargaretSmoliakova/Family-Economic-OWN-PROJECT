using ClassLibraryEF;
using ClassLibraryEF.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryEF.Repos
{
    public class PermanentPricesRepo : BaseRepo<PermanentPrice>, IRepo<PermanentPrice>
    {
        public PermanentPricesRepo()
        {
            Table = Context.PermanentPrices;
        }

        public int Delete(int id)
        {
            Context.Entry(new PermanentPrice() { Id = id }).State = EntityState.Deleted;
            return SaveChanges();
        }

        public Task<int> DeleteAsync(int id)
        {
            Context.Entry(new PermanentPrice() { Id = id }).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        public PermanentPrice GetOneByName(string name)
        {
            PermanentPrice entity = new PermanentPrice();

            foreach (PermanentPrice ent in Table.Where(ent => ent.Name == name))
            {
                entity = ent;
            }

            return entity;
        }

        public List<string> GetAllNames()
        {
            List<string> list = new List<string>();

            using (var repoPermanent = new PermanentPricesRepo())
            {
                foreach (PermanentPrice pp in repoPermanent.GetAll())
                {
                    list.Add(pp.Name);
                }
            }

            return list;
        }

    }
}
