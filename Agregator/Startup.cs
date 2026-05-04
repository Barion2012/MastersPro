using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Agregator.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Twilio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Microsoft.IdentityModel.Logging;
using WebMarkupMin.AspNetCore5;
using WebMarkupMin;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNet.Common.UrlMatchers;
using WebMarkupMin.Core;
using WebMarkupMin.NUglify;
using System.IO.Compression;

namespace Agregator
{
    using Services;
//     using Api.Extensions;
    using Api.Settings;
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

          //  IdentityModelEventSource.ShowPII = true;

            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            services.Configure<TwilioVerifySettings>(Configuration.GetSection("Twilio"));

#if DESIGN
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection") 
                    , MariaDbServerVersion.Parse("10.3.22-mariadb")
                    , o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    )
                );
#else
            services.AddDbContext<ApplicationStoreContext>(options =>
            {
//                Microsoft.Data.SqlClient.SqlConnectionStringBuilder sqlConnectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(Configuration.GetConnectionString("StoreConnection"));
//                sqlConnectionString.Password = Configuration["Password"];


                options.UseSqlServer(Configuration.GetConnectionString("StoreConnection") //sqlConnectionString.ConnectionString
                    , o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    );
            });

#endif
            services.AddIdentity<AgregatorUser,Role>(options =>
            {
                
                options.SignIn.RequireConfirmedAccount = true;
                var passwprd = options.Password;
                passwprd.RequireDigit = true;
                passwprd.RequireLowercase = false;
                passwprd.RequiredLength = 6;
                passwprd.RequiredUniqueChars = 2;
                passwprd.RequireUppercase = false;
                passwprd.RequireNonAlphanumeric = false;                             
            })
              
            .AddRoles<Role>()
#if !DESIGN
            .AddEntityFrameworkStores<ApplicationStoreContext>()
#else
            .AddEntityFrameworkStores<ApplicationDbContext>()
#endif
            .AddErrorDescriber<RussianIdentityErrorDescriber>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()

            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-google";
            })
            .AddYandex(options =>
            {
                IConfigurationSection section = Configuration.GetSection("Authentication:Yandex");
                options.ClientId = section["ClientId"];
                options.ClientSecret = section["ClientSecret"];
                options.CallbackPath = "/signin-yandex";
            })
            .AddMailRu(options =>
            {
                IConfigurationSection mailRuAuthNSection = Configuration.GetSection("Authentication:MailRu");
                options.ClientId = mailRuAuthNSection["ClientId"];
                options.ClientSecret = mailRuAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-mailru";
            }).AddFacebook(facebookOptions =>
            {
                IConfigurationSection facebookAuthNSection = Configuration.GetSection("Authentication:Facebook");
                facebookOptions.AppId = facebookAuthNSection["ClientId"];
                facebookOptions.AppSecret = facebookAuthNSection["ClientSecret"];
                facebookOptions.CallbackPath = "/signin-facebook";
            })
            .AddVKontakte(options =>
            {
                IConfigurationSection section = Configuration.GetSection("Authentication:VK");
                options.ClientId = section["ClientId"];
                options.ClientSecret = section["ClientSecret"];
                options.CallbackPath = "/signin-vk";
            })
            .AddOdnoklassniki(options =>
            {
                IConfigurationSection section = Configuration.GetSection("Authentication:OK");
                options.ClientId = section["ClientId"];
                options.ClientSecret = section["ClientSecret"];
                options.CallbackPath = "/signin-ok";
            })

            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

            });

           services.ConfigureApplicationCookie(f => {
               f.LoginPath = "/login";
               f.LogoutPath = "/logout";
               });

            TwilioClient.Init(
                Configuration["Twilio:AccountSID"],
                Configuration["Twilio:AuthToken"]
                );

            services.AddControllersWithViews(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            })
            .AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ContractResolver = new DefaultContractResolver();
                o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddRazorRuntimeCompilation();

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddSignalR();

            services.AddSingleton<IFormatProvider>(new AppFormats());
            services.AddScoped(typeof(PdfUtil));

            services.AddDatabaseDeveloperPageExceptionFilter();

#if SERVER
            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.AllowCompressionInDevelopmentEnvironment = true;
            })
               .AddHtmlMinification(options =>
               {
                   options.ExcludedPages = new List<IUrlMatcher>
                   {
                        new WildcardUrlMatcher("/minifiers/x*ml-minifier"),
                        new ExactUrlMatcher("/contact")
                   };

                   HtmlMinificationSettings settings = options.MinificationSettings;
                   settings.RemoveRedundantAttributes = true;
                   settings.RemoveHttpProtocolFromAttributes = true;
                   settings.RemoveHttpsProtocolFromAttributes = true;

                   options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                   options.JsMinifierFactory = new NUglifyJsMinifierFactory();
               })
               .AddXhtmlMinification(options =>
               {
                   options.IncludedPages = new List<IUrlMatcher>
                   {
                        new WildcardUrlMatcher("/minifiers/x*ml-minifier"),
                        new ExactUrlMatcher("/contact")
                   };

                   XhtmlMinificationSettings settings = options.MinificationSettings;
                   settings.RemoveRedundantAttributes = true;
                   settings.RemoveHttpProtocolFromAttributes = true;
                   settings.RemoveHttpsProtocolFromAttributes = true;

                   options.CssMinifierFactory = new KristensenCssMinifierFactory();
                   options.JsMinifierFactory = new CrockfordJsMinifierFactory();
               })
               .AddXmlMinification(options =>
               {
                   XmlMinificationSettings settings = options.MinificationSettings;
                   settings.CollapseTagsWithoutContent = true;
               });
#endif
            services.AddSingleton<ISignService>(new SignService());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
#if !SERVER
                app.UseHsts();
                app.UseHttpsRedirection();
#endif
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

#if SERVER
            app.UseWebMarkupMin();
#endif

            app.UseStaticFiles();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("MAIN","{controller=Vacancy}/{action=Start}/{id?}");
                endpoints.MapHub<QLabHub>("/qlab");
                endpoints.MapControllerRoute("PersonalInfo","User/PersonalInfo",new { contoller="User",actin="PersonalInfo" });
            });
        }
    }
}
