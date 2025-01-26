using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PromoCodeFactory.WebHost
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepository<Employee>, EfRepository<Employee>>();
            services.AddScoped<IRepository<Role>, EfRepository<Role>>();
            services.AddScoped<IRepository<Preference>, EfRepository<Preference>>();
            services.AddScoped<IRepository<Customer>, EfRepository<Customer>>();
            services.AddScoped<IRepository<PromoCode>, EfRepository<PromoCode>>();

            services.AddControllers();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory API Doc";
                options.Version = "1.0";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            SeedDatabase(app);

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private static void SeedDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(FakeDataFactory.Roles);
                    context.SaveChanges();
                }

                if (!context.Employees.Any())
                {
                    var roles = context.Roles.ToList();
                    foreach (var employee in FakeDataFactory.Employees)
                    {
                        employee.Role = roles.FirstOrDefault(r => r.Id == employee.Role.Id);
                        context.Employees.Add(employee);
                    }
                    context.SaveChanges();
                }

                if (!context.Preferences.Any())
                {
                    context.Preferences.AddRange(FakeDataFactory.Preferences);
                    context.SaveChanges();
                }

                if (!context.Customers.Any())
                {
                    var preferences = context.Preferences.ToList();
                    foreach (var customer in FakeDataFactory.Customers)
                    {
                        customer.CustomerPreferences.ForEach(cp =>
                        {
                            cp.Preference = preferences.First(pr => pr.Id == cp.PreferenceId);
                        });
                        context.Customers.Add(customer);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}