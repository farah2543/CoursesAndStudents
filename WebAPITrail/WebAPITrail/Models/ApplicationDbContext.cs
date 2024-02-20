using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using WebAPITrail.Models;

namespace WebAPITrail
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) 
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(localdb)\ProjectModels;Initial Catalog = learning ;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            });


            modelBuilder.Entity<Student>()
                 .HasMany(c => c.Courses)
                 .WithMany(c => c.Students)
                 .UsingEntity<Learns>(
                j=>j
                .HasOne(le=>le.course)
                .WithMany(t=>t.learn)
                .HasForeignKey(k=>k.courseId),

                j => j
                .HasOne(le => le.student)
                .WithMany(t => t.learn)
                .HasForeignKey(k => k.StudentId),

                j=>
                {
                    j.HasKey(t => new { t.StudentId, t.courseId,t.semester,t. year});
                }
            );

            modelBuilder.Entity<Student>()
              .Property(b => b.StudentId)
                .ValueGeneratedNever();


        }
        public DbSet<Student>? Students { get; set; }
        public DbSet<Course>? courses { get; set; }

        public DbSet <Learns>? Learns { get; set; }


    }
   
}
