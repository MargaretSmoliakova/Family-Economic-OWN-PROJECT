using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FamilyEconomy.WcfDataServiceCommunicationLayer;

namespace FamilyEconomy
{
    /// <summary>
    /// Interaction logic for PermanentPricesWindow.xaml
    /// </summary>
    public partial class PermanentPricesWindow : Window
    {
        // WCF data service       
        private PermanentPricesTable permanentPricesTable = new PermanentPricesTable();

        public PermanentPricesWindow()
        {
            InitializeComponent();            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {// Проверяем на правильность символов по содержанию и количеству запятых

            Checking check = new Checking();
            string tempText = check.Check(TextBox.Text);

            switch (tempText)
            {
                case "-1":
                    MessageBox.Show(check.getFirstError, "Некорректные данные");
                    return;
                case "-2":
                    MessageBox.Show(check.getSecondError, "Некорректные данные");
                    return;
                default:
                    TextBox.Text = tempText;
                    return;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_Click(sender, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {// Проверяем на правильность символов по содержанию и количеству запятых еще раз, чтобы убедиться, что все ок

            Checking check = new Checking();
            string tempText = check.Check(TextBox.Text);

            switch (tempText)
            {
                case "-1":
                    MessageBox.Show(check.getFirstError, "Некорректные данные");
                    return;
                case "-2":
                    MessageBox.Show(check.getSecondError, "Некорректные данные");
                    return;
                default:                    
                    break;                   
            }
            
            // Проверяем количество знаков после запятой (должно быть не больше 2)
            tempText = check.CheckDigitsAfterComma(tempText);            

            if (tempText == "-3")
            {
                MessageBox.Show(check.getThirdError, "Некорректные данные");
                return;
            }
            
            double priceForDB = double.Parse(tempText);

            // обновляю запись в БД            
            permanentPricesTable.ChangePrice(Title, priceForDB.ToString());         

            this.Close();

            MessageBox.Show($"Сумма постоянного расхода изменена.\nИзменение будет учтено с {StartingDateWindow.datePickerDay} числа (даты начала месяца).", "Информационное сообщение");
        }

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
