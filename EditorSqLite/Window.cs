using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Database.Models;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;

namespace EditorSqLite
{
    public partial class Window : Form
    {
        private readonly IServiceProvider _services;
        private ListBox logListBox;

        public Window(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            InitializeComponent();

            // Taille de la fenêtre
            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(800, 600);
            this.MaximumSize = new Size(1920, 1080);

            // ListBox pour les logs JS
            logListBox = new ListBox
            {
                Dock = DockStyle.Bottom,
                Height = 200
            };
            this.Controls.Add(logListBox);

            // Configuration de BlazorWebView avec les services partagés
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = _services;
            blazorWebView1.RootComponents.Add<App>("#app");

            // Hook WebView2 pour intercepter console.log()
            blazorWebView1.WebView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, EventArgs e)
        {
            var core = blazorWebView1.WebView.CoreWebView2;
            if (core == null) return;

            core.WebMessageReceived += CoreWebView2_WebMessageReceived;
            core.AddScriptToExecuteOnDocumentCreatedAsync(@"
                window.console.log = (message) => {
                    window.chrome.webview.postMessage(message);
                };
            ");
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            logListBox.Items.Add($"[JS] {e.WebMessageAsJson}");
        }

        // Exemple d'utilisation du DbContext depuis un bouton Save dans un composant Blazor
        public async Task ImportFileAsync(string filePath)
        {
            var info = new FileInfo(filePath);

            // Upsert FileEntity et lecture en chunks
            using var scope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LargeFileReaderDbContext>();

            var fileEnt = await db.Files.FirstOrDefaultAsync(f => f.Path == filePath);
            if (fileEnt is null)
            {
                fileEnt = new FileEntity
                {
                    Name = Path.GetFileName(filePath),
                    Path = filePath
                };
                db.Files.Add(fileEnt);
            }
            else
            {
                db.Lines.RemoveRange(db.Lines.Where(l => l.IdFile == fileEnt.Id));
            }

            fileEnt.Size = info.Length;
            fileEnt.LastModified = info.LastWriteTime;
            await db.SaveChangesAsync();

            using var reader = info.OpenText();
            char[] buffer = new char[100];
            int read, pos = 0;
            var lines = new List<LineEntity>();
            while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                lines.Add(new LineEntity
                {
                    IdFile = fileEnt.Id,
                    Text = new string(buffer, 0, read),
                    Position = pos++
                });
            }
            db.Lines.AddRange(lines);
            await db.SaveChangesAsync();
        }
    }
}
