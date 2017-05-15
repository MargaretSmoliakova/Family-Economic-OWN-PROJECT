using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FamilyEconomy.WcfDataServiceCommunicationLayer;
using MainWindow.WcfDataServiceReference;

namespace FamilyEconomy
{
    /// <summary>
    /// Interaction logic for NewCostWindow.xaml
    /// </summary>
    public partial class NewCostWindow : Window
    {
        int maxSizeNameMonthlyInDB;
        int maxSizeNamePermanentInDB;
        int maxSizeNameInDB;
        int maxSizeMonthlyGroupInDB;
        int maxSizePermanentGroupInDB;
        int maxSizeGroupInDB;
        int maxSizeAmount = 30;

        // WCF data service
        CurrentPricesTable currentPricesTable = new CurrentPricesTable();
        private CurrentPrice dependencePrice = new CurrentPrice();
        
        // delegates
        public delegate void ComboBoxItemShouldChangeHandler();
        public event ComboBoxItemShouldChangeHandler ItemShouldChange;

        public delegate void ViewTableShouldUpdateHandler();
        public event ViewTableShouldUpdateHandler ViewTableShouldUpdate;
                
        public NewCostWindow()
        {
            InitializeComponent();

            // Определяем максимальный размер для наименования и категории в зависимости от макс. размера соотв. col в БД
            
            maxSizeNameMonthlyInDB = 20;
            maxSizeMonthlyGroupInDB = 20;

            maxSizeNamePermanentInDB = 20;
            maxSizePermanentGroupInDB = 20;            
            

            nameTextBox.Focus();
        }

        private void NameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {                                                  
                amountTextBox.Focus();               
            }
        }

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tempText = amountTextBox.Text;
            Checking check = new Checking();

            tempText = check.Check(tempText);

            switch (tempText)
            {
                case "-1":
                    amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                    MessageBox.Show(check.getFirstError, "Некорректные данные");
                    return;
                case "-2":
                    amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                    MessageBox.Show(check.getSecondError.ToString(), "Некорректные данные");
                    return;
                default:
                    break;
            }

            amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void AmountTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CheckAmountTextBox();
                groupTextBox.Focus();
            }
        }
        
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            saveButton.Focus();
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на обязательные поля
            if (nameTextBox.Text == "")
            {
                nameTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                MessageBox.Show($"Пожалуйста, введите наименование расхода.", "Некорректный ввод данных");
                return;
            }
            else if (amountTextBox.Text == "")
            {
                amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                MessageBox.Show($"Пожалуйста, введите сумму расхода.", "Некорректный ввод данных");
                return;
            }

            string text = nameTextBox.Text;

            // Определяем максимально возможное количество знаков в БД в зависимости от того, постоянный это расход или разовый
            if (checkBox.IsChecked == true)
            {
                maxSizeNameInDB = maxSizeNamePermanentInDB;
                maxSizeGroupInDB = maxSizePermanentGroupInDB;
            }
            else
            {
                maxSizeNameInDB = maxSizeNameMonthlyInDB;
                maxSizeGroupInDB = maxSizeMonthlyGroupInDB;
            }

            // Проверка длинны наименования и категории
            if (text.Length > maxSizeNameInDB)
            {
                nameTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                MessageBox.Show($"Наименование расхода не должно превышать {maxSizeNameInDB} знаков.", "Некорректный ввод данных");
                return;
            }

            text = groupTextBox.Text;

            if (text.Length > maxSizeGroupInDB)
            {
                groupTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                MessageBox.Show($"Наименование категории не должно превышать {maxSizeGroupInDB} знаков.", "Некорректный ввод данных");
                return;
            }

            // Проверка суммы на правильность заполнения и запись в БД
            if (CheckAmountTextBox().Contains("-")) return;            

            if (RecordIntoDB() == -1) return;

            ViewTableShouldUpdate();

            Close();
        }



// ADDITIONAL method
        private string CheckAmountTextBox()
        {
            string tempText = amountTextBox.Text;
            Checking check = new Checking();
           
            tempText = check.CheckDigitsAfterComma(tempText);

            if (tempText == "-3")
            {
                amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                MessageBox.Show(check.getThirdError, "Некорректные данные");
                return tempText;
            }
            else if (tempText.Length > maxSizeAmount)
            {
                amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));
                MessageBox.Show($"Длина суммы не должна превышать {maxSizeAmount} символов.");
                return tempText = "-";                
            }
            else
            {
                amountTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }

            return tempText;
        }

        private int RecordIntoDB()
        {
            string name = nameTextBox.Text;
            double amount = double.Parse(amountTextBox.Text);
            string group = groupTextBox.Text;

            // Поиск расхода в постоянных и разовых
            dependencePrice = currentPricesTable.GetCurrentPriceWithDependenciesByName(name);            

            // Если имеется такой же расход в разовых, то просьба переименовать
            if (dependencePrice.MonthlyPrice != null && dependencePrice.MonthlyPrice.Name != null && dependencePrice.MonthlyPrice.Name == name)
            {
                MessageBox.Show("Наименование расхода совпадает с существующим разовым.\nПожалуйста, измените наименование.", "Совпадение наименований", MessageBoxButton.OK);
                nameTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));

                return -1;
            }
            // Если имеется такой же расход в постоянных, то просьба переименовать
            else if (dependencePrice.PermanentPrice != null && dependencePrice.PermanentPrice.Name != null && dependencePrice.PermanentPrice.Name == name)
            {
                MessageBox.Show("Наименование расхода совпадает с существующим постоянным.\nПожалуйста, измените наименование.", "Совпадение наименований", MessageBoxButton.OK);
                nameTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 213, 213));

                return -1;
            }

            // Решаем, куда записывать (в постоянный или разовый) в зависимости от checkBox
            if (checkBox.IsChecked == true)
            {                
                currentPricesTable.AddCurrentAndDependentPermanentPrice(name, amount.ToString(), group);

                ItemShouldChange();
                return 0;                
            }
            // Если checkBox пуст (создается разовый расход)
            else
            {                
                currentPricesTable.AddCurrentAndDependentMonthlyPrice(name, amount.ToString(), group);

                return 0;                
            }
        }



        // SELECT all the text in a convinient way
        private void SelectAddress(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }        
    }
}
