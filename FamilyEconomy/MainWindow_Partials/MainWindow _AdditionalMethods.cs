
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FamilyEconomy.WcfDataServiceCommunicationLayer;
using MainWindow.WcfDataServiceReference;

namespace FamilyEconomy
{
    // ADDITIONAL methods

    public partial class MainWindow : Window
    {
        private void ChangeComboboxItems()
        {
            PermanentPricesComboBox.Items.Clear();
            PermanentPricesComboBox.Items.Add("Изменить постоянные расходы");
            PermanentPricesComboBox.SelectedIndex = 0;

            foreach (string str in permanentPricesTable.GetAllNames())
            {
                PermanentPricesComboBox.Items.Add(str);
            }

            UpdateViewTable();
        }

        private void UpdateViewTable()
        {
            ShowAppropriateDBTable();
        }

        private void CountAndShowLimit()
        {   // этот метод создан для изменения limit при изменении бюджета

            int remainingDays = CountRemainingDays();

            if (remainingDays == 0) return;

            // Здесь считается лимит на каждый оставшийся день
            double limit = 0;
            double budget = 0;
            double spentToday = 0;

            budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);
            spentToday = double.Parse(userDataTable.GetOneByName(spentTodayDBColumnName).Value);

            if (budget < 0)
            {
                ReplenishBudgetUsingMargin();
                return;
            }

            limit = budget / remainingDays;
            limit = Math.Round(limit, 2, MidpointRounding.AwayFromZero);

            // Проверка на корректность полученных данных
            if (Check(budget.ToString()) == "-") return;
            if (Check(limit.ToString()) == "-") return;

            // Запись            
            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());
            userDataTable.ChangeValue(limitDBColumnName, limit.ToString());

            // Выведение данных           
            DisappointmentTextBox.Text = limit.ToString();

            dontRunHandler = true;
            MarginMoneyMonthTextBox.Text = budget.ToString();
            dontRunHandler = false;

            if (spentToday > limit) SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            if (spentToday <= limit) SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void CountAndShowLimitConsideringSpentToday()
        {   // этот метод создан для ежедневного обновления limit

            int remainingDays = CountRemainingDays();

            if (remainingDays == 0) return;

            // Здесь считается лимит на каждый оставшийся день
            double limit = 0;
            double budget = 0;
            double spentToday = 0;

            budget = double.Parse(userDataTable.GetOneByName(budgetDBColumnName).Value);
            spentToday = double.Parse(userDataTable.GetOneByName(spentTodayDBColumnName).Value);

            budget -= spentToday;

            if (budget < 0)
            {
                ReplenishBudgetUsingMargin();
                return;
            }

            limit = budget / remainingDays;
            limit = Math.Round(limit, 2, MidpointRounding.AwayFromZero);

            // Проверка на корректность полученных данных
            if (Check(budget.ToString()) == "-") return;
            if (Check(limit.ToString()) == "-") return;

            // Запись            
            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());
            userDataTable.ChangeValue(limitDBColumnName, limit.ToString());

            // Выведение данных           
            DisappointmentTextBox.Text = limit.ToString();

            dontRunHandler = true;
            MarginMoneyMonthTextBox.Text = budget.ToString();
            dontRunHandler = false;

            if (spentToday > limit) SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
            if (spentToday <= limit) SpentMoneyTodayTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private int DependanceOnWeekends()
        {// Проверяем попала ли сб или вс на дату обновления в этом и предыдущем/следующем месяце

            int selectedByUserDay = int.Parse(userDataTable.GetOneByName(selectedByUserDayDBColumnName).Value);
            string dayOfWeekThisMonth = null;
            string dayOfWeekAnotherMonth = null;
            int appropriateDateForAnotherMonth = 0;
            int thisMonth = 0;
            int year = DateTime.Now.Year;
            int anotherMonth = 0;
            DateTime tempAnotherDate;
            DaysCalculation daysCalculation = new DaysCalculation();

            // Определяем день недели для текущего месяца            
            thisMonth = DateTime.Now.Month;
            DateTime tempDate = DateTime.Parse(StartingDateWindow.datePickerDay.ToString() + "." + thisMonth.ToString() + "." + DateTime.Now.Year.ToString());
            dayOfWeekThisMonth = tempDate.DayOfWeek.ToString();

            // Определяем, какой месяц (кроме текущего) необходимо взять для учета выходных в зависимость от местоположения даты (до даты обновления учитывать вых. прошлого мес., после даты обновления учитывать вых сл. мес.)
            if (DateTime.Now.Day >= StartingDateWindow.datePickerDay)
            {
                anotherMonth = DateTime.Now.Month + 1;
                if (anotherMonth == 13)
                {
                    anotherMonth = 1;
                    year++;
                }
            }
            else
            {
                anotherMonth = DateTime.Now.Month - 1;
                if (anotherMonth == 0)
                {
                    anotherMonth = 12;
                    year--;
                }
            }

            // Определяем день недели для найденного месяца
            appropriateDateForAnotherMonth = daysCalculation.RecognizeAppropriateDay(selectedByUserDay, anotherMonth, year);
            tempAnotherDate = DateTime.Parse(appropriateDateForAnotherMonth.ToString() + "." + anotherMonth.ToString() + "." + year.ToString());
            dayOfWeekAnotherMonth = tempAnotherDate.DayOfWeek.ToString();

            // Считаем сб. и вс. в зависимости от месяца, который раньше
            if (tempDate < tempAnotherDate)
            {
                return CountSundaysAndSaturdays(dayOfWeekThisMonth, dayOfWeekAnotherMonth);
            }
            else
            {
                return CountSundaysAndSaturdays(dayOfWeekAnotherMonth, dayOfWeekThisMonth);
            }
        }

        private int CountRemainingDays()
        {
            // Определяем, в каком периоде находится дата для подсчета дней в месяце (например, если уст. начало на 10, а у нас 8, то количество дней в месяце считается от прошлого месяца, а не от текущего)
            int tempMonth = DateTime.Now.Month;
            if (DateTime.Now.Day < StartingDateWindow.datePickerDay) tempMonth--;

            int dependanceOnWeekends = 0;

            if (int.Parse(userDataTable.GetOneByName(considerWeekendsDBColumnName).Value) == 1)
            {
                dependanceOnWeekends += DependanceOnWeekends();
            }

            DaysCalculation checkPickedDay = new DaysCalculation();
            daysInMonth = checkPickedDay.CountDaysInMonth();
            daysInMonth += dependanceOnWeekends;

            // Здесь считаем оставшиеся дни до обновления данных (обновление раз в месяц в установленную пользователем дату)
            int remainingDays = 0;
            if (StartingDateWindow.datePickerDay <= DateTime.Now.Day) remainingDays = daysInMonth - (DateTime.Now.Day - StartingDateWindow.datePickerDay);
            else if (StartingDateWindow.datePickerDay > DateTime.Now.Day) remainingDays = StartingDateWindow.datePickerDay - DateTime.Now.Day;

            return remainingDays;
        }

        private int CountSundaysAndSaturdays(string dayOfWeekThisMonth, string dayOfWeekAnotherMonth)
        {
            int sundays = 0;
            int saturdays = 0;

            if (dayOfWeekThisMonth == "Sunday" || dayOfWeekThisMonth == "Saturday")
            {
                switch (dayOfWeekThisMonth)
                {
                    case "Sunday":
                        sundays += 2;
                        break;
                    case "Saturday":
                        saturdays++;
                        break;
                }
            }

            if (dayOfWeekAnotherMonth == "Sunday" || dayOfWeekAnotherMonth == "Saturday")
            {
                switch (dayOfWeekAnotherMonth)
                {
                    case "Sunday":
                        sundays -= 2;
                        break;
                    case "Saturday":
                        saturdays--;
                        break;
                }
            }

            return sundays + saturdays;
        }

        private string Check(string tempText)
        {
            Checking check = new Checking();
            tempText = check.Check(tempText);

            switch (tempText)
            {
                case "-1":
                    MessageBox.Show(check.getFirstError, "Некорректные данные");
                    return tempText = "-";
                case "-2":
                    MessageBox.Show(check.getSecondError.ToString(), "Некорректные данные");
                    return tempText = "-";
                default:
                    break;
            }

            tempText = check.CheckDigitsAfterComma(tempText);

            if (tempText == "-3")
            {
                MessageBox.Show(check.getThirdError, "Некорректные данные");
                return tempText = "-";
            }

            if (tempText.Length > maxSizePrices)
            {
                MessageBox.Show($"Длина числа не должна превышать {maxSizePrices} символов.");
                return tempText = "-";
            }

            return tempText;
        }

        private void SetToZeroAll()
        {
            userDataTable.SetToZeroAll();
        }

        private void ShowAppropriateDBTable()
        {
            switch (showDBTable)
            {
                case 1:
                    ShowMonthlyCostsButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    ShowPermanentCostsButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    ShowAllCostsButton.Background = new SolidColorBrush(Color.FromRgb(125, 170, 115));
                    CostsDataGrid.ItemsSource = currentPricesTable.GetAllUserFriendlyView();
                    break;
                case 2:
                    ShowMonthlyCostsButton.Background = new SolidColorBrush(Color.FromRgb(125, 170, 115));
                    ShowPermanentCostsButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    ShowAllCostsButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    CostsDataGrid.ItemsSource = currentPricesTable.GetMonthlyUserFriendlyView();
                    break;
                case 3:
                    ShowMonthlyCostsButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    ShowPermanentCostsButton.Background = new SolidColorBrush(Color.FromRgb(125, 170, 115));
                    ShowAllCostsButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    CostsDataGrid.ItemsSource = currentPricesTable.GetPermanentUserFriendlyView();
                    break;
            }

        }

        private void ReplenishBudgetUsingMargin()
        {
            MessageBox.Show("Вы превысили бюджет, рассчитанный на месяц.\nХотите пополнить бюджет посредством запасных средств?");

            double budget = 0;
            double limit = 0;

            userDataTable.ChangeValue(budgetDBColumnName, budget.ToString());
            userDataTable.ChangeValue(limitDBColumnName, limit.ToString());

            DisappointmentTextBox.Text = limit.ToString();
            MarginMoneyMonthTextBox.Text = budget.ToString();
        }

        private void ResetAll()
        {
            // Обнуляем данные
            SalaryTextBox.Text = "0";
            dontRunHandler = true;
            MarginMoneyMonthTextBox.Text = "0";
            dontRunHandler = false;
            SpentMoneyTodayTextBox.Text = "0";
            DisappointmentTextBox.Text = "0";

            SetToZeroAll();
            currentPricesTable.ResetAll();

            UpdateViewTable();

            // Считаем количество дней в этом месяце
            int dependanceOnWeekendsDifferance = 0;

            if (int.Parse(userDataTable.GetOneByName(considerWeekendsDBColumnName).Value) == 1)
            {
                dependanceOnWeekendsDifferance = DependanceOnWeekends();
            }

            DaysCalculation checkPickedDay = new DaysCalculation();

            daysInMonth = checkPickedDay.CountDaysInMonth();
            daysInMonth += dependanceOnWeekendsDifferance;
        }

        private void DailyBalanceCalculationAndNotification()
        {
            double difference = double.Parse(userDataTable.GetOneByName(limitDBColumnName).Value) - double.Parse(userDataTable.GetOneByName(spentTodayDBColumnName).Value);
            difference = Math.Round(difference, 2, MidpointRounding.AwayFromZero);
            if (difference > 0) MessageBox.Show("Вы сэкономили: " + difference + " рублей.");
            else if (difference < 0) MessageBox.Show("Вчера лимит превышен на: " + Math.Abs(difference) + " рублей.");

            CountAndShowLimitConsideringSpentToday();
        }
    }
}
