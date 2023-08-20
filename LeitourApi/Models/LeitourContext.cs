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
        public DbSet<FollowingList> FollowingLists { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<FollowingList>()
                .WithOne();

            modelBuilder.Entity<User>()
                .HasMany<Post>()
                .WithOne();
        }
    }
}
