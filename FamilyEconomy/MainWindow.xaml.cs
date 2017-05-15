using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using MainWindow.WcfDataServiceReference;
using FamilyEconomy.WcfDataServiceCommunicationLayer.AdditionalClasses;

namespace FamilyEconomy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {            
            InitializeComponent();

            UpdateViewTable();
            ChangeComboboxItems();
            
            // Определяем datePickerDay (с учетом ситуации, если user числом обновления выбрал > 28)            
            if (int.Parse(userDataTable.GetOneByName(selectedByUserDayDBColumnName).Value) > 28)
            {
                DaysCalculation checkPickedDay = new DaysCalculation();
                StartingDateWindow.datePickerDay = checkPickedDay.RecognizeAppropriateDay(int.Parse(userDataTable.GetOneByName(selectedByUserDayDBColumnName).Value), DateTime.Now.Month, DateTime.Now.Year);
                                
                userDataTable.ChangeValue(startDateDBColumnName, StartingDateWindow.datePickerDay.ToString());                       
            }
            else StartingDateWindow.datePickerDay = int.Parse(userDataTable.GetOneByName(startDateDBColumnName).Value);

            // Заполняем поля существующими значениями из БД
            SalaryTextBox.Text = userDataTable.GetOneByName(salaryDBColumnName).Value;
            MarginMoneyMonthTextBox.Text = userDataTable.GetOneByName(budgetDBColumnName).Value;
            SpentMoneyTodayTextBox.Text = userDataTable.GetOneByName(spentTodayDBColumnName).Value;
            DisappointmentTextBox.Text = userDataTable.GetOneByName(limitDBColumnName).Value;
            
            // Запускаем таймер
            this.DataContext = this;

            DispatcherTimer dayTimer = new DispatcherTimer();
            dayTimer.Interval = TimeSpan.FromMilliseconds(100);
            dayTimer.Tick += new EventHandler(dayTimer_Tick);
            dayTimer.Start();
        }

        private void dayTimer_Tick(object sender, EventArgs e)
        {
            CurrentDateAndTime = DateTime.Now;

            // Ежедневное обновление
            if (CurrentDateAndTime.Hour == 0
                && CurrentDateAndTime.Minute == 0
                && CurrentDateAndTime.Second == 10)
            {
                if (int.Parse(userDataTable.GetOneByName(selectedByUserDayDBColumnName).Value) > 28)
                {
                    DaysCalculation checkPickedDay = new DaysCalculation();
                    StartingDateWindow.datePickerDay = checkPickedDay.RecognizeAppropriateDay(int.Parse(userDataTable.GetOneByName(selectedByUserDayDBColumnName).Value), DateTime.Now.Month, DateTime.Now.Year);

                    userDataTable.ChangeValue(startDateDBColumnName, StartingDateWindow.datePickerDay.ToString());                    
                }

                DailyBalanceCalculationAndNotification();

                // Обновляем textBox о потраченных сегодня средствах на 0 и запись
                SpentMoneyTodayTextBox.Text = "0";
                SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Green);

                userDataTable.ChangeValue(spentTodayDBColumnName, "0");                
            }

            // Ежемесячное обновление
            if (CurrentDateAndTime.Hour == 0
                && CurrentDateAndTime.Minute == 0
                && CurrentDateAndTime.Second == 10
                && CurrentDateAndTime.Day == StartingDateWindow.datePickerDay)
            {
                ResetAll();
            }

            PropertyChanged(this, new PropertyChangedEventArgs("CurrentDateAndTime"));
        }

        // Right side of the Window +  button establishing starting date          

        private void PermanentPricesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (PermanentPricesComboBox.SelectedIndex == 0) return;

            // Задаем Title новому окну            
            string comboBoxValue = (string)PermanentPricesComboBox.SelectedItem;
            PermanentPricesWindow setPermanentPricesWindow = new PermanentPricesWindow();
            setPermanentPricesWindow.Title = comboBoxValue;

            // Поиск в БД существующего значения постоянного расхода            
            setPermanentPricesWindow.TextBox.Text = permanentPricesTable.GetOneByName(comboBoxValue).Price;            

            PermanentPricesComboBox.SelectedIndex = 0;

            setPermanentPricesWindow.Show();            
        }        

        private void StartingDateButton_Click(object sender, RoutedEventArgs e)
        {
            StartingDateWindow startingDateWindow = new StartingDateWindow();
            startingDateWindow.Show();

            startingDateWindow.LimitShouldChanged += CountAndShowLimit;
        }

        private void AddCostsButton_Click(object sender, RoutedEventArgs e)
        {
            NewCostWindow newCostWindow = new NewCostWindow();
            newCostWindow.Show();

            newCostWindow.ItemShouldChange += ChangeComboboxItems;
            newCostWindow.ViewTableShouldUpdate += UpdateViewTable;            
        }
        
        private void ShowAllCostsButton_Click(object sender, RoutedEventArgs e)
        {
            showDBTable = 1;
            ShowAppropriateDBTable();
        }

        private void ShowMonthlyCostsButton_Click(object sender, RoutedEventArgs e)
        {
            showDBTable = 2;
            ShowAppropriateDBTable();
        }

        private void ShowPermanentCostsButton_Click(object sender, RoutedEventArgs e)
        {
            showDBTable = 3;
            ShowAppropriateDBTable();
        }

        private void CostsDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetFocusedRow(e);

            ContextMenu cm = FindResource("EditAndDelDataGrid") as ContextMenu;
            DataGridCell dgc = sender as DataGridCell;
            cm.PlacementTarget = dgc;
            cm.IsOpen = true;
        }
        
        private void SetFocusedRow(MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null) return;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;
                cell.Focus();

                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                DataGridRow row = dep as DataGridRow;
                CostsDataGrid.SelectedItem = row.DataContext;
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentPriceUserFriendlyView currentEntity = CostsDataGrid.SelectedItem as CurrentPriceUserFriendlyView;

            if (currentEntity == null) return;
            if (currentEntity.Check_box == 1)
            {
                MessageBox.Show($"Расход '{currentEntity.Name}' с отметкой 'УЧТЕН'и не может быть изменен.\nДля редактирования, пожалуйста, измените отметку на 'НЕ УЧТЕН'.");
                return;
                
            }

            EditCostWindow editCostWindow = new EditCostWindow(currentEntity);
            editCostWindow.Show();

            editCostWindow.ItemShouldChange += ChangeComboboxItems;
            editCostWindow.ViewTableShouldUpdate += UpdateViewTable;            
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CurrentPriceUserFriendlyView currentPriceEntity = CostsDataGrid.SelectedItem as CurrentPriceUserFriendlyView;

            if (currentPriceEntity == null) return;
            if (currentPriceEntity.Consideration == 1)
            {
                if (MessageBox.Show($"Расход '{currentPriceEntity.Name}' с отметкой 'УЧТЕН'.\nОставить учтенным его в текущем месяце после удаления?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {                    
                    currentPricesTable.DeleteCurrentPriceWithDependencies(currentPriceEntity.Name);

                    ChangeComboboxItems();
                    UpdateViewTable();
                }
                else
                {
                    OffConsider(sender, e);

                    currentPricesTable.DeleteCurrentPriceWithDependencies(currentPriceEntity.Name);

                    ChangeComboboxItems();
                    UpdateViewTable();
                }
            }
            else
            {
                if (MessageBox.Show($"Вы действительно хотите удалить расход '{currentPriceEntity.Name}'.", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    currentPricesTable.DeleteCurrentPriceWithDependencies(currentPriceEntity.Name);

                    ChangeComboboxItems();
                    UpdateViewTable();
                }
            }          
        }

        void OnChecked(object sender, RoutedEventArgs e)
        {           
            CurrentPriceUserFriendlyView currentEntity = CostsDataGrid.SelectedItem as CurrentPriceUserFriendlyView;            
            if (currentEntity == null) return;            
            
            if (currentPricesTable.GetOneByName(currentEntity.Name).Check_box == 1) return;

            // Запись в БД, что user выбрал расход              
            currentPricesTable.ChangeCheckBox(currentEntity.Name, 1);            
        }

        void OffChecked(object sender, RoutedEventArgs e)
        {
            CurrentPriceUserFriendlyView currentEntity = CostsDataGrid.SelectedItem as CurrentPriceUserFriendlyView;

            if (currentEntity == null) return;

            // Запись в БД, что user отменил выбор расхода           
            currentPricesTable.ChangeCheckBox(currentEntity.Name, 0);
        }

        void OnConsider(object sender, RoutedEventArgs e)
        {
            double price = 0;
            double newBudget = 0;

            CurrentPriceUserFriendlyView currentEntity = CostsDataGrid.SelectedItem as CurrentPriceUserFriendlyView;
            if (currentEntity == null) return;

            if (currentPricesTable.GetOneByName(currentEntity.Name).Consideration == 1) return;

            // Вычисляем бюджет с изменениями и записываем в БД
            price = double.Parse(currentEntity.Price);
            newBudget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value) - price;

            userDataTable.ChangeValue(budgetDBColumnName, newBudget.ToString());

            MarginMoneyMonthTextBox.Text = newBudget.ToString();

            // Запись в БД, что user учел расход              
            currentPricesTable.ChangeConsideration(currentEntity.Name, 1);
        }

        void OffConsider(object sender, RoutedEventArgs e)
        {
            double price = 0;
            double newBudget = 0;

            CurrentPriceUserFriendlyView currentEntity = CostsDataGrid.SelectedItem as CurrentPriceUserFriendlyView;

            if (currentEntity == null) return;

            // Вычисляем бюджет с изменениями и записываем в БД
            price = double.Parse(currentEntity.Price);
            newBudget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value) + price;

            userDataTable.ChangeValue(budgetDBColumnName, newBudget.ToString());

            MarginMoneyMonthTextBox.Text = newBudget.ToString();

            // Запись в БД, что user отменил учет расхода           
            currentPricesTable.ChangeConsideration(currentEntity.Name, 0);
        }


        // Left side of the Window (except the button establishing starting date)      

        // Salary
        private void SalaryTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {                
                // Проверка на правильность введенных данных        
                if (Check(SalaryTextBox.Text) == "-") return;

                // Проверка на пустоту данных            
                if (SalaryTextBox.Text == "")
                {                    
                    userDataTable.ChangeValue(salaryDBColumnName, "0");               
                }
                if (SalaryChangeTextBox.Text == "") SalaryChangeTextBox.Text = "0";
                if (MarginMoneyMonthTextBox.Text == "")
                {                    
                    userDataTable.ChangeValue(budgetDBColumnName, "0");
                    MarginMoneyMonthTextBox.Text = "0";
                }

                // Получение необходимых для вычисления данных из БД
                double salary;
                double budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);

                // Суммирование
                salary = double.Parse(SalaryTextBox.Text);
                budget += salary;

                // Запись в БД
                userDataTable.ChangeValue(salaryDBColumnName, salary.ToString());
                userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());

                // Вывод полученных данных
                SalaryTextBox.Text = salary.ToString();
                MarginMoneyMonthTextBox.Text = budget.ToString();
            }
        }

        private void SalaryChangeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SalaryChangePlusButton_Click(sender, e);
            }
        }

        private void SalaryChangePlusButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на правильность введенных данных        
            if (Check(SalaryChangeTextBox.Text) == "-") return;

            // Проверка на пустоту данных            
            if (SalaryTextBox.Text == "")
            {
                userDataTable.ChangeValue(salaryDBColumnName, "0");
                SalaryTextBox.Text = "0";
            }
            if (SalaryChangeTextBox.Text == "") SalaryChangeTextBox.Text = "0";
            if (MarginMoneyMonthTextBox.Text == "")
            {
                userDataTable.ChangeValue(budgetDBColumnName, "0");
                MarginMoneyMonthTextBox.Text = "0";
            }

            // Получение необходимых для вычисления данных из БД
            double salary = double.Parse(userDataTable.GetOneByName(salaryDBColumnName).Value);
            double budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);

            // Суммирование
            salary += double.Parse(SalaryChangeTextBox.Text);
            budget += double.Parse(SalaryChangeTextBox.Text);

            // Запись в БД            
            userDataTable.ChangeValue(salaryDBColumnName, salary.ToString());
            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());

            // Вывод полученных данных
            SalaryTextBox.Text = salary.ToString();
            MarginMoneyMonthTextBox.Text = budget.ToString();
        }

        private void SalaryChangeMinusButton_Click(object sender, RoutedEventArgs e)
        {            
            // Проверка на правильность введенных данных        
            if (Check(SalaryChangeTextBox.Text) == "-") return;

            // Проверка на пустоту данных
            if (SalaryTextBox.Text == "")
            {                
                userDataTable.ChangeValue(salaryDBColumnName, "0");
                SalaryTextBox.Text = "0";
            }
            if (SalaryChangeTextBox.Text == "") SalaryChangeTextBox.Text = "0";
            if (MarginMoneyMonthTextBox.Text == "")
            {                
                MarginMoneyMonthTextBox.Text = "0";
                userDataTable.ChangeValue(budgetDBColumnName, "0");
                MarginMoneyMonthTextBox.Text = "0";
            }

            // Получение необходимых для вычисления данных из БД
            double salary = double.Parse(userDataTable.GetOneByName(salaryDBColumnName).Value);
            double budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);

            // Вычисление
            salary -= double.Parse(SalaryChangeTextBox.Text);
            budget -= double.Parse(SalaryChangeTextBox.Text);

            // Проверка полученных результатов
            if (Check(salary.ToString()) == "-") return;
            if (Check(budget.ToString()) == "-") return;

            // Запись в БД
            userDataTable.ChangeValue(salaryDBColumnName, salary.ToString());
            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());

            // Вывод полученных данных
            SalaryTextBox.Text = salary.ToString();
            MarginMoneyMonthTextBox.Text = budget.ToString();
        }

        // Budget for month
        private void MarginMoneyMonthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dontRunHandler) return;

            if (MarginMoneyMonthTextBox.Text == "")
            {                
                userDataTable.ChangeValue(budgetDBColumnName, "0");
                MarginMoneyMonthTextBox.Text = "0";                
            }

            CountAndShowLimit();
        }

        private void MarginMonthChangeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MarginMonthChangePlusButton_Click(sender, e);
            }
        }

        private void MarginMonthChangePlusButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на правильность введенных данных        
            if (Check(MarginMonthChangeTextBox.Text) == "-") return;

            // Проверка на пустоту данных   
            if (MarginMoneyMonthTextBox.Text == "")
            {
                userDataTable.ChangeValue(budgetDBColumnName, "0");
                MarginMoneyMonthTextBox.Text = "0";
            }
            if (MarginMonthChangeTextBox.Text == "") MarginMonthChangeTextBox.Text = "0";

            // Получение необходимых для вычисления данных из БД            
            double budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);

            // Суммирование            
            budget += double.Parse(MarginMonthChangeTextBox.Text);

            // Запись в БД            
            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());

            // Вывод полученных данных            
            MarginMoneyMonthTextBox.Text = budget.ToString();
        }

        private void MarginMonthChangeMinusButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на правильность введенных данных        
            if (Check(MarginMonthChangeTextBox.Text) == "-") return;

            // Проверка на пустоту данных
            if (MarginMoneyMonthTextBox.Text == "")
            {                
                userDataTable.ChangeValue(budgetDBColumnName, "0");
                MarginMoneyMonthTextBox.Text = "0";
            }
            if (MarginMonthChangeTextBox.Text == "") MarginMonthChangeTextBox.Text = "0";

            // Получение необходимых для вычисления данных из БД            
            double budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);

            // Вычисление           
            budget -= double.Parse(MarginMonthChangeTextBox.Text);

            // Проверка полученных результатов            
            if (Check(budget.ToString()) == "-") return;

            // Запись в БД            
            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());

            // Вывод полученных данных            
            MarginMoneyMonthTextBox.Text = budget.ToString();
        }

        // Spent money today
        private void SpentTodayChangeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SpentTodayChangePlusButton_Click(sender, e);
            }
        }

        private void SpentTodayChangePlusButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на правильность введенных данных            
            if (Check(SpentTodayChangeTextBox.Text) == "-") return;

            // Проверка на пустоту данных   
            if (SpentMoneyTodayTextBox.Text == "")
            {
                SpentMoneyTodayTextBox.Text = "0";                
                userDataTable.ChangeValue(spentTodayDBColumnName, "0");
            }
            if (SpentTodayChangeTextBox.Text == "") SpentTodayChangeTextBox.Text = "0";

            // Получение необходимых для вычисления данных из БД            
            double spentToday = double.Parse(userDataTable.GetOneByName(spentTodayDBColumnName).Value);

            // Суммирование            
            spentToday += double.Parse(SpentTodayChangeTextBox.Text);

            // Запись в БД            
            userDataTable.ChangeValue(spentTodayDBColumnName, spentToday.ToString());

            // Вывод полученных данных            
            SpentMoneyTodayTextBox.Text = spentToday.ToString();

            if (spentToday > double.Parse(userDataTable.GetOneByName(limitDBColumnName).Value)) SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Red);

        }

        private void SpentTodayChangeMinusButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на правильность введенных данных        
            if (Check(SpentTodayChangeTextBox.Text) == "-") return;

            // Проверка на пустоту данных
            if (SpentMoneyTodayTextBox.Text == "")
            {
                SpentMoneyTodayTextBox.Text = "0";
                userDataTable.ChangeValue(spentTodayDBColumnName, "0");
            }
            if (SpentTodayChangeTextBox.Text == "") SpentTodayChangeTextBox.Text = "0";

            // Получение необходимых для вычисления данных из БД            
            double spentToday = double.Parse(userDataTable.GetOneByName(spentTodayDBColumnName).Value);

            // Вычисление           
            spentToday -= double.Parse(SpentTodayChangeTextBox.Text);

            // Проверка полученных результатов            
            if (Check(spentToday.ToString()) == "-") return;

            // Запись в БД            
            userDataTable.ChangeValue(spentTodayDBColumnName, spentToday.ToString());

            // Вывод полученных данных            
            SpentMoneyTodayTextBox.Text = spentToday.ToString();

            if (spentToday < double.Parse(userDataTable.GetOneByName(limitDBColumnName).Value)) SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
            
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



        // CLOSE
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

    }           
}

