using System.Windows;
using System.Windows.Input;
using MainWindow.WcfServiceReference;

namespace FamilyEconomy
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        UserDatabaseCreationClient databaseCreationService;

        public RegistrationWindow()
        {
            InitializeComponent();

            MailTextBox.Focus();            
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            if (MailTextBox.Text == "" || LoginNameTextBox.Text == "" || PasswordTextBox.Text == "")
            {
                MessageBox.Show("Заполните, пожалуйста, каждое поле.");
                return;
            }

            databaseCreationService = new UserDatabaseCreationClient();
            databaseCreationService.CreateUserDataBase(LoginNameTextBox.Text);
            MailTextBox.Text = databaseCreationService.MethodForTest();

            //LoginWindow lg = new LoginWindow();
            //lg.InfoLabel.Content = "* Пожалуйста, введите временный пароль для первичной\n  авторизации. Пароль направлен на указанный почтовый\n  ящик.";
            //lg.Show();

            //this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow lg = new LoginWindow();
            lg.Show();

            this.Close();
        }

        private void MailTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoginNameTextBox.Focus();
            }
        }

        private void LoginNameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                PasswordTextBox.Focus();
            }
        }

        private void PasswordTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                RegistrationButton.Focus();
            }            
        }       
    }
}
