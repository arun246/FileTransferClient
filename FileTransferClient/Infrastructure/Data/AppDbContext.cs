using FileTransferClient.Domain.Models.Connection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FileTransferClient.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ServerProfile> ServerProfiles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
