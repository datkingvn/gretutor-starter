using GreTutor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GreTutor.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating (ModelBuilder builder) {

            base.OnModelCreating (builder); 
            // Bỏ tiền tố AspNet của các bảng: mặc định các bảng trong IdentityDbContext có
            // tên với tiền tố AspNet như: AspNetUserRoles, AspNetUser ...
            // Đoạn mã sau chạy khi khởi tạo DbContext, tạo database sẽ loại bỏ tiền tố đó
            foreach (var entityType in builder.Model.GetEntityTypes ()) {
                var tableName = entityType.GetTableName ();
                if (tableName.StartsWith ("AspNet")) {
                    entityType.SetTableName (tableName.Substring (6));
                }
            }
        }
        // DbSet đại diện cho bảng trong database
        //Example to test table update to database
        // public DbSet<Candidate> Candidates { get; set; }
    }
    //Example to test table update to database
    // public class Candidate
    // {
    //     public int Id { get; set; }
    //     public string JobPosition { get; set; }
    //     public string Name { get; set; }
    //     public string Email { get; set; }
    //     public string PhoneNumber { get; set; }
    //     public string WorkExperiences { get; set; }

    // }
}
