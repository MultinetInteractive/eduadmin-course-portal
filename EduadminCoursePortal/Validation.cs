using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduadminCoursePortal
{
    public static class Validation
    {
        public static bool IsCorrectCivilRegNr(string input)
        {
            input = (input ?? string.Empty).Trim().Replace("-", "");

            if (input.Any(c => !char.IsNumber(c)))
                return false;

            if (input.Length < 10)
                return false;

            if (input.Length > 12)
                return false;

            if (input.Length == 12)
                input = input.Substring(2);

            var withoutLastFourDigits = input.Substring(0, 6);
            
            withoutLastFourDigits = withoutLastFourDigits.Insert(0, "19").Insert(4, "-").Insert(7, "-");
            
            if (!DateTime.TryParse(withoutLastFourDigits, out var testDatetime))
                return false;

            if (input.Length != 10)
                return false;

            return CalculateControlNumber(input.Substring(0, input.Length - 1)) == int.Parse(input.Substring(input.Length - 1));
        }

        public static bool IsCorrectOrgnr(string input, out string message)
        {
            var sb = new StringBuilder();

            foreach (char c in input)
            {
                if (char.IsNumber(c))
                    sb.Append(c);
            }

            input = sb.ToString();

            if (input.Length == 12)
            {
                if (input.Substring(0, 2) == "16")
                    input = input.Substring(2);
            }

            if (input.Length == 10)
            {
                if (CalculateControlNumber(input.Substring(0, input.Length - 1)) == int.Parse(input.Substring(input.Length - 1)))
                {
                    if (int.Parse(input.Substring(2, 1)) >= 2)
                    {
                        if (input.StartsWith("2") || input.StartsWith("4") || input.StartsWith("5") || input.StartsWith("7") || input.StartsWith("8") || input.StartsWith("9"))
                        {
                            message = string.Empty;
                            return true;
                        }

                        message = "Felaktig startsiffra";
                    }
                    else
                        message = "Tredje siffran (månadens tiotal) måste vara större än eller lika med två";
                }
                else
                    message = "Felaktig kontrollsiffra";
            }
            else
                message = "Fel längd";

            return false;
        }

        public static int CalculateControlNumber(string idNumber)
        {
            var idNumberChars = idNumber.ToCharArray();

            var newNumber = 0;

            var include = true;

            for (int charCount = 0; charCount < idNumberChars.Length; charCount++)
            {
                var number = Convert.ToInt32(idNumberChars[charCount].ToString());
                if (include)
                    number *= 2;

                include = !include;

                if (number > 9)
                    number -= 9;

                newNumber += number;
            }

            var newNumberAsString = newNumber.ToString();

            var last = newNumberAsString.Substring(newNumberAsString.Length - 1);
            var controlNumber = 10 - Convert.ToInt32(last);

            if (controlNumber == 10)
                controlNumber = 0;

            return controlNumber;

        }
    }
}
