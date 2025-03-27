using System.Windows;

namespace XArch.AppShell.Controls
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string ResponseText => InputBox.Text;

        public InputDialog(string prompt)
        {
            InitializeComponent();
            Prompt.Text = prompt;
        }

        private void Ok_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
