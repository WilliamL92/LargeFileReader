using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;

namespace EditorSqLite
{
    public partial class Window : Form
    {
        private ListBox logListBox;

        public Window()
        {
            InitializeComponent();

            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(800, 600);
            this.MaximumSize = new Size(1920, 1080);

            // Ajouter une ListBox pour afficher les logs
            logListBox = new ListBox
            {
                Dock = DockStyle.Bottom,
                Height = 200
            };
            this.Controls.Add(logListBox);

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>(selector: "#app");

            // Initialiser WebView2 et capturer les logs
            blazorWebView1.WebView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, EventArgs e)
        {
            if (blazorWebView1.WebView.CoreWebView2 != null)
            {
                // Configurer un gestionnaire pour les messages envoyés depuis JavaScript
                blazorWebView1.WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

                // Injecter un script JavaScript pour rediriger les logs console
                blazorWebView1.WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
            window.console.log = (message) => {
                window.chrome.webview.postMessage(message);
            };
        ");
            }
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Ajouter les messages reçus dans la ListBox
            logListBox.Items.Add($"[JS Log] {e.WebMessageAsJson}");
        }
    }
}
