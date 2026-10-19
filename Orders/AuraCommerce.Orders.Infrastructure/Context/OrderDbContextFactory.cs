using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.Orders.Infrastructure.Context
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var cs = Environment.GetEnvironmentVariable("ORDERDB_CONNECTION")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=AuraCommerce_OrdersDb;Trusted_Connection=True;TrustServerCertificate=True";
            var options = new DbContextOptionsBuilder<OrderDbContext>().UseSqlServer(cs).Options;
            return new OrderDbContext(options);
        }
    }
}
