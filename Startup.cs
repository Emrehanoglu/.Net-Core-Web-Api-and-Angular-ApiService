using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            services.AddScoped<ISocialRepository,SocialRepository>();
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = true; //parola da sayısal değer olsun.
                options.Password.RequireLowercase = true; //parola da küçük harf olsun. 
                options.Password.RequireUppercase = true; //parola da büyük harf olsun. 
                options.Password.RequireNonAlphanumeric = true; //parola da nokta,virgül gibi değerler olsun.
                options.Password.RequiredLength = 6; //parola min 6 karakter olsun

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //hesap 5 dk kitlenir
                options.Lockout.MaxFailedAccessAttempts = 5; //hesap 5 yanlış denemeden sonra kitlenir
                options.Lockout.AllowedForNewUsers = true; //yeni hesap olsutruna bir kişinin hesabı kitlenebilir

                options.User.AllowedUserNameCharacters="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //parolada olması gereken karakterler
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
            services.AddAuthentication(x=>{
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x=>{
                //gelen token bilgisini nasıl validate edeceğimi söylüyorum
                x.RequireHttpsMetadata = false; //jwt sadece https protokolünü kullanan isteklerden gelmesin
                x.SaveToken = true; //token bilgisi Server tarafında kaydedilsin
                x.TokenValidationParameters = new TokenValidationParameters{
                    //token bilgisinin 3. kısmı olan imza bilgisi kontrolü yapılsın diyorum
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Secret").Value)),
                    ValidateIssuer = false, //token 'ı kim olusturdu bilgisi kontrol edilmesin istiyorum
                    ValidateAudience = false //token 'ın kimin için olusturulduğu bilgisi kontrol edilmesin istiyorum
                };               
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,UserManager<User> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedDatabase.Seed(userManager).Wait(); //async metot old. için .Wait() dedim.
            }else{
                app.UseExceptionHandler(appError => {
                    //fırlatılan Exception bir request şeklinde gelecek
                    //bu noktada JSON formatında gelecek olan Exception 'ı
                    //bir response içerisinde dolduracağım.
                    //bu işlemi de middleware 'e dahil olup gercekleştiriyorum.
                    appError.Run(async context => {
                        //exception statuscode bilgisini respone içerisine aldım
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        
                        //exception türünü response içerisine aldım
                        context.Response.ContentType = "application/json";

                        //exception bilgisini response içerisine aldım
                        var contextError = context.Features.Get<IExceptionHandlerFeature>();
                    
                        //gercekten bir hata gelmiş ise
                        if(contextError != null){
                            //loglama yapılabilir

                            //middleware 'e hata yönetimi için dahil olduğum süreci sonlandırıyorum
                            //aşağıda ToString() olara ErrorDetails olusturdum,
                            //oluşan yapı ErrorDetails içerisinde Json formata dönüşecek.
                            await context.Response.WriteAsync(new ErrorDetails(){
                                StatusCode = context.Response.StatusCode,
                                Message = contextError.Error.Message
                            }.ToString());
                        }
                    });
                });
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
