using System.Text;
using cw3.Middlewares;
using cw3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace cw3
{
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "JakubSpZoo",
                        ValidAudience = "students",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(Configuration["SecretKey"]))
                    };
                });
            services.AddScoped<IStudentsDbService, SqlServerDbService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsDbService dbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.Use(async (context, next) =>
            // {
            //     if (!context.Request.Headers.ContainsKey("Index"))
            //     {
            //         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //         await context.Response.WriteAsync("Nie podano indeksu w naglowku!");
            //         return;
            //     }
            //     else
            //     {
            //         var index = context.Request.Headers["Index"].ToString();
            //         var czyIstnieje = dbService.checkIfStudentExists(index);
            //
            //         if (!czyIstnieje)
            //         {
            //             context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //             await context.Response.WriteAsync("Student o podanym indeksie nie istnieje w bazie danych");
            //             return;
            //         }
            //     }
            //
            //     await next();
            // });

            app.UseMiddleware<LoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
