using System;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BLL.Models;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;

namespace EditorSqLite
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<LargeFileReaderDbContext>();
                    services.AddWindowsFormsBlazorWebView();
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LargeFileReaderDbContext>();
                db.Database.EnsureCreated();
            }

            var mainWindow = new Window(host.Services);
            Application.Run(mainWindow);
        }
    }
}
