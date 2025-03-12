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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Class> Classes { get; set; } 
        public DbSet<ClassMember> ClassMembers { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            //BlogPost - IdentityUser
            builder.Entity<BlogPost>()
            .HasOne(bp => bp.User)          // 🟢 Navigation Property
            .WithMany()                      // 🔵 User có thể có nhiều bài viết
            .HasForeignKey(bp => bp.AuthorId) // 🟠 Dùng AuthorId làm khóa ngoại
            .OnDelete(DeleteBehavior.Restrict);   // 🟢 Khi xóa User, xóa luôn bài viết

            //Comment - BlogPost
            builder.Entity<Comment>()
            .HasOne(c => c.BlogPost)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BlogId)  // 👈 Kiểm tra khóa ngoại!
            .OnDelete(DeleteBehavior.Cascade);  

            //ClassMember - IdentityUser
            builder.Entity<ClassMember>()
                .HasOne(cm => cm.User)
                .WithMany()
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Class - ClassMember
            builder.Entity<ClassMember>()
                .HasOne(cm => cm.Class)
                .WithMany(c => c.ClassMembers)
                .HasForeignKey(cm => cm.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            //Class - Document
            builder.Entity<Document>()
                .HasOne(d => d.Class)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            //Class - Meeting
            builder.Entity<Meeting>()
                .HasOne(m => m.Class)
                .WithMany(c => c.Meetings)
                .HasForeignKey(m => m.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            //Class - ChatMessage
            builder.Entity<ChatMessage>()
                .HasOne(cm => cm.Class)
                .WithMany(c => c.ChatMessages)
                .HasForeignKey(cm => cm.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

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
