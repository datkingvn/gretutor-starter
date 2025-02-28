using GreTutor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.Entity<BlogPost>()
            .HasOne(bp => bp.User)          // 🟢 Navigation Property
            .WithMany()                      // 🔵 User có thể có nhiều bài viết
            .HasForeignKey(bp => bp.AuthorId) // 🟠 Dùng AuthorId làm khóa ngoại
            .OnDelete(DeleteBehavior.Cascade); // 🟢 Khi xóa User, xóa luôn bài viết

            // builder.Entity<Comment>()
            //     .HasOne(c => c.BlogPost)
            //     .WithMany(b => b.Comments)
            //     .HasForeignKey(c => c.BlogPostId)
            //     .OnDelete(DeleteBehavior.Cascade); // ✅ Bình luận bị xóa nếu xóa bài viết

            // Bỏ tiền tố AspNet của các bảng: mặc định các bảng trong IdentityDbContext có
            // tên với tiền tố AspNet như: AspNetUserRoles, AspNetUser ...
            // Đoạn mã sau chạy khi khởi tạo DbContext, tạo database sẽ loại bỏ tiền tố đó
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
