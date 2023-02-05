using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Infrastructure
{
    public class OrderDbContext:DbContext
    {
        //şema belirle
        public const string DEFAULT_SCHEMA = "ordering";

        public OrderDbContext(DbContextOptions<OrderDbContext> options):base(options)
        {
        }

        //public override int SaveChanges()
        //{
        //    //eğer birden fazla aggregate'imiz varsa event'leri kullanmalıyız bu aggregate'ler birbirleriyle haberleşme yoluna girecekse.
        //    //event'ler burada fırlatılır.
        //    return base.SaveChanges();
        //}

        public DbSet<Domain.OrderAggregate.Order> Orders { get; set; }
        public DbSet<Domain.OrderAggregate.OrderItem> OrderItems { get; set; }

        //settings klasörü açılıp orada da yapılabilir bu ayarlar.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SCHEMA);
            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().ToTable("OrderItems", DEFAULT_SCHEMA);

            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(o => o.Address).WithOwner();//owned type olduğunu ve sahibinin Order olduğunu belirtir.


            base.OnModelCreating(modelBuilder);
        }
    }
}
