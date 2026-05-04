using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    City = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    BIK = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CorAccount = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BuildObjects",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    parent_id = table.Column<int>(type: "int(11)", nullable: true),
                    status = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    app_id = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    name = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    map = table.Column<string>(type: "varchar(1000)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    lat = table.Column<decimal>(type: "decimal(15,6)", nullable: true),
                    lng = table.Column<decimal>(type: "decimal(15,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                });

            migrationBuilder.CreateTable(
                name: "ContractTemplates",
                columns: table => new
                {
                    RowId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Templ = table.Column<byte[]>(type: "longblob", nullable: true),
                    Core = table.Column<byte[]>(type: "longblob", nullable: true),
                    nameTempate = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    fullNameTempate = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTemplates", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CountryName = table.Column<string>(type: "varchar(200)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alfa2 = table.Column<string>(type: "varchar(10)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alfa3 = table.Column<string>(type: "varchar(10)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "varchar(2000)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEAES = table.Column<sbyte>(type: "tinyint(11)", nullable: true),
                    useProfile = table.Column<sbyte>(type: "tinyint(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "EnumTypes",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumTypes", x => x.row_id);
                });

            migrationBuilder.CreateTable(
                name: "FeedBackStatus",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                });

            migrationBuilder.CreateTable(
                name: "RecipientsView",
                columns: table => new
                {
                    number = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    appUserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Male = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    INN = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Account = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    BankId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    firstName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    lastName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    middleName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    birthDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    birthPlace = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    citizenship = table.Column<int>(type: "int", nullable: true),
                    latinFirstName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    latinLastName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    recipientId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    docSerial = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    docNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    docDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    organization = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    division = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    mgMumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    mgSerial = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    expireDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    mgDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    mgExpireDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    mgOrganization = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    mgDivision = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    postalCode = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    state = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    city = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    district = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    settlement = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    street = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    house = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    building = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    construction = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    apartment = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    countryId = table.Column<int>(type: "int", nullable: true),
                    regPostalCode = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regAddress = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regState = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regCity = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regDistrict = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regSettlement = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regStreet = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regHouse = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regBuilding = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regConstruption = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regApartment = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    regCountryId = table.Column<int>(type: "int", nullable: true),
                    fioName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    docTypeName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipientsView", x => x.number);
                });

            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    status = table.Column<sbyte>(type: "tinyint(4)", nullable: false),
                    name = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_bin")
                        .Annotation("MySql:CharSet", "utf8"),
                    created = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Surname = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Middlename = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Photo = table.Column<byte[]>(type: "longblob", nullable: true),
                    ProfileReady = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdentityReady = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Male = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    CountryCitizenshipId = table.Column<int>(type: "int(11)", nullable: true),
                    INN = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Account = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    BankId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ClientID = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    agreePrivacy = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    agreeRegFNS = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Countries_CountryCitizenshipId",
                        column: x => x.CountryCitizenshipId,
                        principalTable: "Countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enums",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    enumTypeRowId = table.Column<int>(type: "int", nullable: false),
                    enumTypeRowId1 = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enums", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_Enums_EnumTypes_enumTypeRowId1",
                        column: x => x.enumTypeRowId1,
                        principalTable: "EnumTypes",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    position = table.Column<int>(type: "int(11)", nullable: true),
                    billing_type_id_value = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    billing_type_id_view = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area_id_value = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area_id_view = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(8000)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    salary_currency_value = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    salary_currency_view = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    specializations_id_value = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    specializations_id_view = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_id_value = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type_id_view = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    key_skills_name_value = table.Column<string>(type: "varchar(50)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    key_skills_name_view = table.Column<string>(type: "varchar(1000)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    salary_from = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    salary_to = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    address_show_metro_only = table.Column<ulong>(type: "bit(1)", nullable: true),
                    DescriptionTab1 = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DescriptionTab2 = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DescriptionTab3 = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Creator = table.Column<Guid>(type: "char(36)", nullable: false),
                    Updater = table.Column<Guid>(type: "char(36)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_Vacancies_Positions",
                        column: x => x.position,
                        principalTable: "Positions",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlobStorage",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppUserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    RowGuid = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "uuid()"),
                    Name = table.Column<string>(type: "varchar(20)", nullable: true),
                    Blob = table.Column<byte[]>(type: "longblob", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "current_timestamp()"),
                    IsMain = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsZip = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_BlobStorage_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    num = table.Column<string>(type: "varchar(20)", nullable: true),
                    DocDate = table.Column<DateTime>(type: "date", nullable: true),
                    SignDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RegistreInfo = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    docUri = table.Column<string>(type: "varchar(2000)", nullable: true),
                    signUri = table.Column<string>(type: "varchar(2000)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IssuerId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CustomerId = table.Column<Guid>(type: "char(36)", nullable: true),
                    remark = table.Column<string>(type: "varchar(8000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_Contracts_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DevelopmentPlan",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PlanDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FactDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WhoResponsible = table.Column<Guid>(type: "char(36)", nullable: false),
                    WhoResponsibleNavigationId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Creator = table.Column<Guid>(type: "char(36)", nullable: true),
                    created = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    Updater = table.Column<Guid>(type: "char(36)", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.RowId);
                    table.ForeignKey(
                        name: "FK_DevelopmentPlan_AspNetUsers_WhoResponsibleNavigationId",
                        column: x => x.WhoResponsibleNavigationId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevelopmentPlan_DevelopmentPlan_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DevelopmentPlan",
                        principalColumn: "RowId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersoneAddresses",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppUserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    typeName = table.Column<int>(type: "int", nullable: false),
                    postalCode = table.Column<string>(type: "char(6)", nullable: true),
                    state = table.Column<string>(type: "varchar(100)", nullable: true),
                    city = table.Column<string>(type: "varchar(100)", nullable: true),
                    district = table.Column<string>(type: "varchar(100)", nullable: true),
                    settlement = table.Column<string>(type: "varchar(100)", nullable: true),
                    street = table.Column<string>(type: "varchar(100)", nullable: true),
                    house = table.Column<string>(type: "varchar(100)", nullable: true),
                    building = table.Column<string>(type: "varchar(100)", nullable: true),
                    construction = table.Column<string>(type: "varchar(100)", nullable: true),
                    apartment = table.Column<string>(type: "varchar(100)", nullable: true),
                    countryId = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersoneAddresses", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_PersoneAddresses_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersoneAddresses_Countries_countryId",
                        column: x => x.countryId,
                        principalTable: "Countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersoneDocuments",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppUserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    typeName = table.Column<int>(type: "int", nullable: false),
                    serial = table.Column<string>(type: "varchar(10)", nullable: true),
                    number = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    date = table.Column<DateTime>(type: "date", nullable: true),
                    expireDate = table.Column<DateTime>(type: "date", nullable: true),
                    organization = table.Column<string>(type: "varchar(1000)", nullable: true),
                    division = table.Column<string>(type: "varchar(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersoneDocuments", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_PersoneDocuments_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonePhones",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppUserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    typeName = table.Column<int>(type: "int", nullable: false),
                    number = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonePhones", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_PersonePhones_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppUserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    firstName = table.Column<string>(type: "varchar(100)", nullable: true),
                    lastName = table.Column<string>(type: "varchar(100)", nullable: true),
                    middleName = table.Column<string>(type: "varchar(100)", nullable: true),
                    birthDate = table.Column<DateTime>(type: "date", nullable: true),
                    birthPlace = table.Column<string>(type: "varchar(100)", nullable: true),
                    citizenship = table.Column<int>(type: "int", nullable: true),
                    latinFirstName = table.Column<string>(type: "varchar(100)", nullable: true),
                    latinLastName = table.Column<string>(type: "varchar(100)", nullable: true),
                    recipientId = table.Column<int>(type: "int", nullable: true),
                    correlationId = table.Column<Guid>(type: "char(36)", nullable: true),
                    accountStatus = table.Column<string>(type: "varchar(20)", nullable: true),
                    selfEmployedStatus = table.Column<string>(type: "varchar(20)", nullable: true),
                    accountNumber = table.Column<string>(type: "varchar(50)", nullable: true),
                    agreementNumber = table.Column<string>(type: "varchar(50)", nullable: true),
                    created = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_Recipients_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vacancy = table.Column<int>(type: "int(11)", nullable: false),
                    status = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    created = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Status",
                        column: x => x.status,
                        principalTable: "FeedBackStatus",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vacancy",
                        column: x => x.vacancy,
                        principalTable: "Vacancies",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyObject_CT",
                columns: table => new
                {
                    vacancy = table.Column<int>(type: "int(11)", nullable: false),
                    @object = table.Column<int>(name: "object", type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.vacancy, x.@object });
                    table.ForeignKey(
                        name: "FK_Vacancy_Object_CrossTable_Objects",
                        column: x => x.@object,
                        principalTable: "BuildObjects",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vacancy_Object_CrossTable_Vacancies",
                        column: x => x.vacancy,
                        principalTable: "Vacancies",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractAttachments",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int(11)", nullable: false),
                    rowNum = table.Column<short>(type: "smallint(6)", nullable: false),
                    WorkId = table.Column<int>(type: "int(11)", nullable: false),
                    quant = table.Column<int>(type: "int(11)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    created = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    userId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_ContractAttachments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractAttachments_Works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Works",
                        principalColumn: "row_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BankId",
                table: "AspNetUsers",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CountryCitizenshipId",
                table: "AspNetUsers",
                column: "CountryCitizenshipId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlobStorage_AppUserId",
                table: "BlobStorage",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "FK_Objects_idx",
                table: "BuildObjects",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAttachments_ContractId",
                table: "ContractAttachments",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAttachments_WorkId",
                table: "ContractAttachments",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CustomerId",
                table: "Contracts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_IssuerId",
                table: "Contracts",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_DevelopmentPlan_ParentId",
                table: "DevelopmentPlan",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DevelopmentPlan_WhoResponsibleNavigationId",
                table: "DevelopmentPlan",
                column: "WhoResponsibleNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Enums_enumTypeRowId1",
                table: "Enums",
                column: "enumTypeRowId1");

            migrationBuilder.CreateIndex(
                name: "FK_Status_idx",
                table: "Feedbacks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "FK_Vacancy_idx",
                table: "Feedbacks",
                column: "vacancy");

            migrationBuilder.CreateIndex(
                name: "IX_user_vacancy",
                table: "Feedbacks",
                columns: new[] { "UserId", "vacancy" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersoneAddresses_AppUserId",
                table: "PersoneAddresses",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PersoneAddresses_countryId",
                table: "PersoneAddresses",
                column: "countryId");

            migrationBuilder.CreateIndex(
                name: "IX_PersoneDocuments_AppUserId",
                table: "PersoneDocuments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonePhones_AppUserId",
                table: "PersonePhones",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_AppUserId",
                table: "Recipients",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "FK_Vacancies_Positions_idx",
                table: "Vacancies",
                column: "position");

            migrationBuilder.CreateIndex(
                name: "FK_Vacancy_Object_CrossTable_Objects_idx",
                table: "VacancyObject_CT",
                column: "object");

            migrationBuilder.CreateIndex(
                name: "FK_Vacancy_Object_CrossTable_Vacancies_idx",
                table: "VacancyObject_CT",
                column: "vacancy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BlobStorage");

            migrationBuilder.DropTable(
                name: "ContractAttachments");

            migrationBuilder.DropTable(
                name: "ContractTemplates");

            migrationBuilder.DropTable(
                name: "DevelopmentPlan");

            migrationBuilder.DropTable(
                name: "Enums");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "PersoneAddresses");

            migrationBuilder.DropTable(
                name: "PersoneDocuments");

            migrationBuilder.DropTable(
                name: "PersonePhones");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropTable(
                name: "RecipientsView");

            migrationBuilder.DropTable(
                name: "VacancyObject_CT");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Works");

            migrationBuilder.DropTable(
                name: "EnumTypes");

            migrationBuilder.DropTable(
                name: "FeedBackStatus");

            migrationBuilder.DropTable(
                name: "BuildObjects");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
