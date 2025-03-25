using System.IO;

namespace XArch.AppShell.Markdown
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using Markdig;

    using Microsoft.Web.WebView2.Core;

    public partial class MarkdownEditorControl : UserControl
    {
        private string _filePath;

        public MarkdownEditorControl(string filePath)
        {
            InitializeComponent();
            _filePath = filePath;

            this.Loaded += MarkdownEditorControl_Loaded;

            if (File.Exists(_filePath))
                Editor.Text = File.ReadAllText(_filePath);
        }

        private async void MarkdownEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            await PreviewBrowser.EnsureCoreWebView2Async();

            PreviewBrowser.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

            UpdatePreview(Editor.Text);
        }

        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // Navigation in edit mode isn't supported at the moment
            var uri = e.Uri;

            // Only allow data URIs generated from Markdown inside the viewer
           if (!string.IsNullOrWhiteSpace(uri) &&
                !uri.StartsWith("about:", StringComparison.OrdinalIgnoreCase) && 
                !uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                e.Cancel = true;

                //if (uri.StartsWith("https://"))
                //{
                //    try
                //    {
                //        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                //        {
                //            FileName = uri,
                //            UseShellExecute = true
                //        });
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show($"Unable to open link: {uri}\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    }
                //} 
                //else
                //{
                //    MessageBox.Show($"Unable to open link: {uri}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }
        }


        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview(Editor.Text);
        }

        private void UpdatePreview(string markdown)
        {
            if (PreviewBrowser.CoreWebView2 == null) return;

            var html = Markdown.ToHtml(markdown);
            var styled = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{
                            font-family: sans-serif;
                            margin: 1rem;
                            line-height: 1.5;
                            background-color: #1e1e1e;
                            color: #d4d4d4;
                        }}
                        h1, h2, h3 {{
                            color: #4fc1ff;
                        }}
                        a {{
                            color: #569cd6;
                            text-decoration: underline;
                        }}
                        code {{
                            background-color: #2d2d2d;
                            color: #ce9178;
                            padding: 0.2rem;
                            border-radius: 4px;
                        }}
                        pre {{
                            background-color: #2d2d2d;
                            padding: 0.5rem;
                            overflow-x: auto;
                            border-radius: 6px;
                        }}
                        blockquote {{
                            border-left: 4px solid #3a3a3a;
                            margin: 0.5rem 0;
                            padding-left: 1rem;
                            color: #999;
                        }}
                        table {{
                            border-collapse: collapse;
                            margin-top: 1rem;
                        }}
                        th, td {{
                            border: 1px solid #333;
                            padding: 0.5rem;
                        }}
                        th {{
                            background-color: #2b2b2b;
                        }}
                    </style>
                </head>
                <body>
                    {html}
                </body>
                </html>
                ";
            PreviewBrowser.NavigateToString(styled);
        }
    }
}
