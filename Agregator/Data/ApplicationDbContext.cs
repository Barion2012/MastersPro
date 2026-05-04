using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Agregator.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<AgregatorUser, Role, Guid
        ,IdentityUserClaim<Guid>
        ,AppUserRole
        ,IdentityUserLogin<Guid>
        ,IdentityRoleClaim<Guid>
        , AppUserToken
        >
    {
#if DESIGN
        public virtual DbSet<Bank> Banks { set; get; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<FeedBackStatus> FeedBackStatus { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<BuildObject> BuildObjects { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Vacancy> Vacancies { get; set; }
        public virtual DbSet<VacancyBuildObjectCt> VacancyBuildObjectCts { get; set; }

        public virtual DbSet<Contract> Contracts { get; set; }

        public virtual DbSet<AgregatorUser> AppUsers { get => Users; }

        public virtual DbSet<BlobStorage> BlobStorage { get; set; }
        public virtual DbSet<ContractAttachment> ContractAttachments { get; set; }
        public virtual DbSet<Work> Works { get; set; }

        public virtual DbSet<DevelopmentPlan> DevelopmentPlan { get; set; }

        public virtual DbSet<Recipient> Recipients { get; set; }
        public virtual DbSet<PersoneAddress> PersoneAddresses { get; set; }
        public virtual DbSet<PersoneDocument> PersoneDocuments { get; set; }
        public virtual DbSet<PersonePhone> PersonePhones { get; set; }
        public virtual DbSet<Enum> Enums { get; set; }
        public virtual DbSet<EnumType> EnumTypes { get; set; }
        public virtual DbSet<Frecipient> RecipientsView { get; set; }
        public virtual DbSet<ContractTemplate> ContractTemplates { get; set; }
        public virtual DbSet<CMessage> Messages { get; set; }
        public virtual DbSet<Applicant> Applicants { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AgregatorUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientID).HasColumnType("char(22)");
                entity.Property(e => e.Created).HasColumnName("created").HasColumnType("datetime").HasDefaultValueSql("current_timestamp()");
                entity.Property(e => e.Birthday).HasColumnType("datetime");
            });

            builder.Entity<AppUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

            });

            builder.Entity<BlobStorage>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");
                entity.Property(e => e.RowId).HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name).HasColumnType("varchar(20)");
                entity.Property(e => e.RowGuid).HasDefaultValueSql("uuid()");
                entity.Property(e => e.Created).HasColumnType("datetime").HasDefaultValueSql("current_timestamp()");


            });


            builder.Entity<Contract>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DocNum).HasColumnName("num").HasColumnType("varchar(20)");
                entity.Property(e => e.DocUri).HasColumnName("docUri").HasColumnType("varchar(2000)");
                entity.Property(e => e.SignUri).HasColumnName("signUri").HasColumnType("varchar(2000)");
                entity.Property(e => e.Remark).HasColumnName("remark").HasColumnType("Text");
                entity.Property(e => e.DocDate).HasColumnType("date");
                entity.Property(e => e.PaidDate).HasColumnName("paidDate").HasColumnType("datetime");

                entity.Property(e=>e.Created).HasColumnType("datetime");
                entity.Property(e=>e.SignDate).HasColumnType("datetime");
                entity.Property(e => e.Fee)
.HasColumnName("fee")
.HasColumnType("decimal(18,2)");

            });




            builder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("int(11)");

            entity.Property(e => e.Alfa2)
                .HasColumnName("alfa2")
                .HasColumnType("varchar(10)")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.Alfa3)
                .HasColumnName("alfa3")
                .HasColumnType("varchar(10)")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.CountryName)
                .HasColumnType("varchar(200)")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.FullName)
                .HasColumnType("varchar(2000)")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.IsEaes)
                .HasColumnName("IsEAES")
                .HasColumnType("tinyint(11)");

            entity.Property(e => e.UseProfile)
                .HasColumnName("useProfile")
                .HasColumnType("tinyint(11)");
        });

            builder.Entity<FeedBackStatus>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");
            });

            builder.Entity<Feedback>(entity =>
             {
                 entity.HasKey(e => e.RowId)
                     .HasName("PRIMARY");

                 entity.HasIndex(e => e.Status)
                     .HasDatabaseName("FK_Status_idx");

                 entity.HasIndex(e => e.Vacancy)
                     .HasDatabaseName("FK_Vacancy_idx");

                 entity.HasIndex(e => new { e.UserId, e.Vacancy })
                     .HasDatabaseName("IX_user_vacancy")
                     .IsUnique();

                 entity.Property(e => e.RowId)
                     .HasColumnName("row_id")
                     .HasColumnType("int(11)");

                 entity.Property(e => e.UserId)
                     .HasColumnName("UserId")
                     .HasCharSet("utf8mb4")
                     .UseCollation("utf8mb4_general_ci");

                 entity.Property(e => e.Created)
                     .HasColumnName("created")
                     .HasColumnType("datetime")
                     .HasDefaultValueSql("current_timestamp()");

                 entity.Property(e => e.Status)
                     .HasColumnName("status")
                     .HasColumnType("int(11)")
                     .HasDefaultValueSql("'1'");

                 entity.Property(e => e.Updated)
                     .HasColumnName("updated")
                     .HasColumnType("datetime");

                 entity.Property(e => e.Vacancy)
                     .HasColumnName("vacancy")
                     .HasColumnType("int(11)");

                 entity.HasOne(d => d.StatusNavigation)
                     .WithMany(p => p.Feedbacks)
                     .HasForeignKey(d => d.Status)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_Status");

                 entity.HasOne(d => d.VacancyNavigation)
                     .WithMany(p => p.Feedbacks)
                     .HasForeignKey(d => d.Vacancy)
                     .HasConstraintName("FK_Vacancy");
             });

            builder.Entity<BuildObject>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ParentId)
                    .HasDatabaseName("FK_Objects_idx");

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AppId)
                    .HasColumnName("app_id")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Lat)
                    .HasColumnName("lat")
                    .HasColumnType("decimal(15,6)");

                entity.Property(e => e.Lng)
                    .HasColumnName("lng")
                    .HasColumnType("decimal(15,6)");

                entity.Property(e => e.Map)
                    .HasColumnName("map")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");
            });

            builder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");

                entity.Property(e => e.RostScope)
                    .HasColumnName("rostScope")
                    .HasColumnType("bit");

            });


            builder.Entity<Vacancy>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Position)
                    .HasDatabaseName("FK_Vacancies_Positions_idx");

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AddressShowMetroOnly)
                    .HasColumnName("address_show_metro_only")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.AreaIdValue)
                    .HasColumnName("area_id_value")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.AreaIdView)
                    .HasColumnName("area_id_view")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.BillingTypeIdValue)
                    .HasColumnName("billing_type_id_value")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.BillingTypeIdView)
                    .HasColumnName("billing_type_id_view")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");
/*
                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(8000)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
*/
                entity.Property(e => e.KeySkillsNameValue)
                    .HasColumnName("key_skills_name_value")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.KeySkillsNameView)
                    .HasColumnName("key_skills_name_view")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");
                /*
                                entity.Property(e => e.Name)
                                    .HasColumnName("name")
                                    .HasColumnType("varchar(1000)")
                                    .HasCharSet("utf8mb4")
                                    .HasCollation("utf8mb4_general_ci");
                */
                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SalaryCurrencyValue)
                    .HasColumnName("salary_currency_value")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.SalaryCurrencyView)
                    .HasColumnName("salary_currency_view")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.SalaryFrom)
                    .HasColumnName("salary_from")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.SalaryTo)
                    .HasColumnName("salary_to")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.SpecializationsIdValue)
                    .HasColumnName("specializations_id_value")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.SpecializationsIdView)
                    .HasColumnName("specializations_id_view")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.TypeIdValue)
                    .HasColumnName("type_id_value")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.TypeIdView)
                    .HasColumnName("type_id_view")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.PositionNavigation)
                    .WithMany(p => p.Vacancies)
                    .HasForeignKey(d => d.Position)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vacancies_Positions");
            });

            builder.Entity<VacancyBuildObjectCt>(entity =>
            {
                entity.HasKey(e => new { e.Vacancy, e.Object })
                    .HasName("PRIMARY");

                entity.ToTable("VacancyObject_CT");

                entity.HasIndex(e => e.Object)
                    .HasDatabaseName("FK_Vacancy_Object_CrossTable_Objects_idx");

                entity.HasIndex(e => e.Vacancy)
                    .HasDatabaseName("FK_Vacancy_Object_CrossTable_Vacancies_idx");

                entity.Property(e => e.Vacancy)
                    .HasColumnName("vacancy")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Object)
                    .HasColumnName("object")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ObjectNavigation)
                    .WithMany(p => p.VacancyBuildObjectCts)
                    .HasForeignKey(d => d.Object)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Vacancy_Object_CrossTable_Objects");

                entity.HasOne(d => d.VacancyNavigation)
                    .WithMany(p => p.VacancyBuildObjectCts)
                    .HasForeignKey(d => d.Vacancy)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Vacancy_Object_CrossTable_Vacancies");

            });

            builder.Entity<Work>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");


                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");
            });

            builder.Entity<ContractAttachment>(entity =>
            {
                entity.HasKey(e => e.RowId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ContractId);

                entity.HasIndex(e => e.WorkId);

                entity.Property(e => e.RowId)
                    .HasColumnName("row_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ContractId).HasColumnType("int(11)");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(19,2)");

                entity.Property(e => e.Quant)
                    .HasColumnName("quant")
                    .HasColumnType("decimal(10,4)");

                entity.Property(e => e.RowNum)
                    .HasColumnName("rowNum")
                    .HasColumnType("smallint(6)");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.WorkId).HasColumnType("int(11)");

                entity.HasOne(d => d.Contract)
                        .WithMany(p => p.ContractAttachments)
                        .HasForeignKey(d => d.ContractId);

                entity.HasOne(d => d.Work)
                    .WithMany(p => p.ContractAttachments)
                    .HasForeignKey(d => d.WorkId);


                entity.Property(e => e.Path)
    .HasColumnName("path")
    .HasColumnType("varchar(1000)");

                entity.Property(e => e.Unit)
    .HasColumnName("unit")
    .HasColumnType("varchar(10)");

            });


            builder.Entity<DevelopmentPlan>(entity =>
            {
                entity.HasKey(e => e.RowId).HasName("PRIMARY");

//                entity.HasOne(f=>f.WhoResponsible).WithMany(p=>p.Id).

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

            });


            builder.Entity<PersoneDocument>(entity =>
            {

                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");
                entity.Property(e => e.serial).HasColumnType("varchar(10)");
                entity.Property(e => e.date).HasColumnType("datetime");
                entity.Property(e => e.expireDate).HasColumnType("datetime");
                entity.Property(e => e.organization).HasColumnType("varchar(1000)");
                entity.Property(e => e.division).HasColumnType("varchar(1000)");
            });

            builder.Entity<PersoneAddress>(entity =>
            {
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");

                entity.Property(e => e.city).HasColumnType("varchar(100)");
                entity.Property(e => e.postalCode).HasColumnType("char(6)");
                entity.Property(e => e.settlement).HasColumnType("varchar(100)");
                entity.Property(e => e.state).HasColumnType("varchar(100)");
                entity.Property(e => e.district).HasColumnType("varchar(100)");

                entity.Property(e => e.street).HasColumnType("varchar(100)");
                entity.Property(e => e.house).HasColumnType("varchar(100)");
                entity.Property(e => e.building).HasColumnType("varchar(100)");
                entity.Property(e => e.construction).HasColumnType("varchar(100)");
                entity.Property(e => e.apartment).HasColumnType("varchar(100)");

            });

            builder.Entity<PersonePhone>(entity =>
            {
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");

                entity.Property(e => e.number).HasColumnType("varchar(100)");

            });

            builder.Entity<Recipient>(entity =>
            {
                entity.HasKey(e => e.number);
                entity.Property(e => e.number).HasColumnName("row_id");

                entity.Property(e => e.firstName).HasColumnType("varchar(100)");
                entity.Property(e => e.lastName).HasColumnType("varchar(100)");
                entity.Property(e => e.middleName).HasColumnType("varchar(100)");
                entity.Property(e => e.birthDate).HasColumnType("date");
                entity.Property(e => e.latinFirstName).HasColumnType("varchar(100)");
                entity.Property(e => e.latinLastName).HasColumnType("varchar(100)");
                entity.Property(e => e.birthPlace).HasColumnType("varchar(100)");

                entity.Property(e => e.recipientId).HasColumnType("int");
                entity.Property(e => e.accountNumber).HasColumnType("varchar(50)");
                entity.Property(e => e.agreementNumber).HasColumnType("varchar(50)");
                entity.Property(e => e.accountStatus).HasColumnType("varchar(20)");
                entity.Property(e => e.selfEmployedStatus).HasColumnType("varchar(20)");

                entity.Property(e => e.created).HasColumnType("datetime").HasDefaultValueSql("current_timestamp()");
                entity.Property(e => e.updated).HasColumnType("datetime");

               // entity.Property(e => e.lastError).HasColumnType("varchar(max)");

            });

            builder.Entity<PersoneDocument>(entity =>
            {
                entity.Property(e => e.date).HasColumnType("date");
                entity.Property(e => e.expireDate).HasColumnType("date");

            });

            builder.Entity<Enum>(entity =>
            {
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");
                entity.Property(e => e.name).HasColumnType("varchar(100)");
                entity.Property(e => e.typeId).HasColumnName("enumTypeRowId");
            });
            
            builder.Entity<CMessage>(entity => 
            { 
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");
                entity.Property(e => e.Text).HasColumnType("varchar(8000)");
                entity.Property(e => e.Created).HasColumnName("created").HasColumnType("datetime").HasDefaultValueSql("current_timestamp()");
           
            });
            

            builder.Entity<EnumType>(entity =>
            {
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");
                entity.Property(e => e.name).HasColumnType("varchar(100)");
            });

            builder.Entity<Applicant>(entity =>
            {
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowId).HasColumnName("row_id");
                entity.Property(e => e.FirstName).HasColumnName("firstName").HasColumnType("varchar(100)");
                entity.Property(e => e.LastName).HasColumnName("lastName").HasColumnType("varchar(100)");
                entity.Property(e => e.MiddleName).HasColumnName("middleName").HasColumnType("varchar(100)");
                entity.Property(e => e.BirthPlace).HasColumnName("birthPlace").HasColumnType("varchar(200)");
                entity.Property(e => e.BirthDate).HasColumnName("birthDate").HasColumnType("datetime");
                entity.Property(e => e.Male).HasColumnName("male").HasColumnType("bit").HasDefaultValueSql("1");
                entity.Property(e => e.CitizenshipId).HasColumnName("citizenshipId").HasColumnType("int");
                entity.Property(e => e.ActualPlace).HasColumnName("actualPlace").HasColumnType("varchar(200)");


                entity.Property(e => e.Education).HasColumnName("education").HasColumnType("varchar(200)");
                entity.Property(e => e.MarriedStatus).HasColumnName("marriedStatus").HasColumnType("varchar(200)");
                entity.Property(e => e.DeviceNumber).HasColumnName("deviceNumber").HasColumnType("varchar(20)");
                entity.Property(e => e.ProfessionId).HasColumnName("professionId").HasColumnType("int");
                entity.Property(e => e.Skils).HasColumnName("skils").HasColumnType("varchar(200)");
                entity.Property(e => e.Tools).HasColumnName("tools").HasColumnType("varchar(200)");
                entity.Property(e => e.Hostel).HasColumnName("hostel").HasColumnType("bit");
                entity.Property(e => e.Agreement).HasColumnName("agreement").HasColumnType("bit");
                entity.Property(e => e.Сonfirmation).HasColumnName("confirmation").HasColumnType("varchar(200)");

                entity.Property(e => e.Created).HasColumnName("created").HasColumnType("datetime").HasDefaultValueSql("current_timestamp()");
            });


        }
#endif
    }
}
