
using EbookSearch.Web.Components;

using EbooksIndex.ClassLibrary;
using EbooksIndex.ClassLibrary.DataAccess;
using EbooksIndex.ClassLibrary.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace EbookSearch.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            Log.Logger = new LoggerConfiguration() // needs Serilog.AspNetCore Nuget package
                .ReadFrom.Configuration(configuration) // needs Serilog.Settings.Configuration Nuget package
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            builder.Host.UseSerilog();

            builder.Services.AddSerilog();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();


            builder.Services.AddMemoryCache(options =>
            {
                // for now a maximum of 10 books in the cache sounds good.
                options.SizeLimit = 10;
                options.CompactionPercentage = 0.2;
            });

            builder.Services.Configure<OpenSearchAccessOptions>(builder.Configuration.GetSection("BooksOpenSearchOptions"));
            builder.Services.AddScoped<OpenSearchAccess>();
            builder.Services.AddScoped<IEbooksFinder, EbooksFinderOpenSearch>();
            builder.Services.AddScoped<IEbookRetriever, EbookRetrieverOpenSearch>();
            builder.Services.AddScoped<IRepositoryHelper, RepositoryHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
