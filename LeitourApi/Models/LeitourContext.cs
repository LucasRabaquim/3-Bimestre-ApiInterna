using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace LeitourApi.Models
{
    public class LeitourContext : DbContext
    {
        public LeitourContext(DbContextOptions<LeitourContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
