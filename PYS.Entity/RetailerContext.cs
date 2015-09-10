using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PYS.Entity
{
    public class RetailerContext : DbContext
    {
        public RetailerContext()
            : base("RegistryOps")
        {
            Database.SetInitializer<RetailerContext>(null);
        }

        public DbSet<Retailer> Retailers { get; set; }

        public static void Test()
        {
            using (var db = new RetailerContext())
            {
                var query = from r in db.Retailers orderby r.RetailerId select r;
                foreach (var item in query)
                {
                    Console.WriteLine(item.RetailerId);
                }
            }
        }
    }

    [Table("RetailerInfo", Schema = "product")]
    public class Retailer
    {
        [Key]
        public short RetailerId { get; set; }

        public bool IsEnable { get; set; }
    }
}
