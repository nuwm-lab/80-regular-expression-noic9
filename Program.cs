using System;
using System.Text.RegularExpressions;

namespace DateExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Заданий текст з різними датами
            string text = @"Важливі дати: 
            Перша подія відбулася 15/03/2023, а друга - 22-12-2022.
            Наступна зустріч запланована на 01/01/2024.
            Останнє оновлення було 31-05-2023 та 10/10/2023.
            Невірні формати: 2023/12/25, 5/3/23, 2023-12-01.
            Ще дати: 28/02/2024, 29-02-2024, 15/08/2023.";

            Console.WriteLine("=== ЗАДАНИЙ ТЕКСТ ===");
            Console.WriteLine(text);
            Console.WriteLine("\n=== ЗНАЙДЕНІ ДАТИ ===\n");

            // Регулярний вираз для пошуку дат у форматах dd/mm/yyyy або dd-mm-yyyy
            // \b - межа слова
            // \d{2} - рівно 2 цифри (день)
            // [/-] - роздільник (слеш або дефіс)
            // \d{2} - рівно 2 цифри (місяць)
            // [/-] - роздільник (слеш або дефіс)
            // \d{4} - рівно 4 цифри (рік)
            string pattern = @"\b\d{2}[/-]\d{2}[/-]\d{4}\b";

            // Створення об'єкта Regex
            Regex regex = new Regex(pattern);

            // Пошук всіх збігів у тексті
            MatchCollection matches = regex.Matches(text);

            // Виведення результатів
            if (matches.Count > 0)
            {
                Console.WriteLine($"Знайдено дат: {matches.Count}\n");

                int counter = 1;
                foreach (Match match in matches)
                {
                    Console.WriteLine($"{counter}. Дата: {match.Value}");
                    Console.WriteLine($"   Позиція: {match.Index}");
                    Console.WriteLine($"   Довжина: {match.Length}");
                    
                    // Додаткова перевірка валідності дати
                    if (IsValidDate(match.Value))
                    {
                        Console.WriteLine($"   Статус: ✓ Валідна дата");
                    }
                    else
                    {
                        Console.WriteLine($"   Статус: ✗ Невалідна дата (перевірте день/місяць)");
                    }
                    
                    Console.WriteLine();
                    counter++;
                }
            }
            else
            {
                Console.WriteLine("Дати не знайдено.");
            }

            // Демонстрація заміни дат
            Console.WriteLine("\n=== ЗАМІНА ДАТ НА PLACEHOLDER ===\n");
            string replacedText = regex.Replace(text, "[ДАТА]");
            Console.WriteLine(replacedText);

            Console.WriteLine("\n\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }

        // Допоміжний метод для перевірки валідності дати
        static bool IsValidDate(string dateString)
        {
            try
            {
                // Розділяємо дату на компоненти
                char separator = dateString.Contains('/') ? '/' : '-';
                string[] parts = dateString.Split(separator);

                if (parts.Length != 3)
                    return false;

                int day = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int year = int.Parse(parts[2]);

                // Перевірка валідності
                if (month < 1 || month > 12)
                    return false;

                if (day < 1 || day > DateTime.DaysInMonth(year, month))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
