using System.Windows;
using System.ComponentModel;
using System;
using FamilyEconomy.WcfDataServiceCommunicationLayer;


namespace FamilyEconomy
{
    // FIELDS

    public partial class MainWindow : Window
    {
        // FIELDS static vars
        internal static int daysInMonth;
        internal static int showDBTable = 1;

        // FIELDS DB issues
        public static string considerWeekendsDBColumnName = "considerWeekends";
        public static string selectedByUserDayDBColumnName = "selectedByUserDay";
        public static string startDateDBColumnName = "startDate";
        public static string salaryDBColumnName = "salary";
        public static string budgetDBColumnName = "budget";
        public static string spentTodayDBColumnName = "spentToday";
        public static string limitDBColumnName = "limit";

        // FIELDS WCF (data) services issues
        UserDataTable userDataTable = new UserDataTable();
        PermanentPricesTable permanentPricesTable = new PermanentPricesTable();
        MonthlyPricesTable monthlyPricesTable = new MonthlyPricesTable();
        CurrentPricesTable currentPricesTable = new CurrentPricesTable();       

        // FIELDS date issues
        public DateTime CurrentDateAndTime { get; set; }
        public static object ConfigurationManager { get; private set; }

        // FIELDS additional vars
        bool dontRunHandler;
        int maxSizePrices = 30;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
