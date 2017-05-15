

using System.Windows;

namespace FamilyEconomy
{
    class Checking : Window
    {
        private const char DOT = '.';
        private const char COMMA = ',';

        private const string firstError = "Запись может содержать только один символ ',' и(или) должна состоять из цифр.\nЧисло должно быть положительным.";
        private const string secondError = "Запись должна состоять только из одного символа ','.";
        private const string thirdError = "Пожалуйста, оставьте после знака ',' 2 цифры.";

         public string Check(string text)
         {// Проверяем на правильность символов по содержанию и количеству запятых
            if (text == null) return text = "";
            
            int countComma = 0;
            
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == DOT || text[i] < '0' && text[i] != COMMA || text[i] > '9') return text = "-1";
                else if (text[i] == COMMA)
                {
                    countComma++;
                    if (countComma > 1) return text = "-2";
                }                
            }
            
            return text;

         }

        public string CheckDigitsAfterComma(string text)
        {// Проверяем количество знаком после запятой (должно быть не больше 2)
            int countDigitsAfterComma = 0;
            int countComma = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == COMMA) countComma++;
                if (countComma == 1 && text[i] != COMMA)
                {
                    countDigitsAfterComma++;
                    if (countDigitsAfterComma > 2) return text = "-3";
                }                
            }
                        
            if (countComma == 1 && countDigitsAfterComma == 0) return text = "-3";
           
            return text;
        }

        public string getFirstError
        {
            get
            {
                return firstError;
            } 
        }

        public string getSecondError
        {
            get
            {
                return secondError;
            }
        }

        public string getThirdError
        {
            get
            {
                return thirdError;
            }
        }
    }
}
