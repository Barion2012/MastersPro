using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Agregator
{
    public class Program
    {
/*        const string ROOT = "/home/www";
        private const string APP_NAME = "/home/boris/.microsoft/usersecrets/aspnet-Agregator-2A8727E1-3813-491A-85C5-E27102D7329D";
        private const string SECRET_CONFIG_FILE_NAME = "secrets.json";
        private static string CurrentDirectory
        {
            get { return Directory.GetParent(typeof(Program).Assembly.Location).FullName; }
        }

        private static string ConfigFileFullPath
        {
            get { return Path.Combine(APP_NAME, SECRET_CONFIG_FILE_NAME); }
        }
*/
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    /*
                                        if (Directory.Exists(ROOT))
                                        {

                                            webBuilder.UseKestrel(options =>
                                            {
                                                options.ListenLocalhost(48004);

                                            });
                                        }

                    #if !DESIGN


                                        webBuilder.ConfigureAppConfiguration((builder, options) =>
                                        {
                                            options.AddJsonFile(ConfigFileFullPath, optional: true, reloadOnChange: false);
                                        });
                    #endif
                    */
/*
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 443, conf =>
                        {
                           
                            conf.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                            conf.UseHttps("D:\\AKRUS\\ssl\\myshell\\myshell.pfx","agregator");
                        });

                    });
*/
                    webBuilder.UseStartup<Startup>();
                });
    }
}
