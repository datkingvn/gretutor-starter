using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GreTutor.Models.Entities;

namespace GreTutor.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogComment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình quan hệ cho BlogPost
            builder.Entity<BlogPost>()
                .HasOne(bp => bp.User)
                .WithMany()
                .HasForeignKey(bp => bp.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình quan hệ cho BlogComment
            builder.Entity<BlogComment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId);

            // Cấu hình kiểu dữ liệu cho cột Created
            builder.Entity<BlogPost>()
                .Property(bp => bp.Created)
                .HasColumnType("datetime");

            builder.Entity<BlogComment>()
                .Property(c => c.Created)
                .HasColumnType("datetime");

            // Loại bỏ tiền tố AspNet
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}