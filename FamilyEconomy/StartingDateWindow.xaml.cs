using System;
using System.Windows;
using System.Windows.Controls;
using FamilyEconomy.WcfDataServiceCommunicationLayer;


namespace FamilyEconomy
{
    /// <summary>
    /// Interaction logic for StartingDateWindow.xaml
    /// </summary>
    public partial class StartingDateWindow : Window
    {
        public delegate void LimitShouldChangedHandler();
        public event LimitShouldChangedHandler LimitShouldChanged;

        private UserDataTable userDataTable = new UserDataTable();        

        internal static int datePickerDay;
        internal int takeIntoAccountWeekends;   
        
        public StartingDateWindow()
        {
            InitializeComponent();
            
            takeIntoAccountWeekends = int.Parse(userDataTable.GetOneByName(MainWindow.considerWeekendsDBColumnName).Value);            

            if (datePickerDay != 0) datePicker.SelectedDate = DateTime.Parse(datePickerDay + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString() + " 0:00:00");
            if (takeIntoAccountWeekends == 1) dependenceOnWeekendsCheckBox.IsChecked = true;
        }
        
        public void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // запись изначально выбранной даты в БД
            datePickerDay = datePicker.SelectedDate.Value.Day;

            userDataTable.ChangeValue(MainWindow.selectedByUserDayDBColumnName, datePickerDay.ToString());

            // проверка, не является ли выбранная дата 29, 30, 31 числом 
            if (datePickerDay > 28)
            {
                DaysCalculation checkPickedDay = new DaysCalculation();
                datePickerDay = checkPickedDay.RecognizeAppropriateDay(datePickerDay, DateTime.Now.Month, DateTime.Now.Year);
            }

            // запись результата проверки (если она была)            
            userDataTable.ChangeValue(MainWindow.startDateDBColumnName, datePickerDay.ToString());                 
        }

        private void DependenceOnWeekendsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            userDataTable.ChangeValue(MainWindow.considerWeekendsDBColumnName, "1");
            
            MainWindow.daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        }

        private void DependenceOnWeekendsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            userDataTable.ChangeValue(MainWindow.considerWeekendsDBColumnName, "0");

            MainWindow.daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            LimitShouldChanged();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            LimitShouldChanged();
            Close();
        }
    }
}
