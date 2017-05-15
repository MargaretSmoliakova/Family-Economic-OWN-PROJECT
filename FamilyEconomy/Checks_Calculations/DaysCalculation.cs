using FamilyEconomy.WcfDataServiceCommunicationLayer;
using MainWindow.WcfDataServiceReference;
using System;
using System.Windows;

namespace FamilyEconomy
{
    class DaysCalculation : Window
    {
        public int RecognizeAppropriateDay (int selectedDay, int month, int year)
        {
            int appropriateDay = selectedDay;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            while (daysInMonth < appropriateDay)
            {
                appropriateDay--;
            }

            return appropriateDay;
        }

        public int CountDaysInMonth()
        {
            UserDataTable userDataTable = new UserDataTable();
            UserData userDataEntitySelectedByUserDay = userDataTable.GetOneByName("selectedByUserDay");

            int firstDifferance = 0;
            int secondDifferance = 0;
            int selectedByUserDay = int.Parse(userDataEntitySelectedByUserDay.Value);
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int appropriateDayFirstMonth = 0;
            int appropriateDaySecondMonth = 0;                  

            int tempMonth = 0;
            int tempYear = DateTime.Now.Year;            

            if (DateTime.Now.Day < StartingDateWindow.datePickerDay)
            {
                tempMonth = DateTime.Now.Month - 1;

                if (tempMonth < 1)
                {
                    tempMonth = 12;
                    tempYear = DateTime.Now.Year - 1;
                }

                daysInMonth = DateTime.DaysInMonth(tempYear, tempMonth);

                if (selectedByUserDay > 28)
                {
                    appropriateDayFirstMonth = RecognizeAppropriateDay(selectedByUserDay, tempMonth, tempYear);
                    appropriateDaySecondMonth = StartingDateWindow.datePickerDay;
                }                 
            }
            else
            {
                tempMonth = DateTime.Now.Month + 1;

                if (tempMonth == 13)
                {
                    tempMonth = 1;
                    tempYear = DateTime.Now.Year + 1;
                }

                if (selectedByUserDay > 28)
                {
                    appropriateDayFirstMonth = StartingDateWindow.datePickerDay;
                    appropriateDaySecondMonth = RecognizeAppropriateDay(selectedByUserDay, tempMonth, tempYear);
                }
            }

            if (selectedByUserDay > 28)
            {
                if (selectedByUserDay - appropriateDayFirstMonth > 0)
                {
                    firstDifferance = selectedByUserDay - appropriateDayFirstMonth;
                    daysInMonth += firstDifferance;
                }
            }

            if (selectedByUserDay > 28)
            {
                if (selectedByUserDay - appropriateDaySecondMonth > 0)
                {
                    secondDifferance = selectedByUserDay - appropriateDaySecondMonth;
                    daysInMonth -= secondDifferance;
                }
            }
            
            return daysInMonth;
        }
    }
}
