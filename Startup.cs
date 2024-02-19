using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowOrigins = "_myAllowOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SocialContext>(x=>x.UseSqlite("Data Source=social.db"));
            services.AddIdentity<User,Role>().AddEntityFrameworkStores<SocialContext>();
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = true; //parola da sayısal değer olsun.
                options.Password.RequireLowercase = true; //parola da küçük harf olsun. 
                options.Password.RequireUppercase = true; //parola da büyük harf olsun. 
                options.Password.RequireNonAlphanumeric = true; //parola da nokta,virgül gibi değerler olsun.
                options.Password.RequiredLength = 6; //parola min 6 karakter olsun

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //hesap 5 dk kitlenir
                options.Lockout.MaxFailedAccessAttempts = 5; //hesap 5 yanlış denemeden sonra kitlenir
                options.Lockout.AllowedForNewUsers = true; //yeni hesap olsutruna bir kişinin hesabı kitlenebilir

                options.User.AllowedUserNameCharacters="abcABC123-._@+"; //parolada olması gereken karakterler
                options.User.RequireUniqueEmail=true; //kullanıcıların mail adresleri aynı olamaz.
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors(options => {
                options.AddPolicy(
                    name: "_myAllowOrigins",
                    builder => {
                        builder
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MyAllowOrigins);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
