using System;
using System.Text.RegularExpressions;

namespace RegularExpressionTask
{
    class DateExtractor
    {
        static void Main(string[] args)
        {
            // Заданий текст для обробки
            string text = @"
                Сьогодні 26/11/2025, а вчора було 25-11-2025. 
                Важливі дати: 01/01/2024, 15-03-2023, 31/12/2022. 
                Невалідні формати: 1/1/2023, 32/13/2025, 2025-11-26.
                Ще дати: 28/02/2024, 29-02-2024, 10/05/2023.
            ";

            Console.WriteLine("Заданий текст:");
            Console.WriteLine(text);
            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // Регулярний вираз для пошуку дат у форматах dd/mm/yyyy або dd-mm-yyyy
            // \b - межа слова
            // (0[1-9]|[12][0-9]|3[01]) - день (01-31)
            // [/-] - роздільник / або -
            // (0[1-9]|1[0-2]) - місяць (01-12)
            // [/-] - роздільник / або -
            // (\d{4}) - рік (4 цифри)
            string pattern = @"\b(0[1-9]|[12][0-9]|3[01])[/-](0[1-9]|1[0-2])[/-](\d{4})\b";

            // Створення об'єкта Regex
            Regex regex = new Regex(pattern);

            // Пошук всіх збігів у тексті
            MatchCollection matches = regex.Matches(text);

            // Виведення результатів
            Console.WriteLine($"Знайдено дат: {matches.Count}\n");

            if (matches.Count > 0)
            {
                Console.WriteLine("Знайдені дати:");
                int counter = 1;
                foreach (Match match in matches)
                {
                    Console.WriteLine($"{counter}.  {match.Value}");
                    
                    // Додаткова інформація про групи захоплення
                    Console.WriteLine($"   - День: {match.Groups[1]. Value}");
                    Console. WriteLine($"   - Місяць: {match.Groups[2].Value}");
                    Console.WriteLine($"   - Рік: {match.Groups[3].Value}");
                    Console.WriteLine($"   - Позиція: {match.Index}\n");
                    
                    counter++;
                }
            }
            else
            {
                Console.WriteLine("Дати не знайдено.");
            }

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // Демонстрація різних методів роботи з Regex
            DemonstrateRegexMethods(text, pattern);

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }

        static void DemonstrateRegexMethods(string text, string pattern)
        {
            Console.WriteLine("Демонстрація методів класу Regex:\n");

            // 1. IsMatch - перевірка наявності збігу
            bool hasMatch = Regex.IsMatch(text, pattern);
            Console.WriteLine($"1. IsMatch: У тексті {'є' if hasMatch else 'немає'} дат\n");

            // 2.  Match - пошук першого збігу
            Match firstMatch = Regex.Match(text, pattern);
            if (firstMatch.Success)
            {
                Console.WriteLine($"2. Match (перший збіг): {firstMatch.Value}\n");
            }

            // 3. Replace - заміна дат на інший формат
            string replaced = Regex.Replace(text, pattern, "[ДАТА]");
            Console.WriteLine("3. Replace (заміна дат на [ДАТА]):");
            Console.WriteLine(replaced. Trim());
            Console.WriteLine();

            // 4. Split - розділення тексту по датам
            string[] parts = Regex.Split(text, pattern);
            Console.WriteLine($"4. Split: Текст розділено на {parts.Length} частин\n");

            // 5.  Використання Regex з опціями
            Regex regexWithOptions = new Regex(
                pattern,
                RegexOptions. Compiled | RegexOptions. Multiline
            );
            MatchCollection matchesWithOptions = regexWithOptions.Matches(text);
            Console.WriteLine($"5.  Regex з опціями: знайдено {matchesWithOptions.Count} збігів");
        }
    }
}
