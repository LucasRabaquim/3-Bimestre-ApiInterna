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
        public DbSet<FollowUser> FollowUsers { get; set; }
        public DbSet<FollowingPage> FollowingPages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<FollowUser>()
                .WithOne();

            modelBuilder.Entity<User>()
                .HasMany<Post>()
                .WithOne();

            modelBuilder.Entity<Page>()
                .HasMany<Post>()
                .WithOne();

            modelBuilder.Entity<Page>()
                .HasMany<FollowingPage>()
                .WithOne();

            modelBuilder.Entity<User>()
                .HasMany<FollowingPage>()
                .WithOne();

             modelBuilder.Entity<Page>()
                .HasMany<BookPage>()
                .WithOne();
        }
    }
}
