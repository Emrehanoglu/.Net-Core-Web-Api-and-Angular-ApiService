using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class SocialContext : IdentityDbContext<User,Role,int>
    {
        public SocialContext(DbContextOptions<SocialContext> options):base(options)
        {
            
        }   

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserToUser> UserToUser { get; set; }

        //UserToUser tablosu için.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //birincil anahtarları burada tanımlıyorum
            builder.Entity<UserToUser>().HasKey(k=> new {k.UserId, k.FollowerId});

            //ilişkileri burada tanımlıyorum
            //bire cok.
            //bir user birden fazla takipçiye sahip olabilecek
            builder.Entity<UserToUser>().HasOne(l=>l.User).WithMany(a=>a.Followers)
            .HasForeignKey(l=>l.UserId);

            //bir takipçi birden fazla hesabı takip edebilecek
            builder.Entity<UserToUser>().HasOne(l=>l.Follower).WithMany(a=>a.Following)
            .HasForeignKey(l=>l.FollowerId);
        }
    }
}