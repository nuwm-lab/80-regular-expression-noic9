using System;
using System.Collections.Generic;
using System. Globalization;
using System. Linq;
using System.Text. RegularExpressions;

namespace RegularExpressionTask
{
    class DateExtractor
    {
        // Статичні regex для продуктивності (compiled один раз)
        private static readonly Regex DateRegex = new Regex(
            @"\b(?<day>0[1-9]|[12][0-9]|3[01])(? <separator>[/-])(?<month>0[1-9]|1[0-2])\k<separator>(?<year>\d{4})\b",
            RegexOptions.Compiled | RegexOptions. Multiline
        );

        private static readonly Regex IpRegex = new Regex(
            @"\b(?<octet1>25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\. (?<octet2>25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?<octet3>25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?<octet4>25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b",
            RegexOptions. Compiled | RegexOptions. Multiline
        );

        private static readonly Regex EmailRegex = new Regex(
            @"\b(? <username>[a-zA-Z0-9. _%+-]+)@(?<domain>[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})\b",
            RegexOptions.Compiled | RegexOptions. IgnoreCase
        );

        static void Main(string[] args)
        {
            // Заданий текст для обробки
            string text = @"
                Сьогодні 26/11/2025, а вчора було 25-11-2025.  
                Важливі дати: 01/01/2024, 15-03-2023, 31/12/2022.  
                Невалідні формати: 1/1/2023, 32/13/2025, 2025-11-26, 29/02/2023, 31/04/2023. 
                Валідні дати: 28/02/2024, 29-02-2024, 10/05/2023. 
                IP-адреси: 192. 168.0.1, 10.0.0.1, 255.255.255.255, 256.1.1.1 (невалідна). 
                Email: user@example.com, test. email@domain.co.uk, invalid@. com.
            ";

            Console.OutputEncoding = System.Text. Encoding.UTF8;
            Console.WriteLine("Заданий текст:");
            Console. WriteLine(text);
            Console. WriteLine("\n" + new string('=', 70) + "\n");

            // Практичне завдання: пошук кількох типів шаблонів
            var patterns = new Dictionary<string, (Regex Regex, string Description)>
            {
                { "Дати", (DateRegex, "Дати у форматі dd/mm/yyyy або dd-mm-yyyy") },
                { "IP-адреси", (IpRegex, "IP-адреси у форматі xxx.xxx.xxx.xxx") },
                { "Email", (EmailRegex, "Email адреси") }
            };

            var allResults = new Dictionary<string, List<MatchInfo>>();

            // Пошук всіх типів шаблонів
            foreach (var pattern in patterns)
            {
                var matches = FindMatches(text, pattern. Value.Regex);
                allResults[pattern.Key] = matches;
            }

            // Виведення загального звіту
            PrintOverallReport(allResults);

            Console.WriteLine("\n" + new string('=', 70) + "\n");

            // Детальний аналіз дат з валідацією
            Console.WriteLine("ДЕТАЛЬНИЙ АНАЛІЗ ДАТ З ВАЛІДАЦІЄЮ:\n");
            AnalyzeDates(text);

            Console.WriteLine("\n" + new string('=', 70) + "\n");

            // Демонстрація різних методів роботи з Regex
            DemonstrateRegexMethods(text);

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }

        // Структура для зберігання інформації про знайдений збіг
        private class MatchInfo
        {
            public string Value { get; set; }
            public int Position { get; set; }
            public Dictionary<string, string> Groups { get; set; }
            public bool IsValid { get; set; }
            public string ValidationMessage { get; set; }

            public MatchInfo()
            {
                Groups = new Dictionary<string, string>();
                IsValid = true;
            }
        }

        // Метод для пошуку збігів
        private static List<MatchInfo> FindMatches(string text, Regex regex)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<MatchInfo>();
            }

            var results = new List<MatchInfo>();

            try
            {
                MatchCollection matches = regex.Matches(text);

                foreach (Match match in matches)
                {
                    var matchInfo = new MatchInfo
                    {
                        Value = match.Value,
                        Position = match.Index
                    };

                    // Збереження іменованих груп
                    foreach (string groupName in regex.GetGroupNames())
                    {
                        if (groupName != "0" && int.TryParse(groupName, out _) == false)
                        {
                            matchInfo.Groups[groupName] = match.Groups[groupName].Value;
                        }
                    }

                    results.Add(matchInfo);
                }
            }
            catch (RegexMatchTimeoutException ex)
            {
                Console. WriteLine($"Помилка: перевищено час очікування regex: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Помилка: невалідний regex шаблон: {ex.Message}");
            }

            return results;
        }

        // Метод для виведення загального звіту
        private static void PrintOverallReport(Dictionary<string, List<MatchInfo>> results)
        {
            Console. WriteLine("ЗАГАЛЬНИЙ ЗВІТ ПО ЗНАЙДЕНИХ ШАБЛОНАХ:\n");

            int totalMatches = 0;

            foreach (var result in results)
            {
                int count = result.Value.Count;
                totalMatches += count;

                Console.WriteLine($"┌─ {result.Key}");
                Console.WriteLine($"│  Знайдено: {count}");

                if (count > 0)
                {
                    Console. WriteLine($"│  Приклади:");
                    foreach (var match in result.Value. Take(3))
                    {
                        Console.WriteLine($"│    • {match.Value}");
                    }
                    if (count > 3)
                    {
                        Console.WriteLine($"│    ... та ще {count - 3}");
                    }
                }

                Console.WriteLine("│");
            }

            Console.WriteLine($"└─ ВСЬОГО ЗНАЙДЕНО: {totalMatches} збігів\n");
        }

        // Детальний аналіз дат з валідацією
        private static void AnalyzeDates(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Текст порожній або null.");
                return;
            }

            var matches = FindMatches(text, DateRegex);

            Console.WriteLine($"Знайдено потенційних дат: {matches.Count}\n");

            if (matches.Count == 0)
            {
                Console.WriteLine("Дати не знайдено.");
                return;
            }

            int validCount = 0;
            int invalidCount = 0;

            foreach (var match in matches)
            {
                // Валідація дати
                ValidateDate(match);

                string statusIcon = match.IsValid ? "✓" : "✗";
                string statusColor = match.IsValid ? "Валідна" : "Невалідна";

                Console. WriteLine($"{statusIcon} {match.Value}");
                Console.WriteLine($"   Статус: {statusColor}");

                if (match.Groups.ContainsKey("day"))
                {
                    Console.WriteLine($"   День: {match.Groups["day"]}");
                    Console.WriteLine($"   Місяць: {match.Groups["month"]}");
                    Console.WriteLine($"   Рік: {match.Groups["year"]}");
                    Console.WriteLine($"   Роздільник: {match.Groups["separator"]}");
                }

                Console.WriteLine($"   Позиція в тексті: {match.Position}");

                if (! match.IsValid)
                {
                    Console.WriteLine($"   Причина: {match.ValidationMessage}");
                    invalidCount++;
                }
                else
                {
                    validCount++;
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Підсумок: Валідних - {validCount}, Невалідних - {invalidCount}");
        }

        // Валідація дати
        private static void ValidateDate(MatchInfo matchInfo)
        {
            if (! matchInfo.Groups.ContainsKey("day") ||
                !matchInfo.Groups. ContainsKey("month") ||
                !matchInfo.Groups. ContainsKey("year"))
            {
                matchInfo.IsValid = false;
                matchInfo.ValidationMessage = "Відсутні необхідні компоненти дати";
                return;
            }

            string day = matchInfo.Groups["day"];
            string month = matchInfo.Groups["month"];
            string year = matchInfo.Groups["year"];
            string separator = matchInfo.Groups. ContainsKey("separator") ?  matchInfo.Groups["separator"] : "/";

            // Спроба розпарсити дату
            string[] formats = { "dd/MM/yyyy", "dd-MM-yyyy" };
            string dateString = $"{day}{separator}{month}{separator}{year}";

            if (DateTime.TryParseExact(
                dateString,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles. None,
                out DateTime parsedDate))
            {
                matchInfo.IsValid = true;
                matchInfo.ValidationMessage = $"Дата коректна: {parsedDate:D}";
            }
            else
            {
                matchInfo. IsValid = false;
                matchInfo.ValidationMessage = "Дата не існує (наприклад, 31/04 або 29/02 у невисокосний рік)";
            }
        }

        // Демонстрація різних методів класу Regex
        private static void DemonstrateRegexMethods(string text)
        {
            Console.WriteLine("ДЕМОНСТРАЦІЯ МЕТОДІВ КЛАСУ REGEX:\n");

            string pattern = DateRegex.ToString();

            // 1. IsMatch - перевірка наявності збігу
            bool hasMatch = DateRegex.IsMatch(text);
            Console.WriteLine($"1. IsMatch: У тексті {(hasMatch ? "є" : "немає")} дат\n");

            // 2.  Match - пошук першого збігу
            Match firstMatch = DateRegex.Match(text);
            if (firstMatch.Success)
            {
                Console.WriteLine($"2. Match (перший збіг): {firstMatch.Value}");
                Console.WriteLine($"   Іменовані групи:");
                Console.WriteLine($"   - day: {firstMatch.Groups["day"]. Value}");
                Console.WriteLine($"   - month: {firstMatch.Groups["month"].Value}");
                Console.WriteLine($"   - year: {firstMatch. Groups["year"].Value}\n");
            }

            // 3. Replace - заміна дат на інший формат
            string replaced = DateRegex.Replace(text, m =>
            {
                return $"[ДАТА: {m.Groups["day"].Value}. {m.Groups["month"].Value}.{m.Groups["year"]. Value}]";
            });
            Console.WriteLine("3. Replace (заміна дат на форматований вигляд):");
            Console. WriteLine(replaced. Trim());
            Console.WriteLine();

            // 4. Split - розділення тексту по датам
            string[] parts = DateRegex.Split(text);
            int nonEmptyParts = parts.Count(p => !string.IsNullOrWhiteSpace(p));
            Console.WriteLine($"4. Split: Текст розділено на {parts.Length} частин ({nonEmptyParts} непорожніх)\n");

            // 5.  Matches з Where - фільтрація результатів
            var februaryDates = DateRegex. Matches(text)
                .Cast<Match>()
                .Where(m => m.Groups["month"].Value == "02")
                .ToList();
            Console.WriteLine($"5. Matches + LINQ: Знайдено {februaryDates.Count} дат у лютому");
            foreach (var match in februaryDates)
            {
                Console.WriteLine($"   - {match.Value}");
            }
        }
    }
}
