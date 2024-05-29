using Edu_Block_dev.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace Edu_Block.DAL.EF
{
    public class EduBlockDataContext : DbContext
    {
        public DbSet<Admin> Admin { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Otp> Otp { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<DockIoDID> DockIoDIDs { get; set; }
        public DbSet<DockSchema> DockSchema { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<UserRequest> UserRequest { get; set; }
        public DbSet<Envelope> Envelopes { get; set; }
        public DbSet<EmployeeRequestGroup> EmployeeRequestGroups { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<EnvelopGroup> EnvelopGroups { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<ShareCredential> ShareCredentials { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<AvailableSubscription> AvailableSubscriptions { get; set; }
        public DbSet<PurchaseSubscription> PurchaseSubscriptions { get; set; }
        public DbSet<Transactions> Transaction { get; set; }
        public DbSet<UniversityDetail> UniversityDetails { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Schema> Schemas { get; set; }
        public DbSet<TemplateSchemaMapping> TemplateSchemaMappings { get; set; }
        public DbSet<PermissionDetail> PermissionDetails { get; set; }
        public DbSet<RolePermissionMapping> RolePermissionMappings { get; set; }
        public DbSet<LoginHistory> LoginHistorys { get; set; }
        public DbSet<DepartmentStudent> DepartmentStudents { get; set; }
        public DbSet<RequestMessage> RequestMessages { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<UniversityUser> UniversityUsers { get; set; }
        public DbSet<UniversityDepartmentUser> UniversityDepartmentUsers { get; set; }
        public DbSet<EmployerToken> EmployerTokens { get; set; }
        public DbSet<PaymentOutput> PaymentOutputs { get; set; }


        public EduBlockDataContext(DbContextOptions<EduBlockDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployerToken>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<EmployerToken>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<PaymentOutput>()
          .Property(s => s.CreatedAt)
          .HasDefaultValueSql("GETDATE()")
          .ValueGeneratedOnAdd();

            modelBuilder.Entity<PaymentOutput>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<UniversityUser>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<UniversityUser>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<UniversityDepartmentUser>()
          .Property(s => s.CreatedAt)
          .HasDefaultValueSql("GETDATE()")
          .ValueGeneratedOnAdd();

            modelBuilder.Entity<UniversityDepartmentUser>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();



            modelBuilder.Entity<TemplateSchemaMapping>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<TemplateSchemaMapping>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Schema>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Schema>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Template>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Template>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<PermissionDetail>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<PermissionDetail>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();




            modelBuilder.Entity<LoginHistory>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<LoginHistory>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<DepartmentStudent>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<DepartmentStudent>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Error>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<Error>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();


            modelBuilder.Entity<PaymentDetail>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<PaymentDetail>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Permission>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<Permission>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<RolePermissionMapping>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<RolePermissionMapping>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<UniversityDetail>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<UniversityDetail>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<RequestMessage>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<RequestMessage>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();


            modelBuilder.Entity<Transactions>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<University>()
               .HasIndex(u => u.Email)
               .IsUnique();

            modelBuilder.Entity<University>()
                .HasIndex(u => u.Id)
                .IsUnique();

            modelBuilder.Entity<University>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<University>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();


            modelBuilder.Entity<Admin>()
             .HasIndex(u => u.Email)
             .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(u => u.Id)
                .IsUnique();

            modelBuilder.Entity<Admin>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Admin>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<User>()
              .HasIndex(u => u.Email)
              .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Id)
                .IsUnique();

            modelBuilder.Entity<User>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();


            modelBuilder.Entity<Employer>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Employer>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Certificate>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Certificate>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Department>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Department>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<DockIoDID>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<DockIoDID>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<DockSchema>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<DockSchema>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<EmployeeRequestGroup>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<EmployeeRequestGroup>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Envelope>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Envelope>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<EnvelopGroup>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<EnvelopGroup>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Otp>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Otp>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Request>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Request>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Role>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Role>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<RoleClaim>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<RoleClaim>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Share>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Share>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<PurchaseSubscription>()
           .Property(s => s.CreatedAt)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAdd();

            modelBuilder.Entity<PurchaseSubscription>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<PurchaseSubscription>()
           .Property(s => s.StartDate)
           .HasDefaultValueSql("GETDATE()")
           .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Transactions>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transactions>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Transactions>()
            .Property(s => s.TransactionDate)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<AvailableSubscription>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<AvailableSubscription>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<ShareCredential>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<ShareCredential>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

           
            modelBuilder.Entity<UserProfile>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserProfile>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<UserRequest>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRequest>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<UserRole>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserRole>()
            .Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<EmployeeRequestGroup>()
            .HasKey(e => e.Id);

            modelBuilder.Entity<EmployeeRequestGroup>()
            .Property(e => e.Id)
            .IsRequired();

            modelBuilder.Entity<ShareCredential>()
            .Property(e => e.Id)
            .IsRequired();

            modelBuilder.Entity<ShareCredential>()
            .HasKey(e => e.Id);

            modelBuilder.Entity<UserProfile>()
            .HasKey(e => e.Id);

            modelBuilder.Entity<UserProfile>()
            .Property(e => e.UserID)
            .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
