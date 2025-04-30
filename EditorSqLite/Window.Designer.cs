using Microsoft.AspNetCore.Components.WebView.WindowsForms;


namespace EditorSqLite
{
    partial class Window
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            blazorWebView = new BlazorWebView();
            blazorWebView1 = new BlazorWebView();
            SuspendLayout();
            // 
            // blazorWebView
            // 
            blazorWebView.Dock = DockStyle.Fill;
            blazorWebView.Location = new Point(0, 0);
            blazorWebView.Name = "blazorWebView";
            blazorWebView.Size = new Size(800, 450);
            blazorWebView.StartPath = "/";
            blazorWebView.TabIndex = 0;
            // 
            // blazorWebView1
            // 
            blazorWebView1.Dock = DockStyle.Fill;
            blazorWebView1.Location = new Point(0, 0);
            blazorWebView1.Margin = new Padding(3, 2, 3, 2);
            blazorWebView1.Name = "blazorWebView1";
            blazorWebView1.Size = new Size(700, 338);
            blazorWebView1.StartPath = "/";
            blazorWebView1.TabIndex = 0;
            blazorWebView1.Text = "blazorWebView1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(blazorWebView1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "LargeFileReader";
            ResumeLayout(false);
        }

        private BlazorWebView blazorWebView1;
    }
}
