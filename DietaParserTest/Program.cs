using DietaParserTest.Model;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace DietaParserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var page = ExtractTextFromPdf(@"E:\dieta.pdf");

            var days = new List<DayParser>();

            var lines = page.Split('\n');

            for (var i = 0; i < lines.Length;)
            {
                var line = lines[i];
                if (line.StartsWith("Piotr Auguścik"))
                {
                    i++;
                    continue;
                }

                if (!line.StartsWith("Dzień "))
                {
                    i++;
                    continue;
                }

                //var numbersMatch = Regex.Match(line, @"Dzień 1: 15.05, .*");

                var dayParser = new DayParser();
                days.Add(dayParser);
                
                if (!dayParser.TryParseDayLine(line))
                {
                    i++;
                    continue;
                }

                Console.WriteLine($"Dzień {dayParser.DayNumber}: {dayParser.Day}");

                line = lines[++i];

                for (int parseStage = 0; parseStage < 5; parseStage++)
                {
                    while (!dayParser.TryParseMealHour(line))
                    {
                        line = lines[++i];
                    }

                    Console.WriteLine($"Godzina: {dayParser.Current.Hour}");

                    dayParser.Current.Name = line = lines[++i];

                    Console.WriteLine($"Danie: {dayParser.Current.Name}");

                    if (dayParser.Current.Name.Length > 99)
                    {
                        Console.WriteLine($"Danie: {dayParser.Current.Name}");
                    }

                    line = lines[++i];
                    while (!line.StartsWith("Sposób przygotowania:"))
                    {
                        dayParser.ParseIngridient(line);
                        line = lines[++i];
                    }

                    while (dayParser.TryParseDescription(line))
                    {
                        line = lines[++i];
                    }

                    if (dayParser.Current.Description.Length > 1024)
                    {
                        Console.WriteLine("Error");
                    }
                }

                if (days.Count == 28)
                {
                    break;
                }
            }

            SavePlan(days);

            Console.ReadLine();
        }

        private static void SavePlan(List<DayParser> days)
        {
            var db = new DietDbContext();

            foreach (var day in days)
            {
                db.Meals.Add(day.Breakfast);
                db.Meals.Add(day.Lunch);
                db.Meals.Add(day.Dinner);
                db.Meals.Add(day.Tea);
                db.Meals.Add(day.Supper);
            }

            db.SaveChanges();
        }

        private static void CheckDb()
        {
            var db = new DietDbContext();

            var meal = new Meal
            {
                Day = DateTime.Today,
                Description = "desc",
                Hour = 4,
                Name = "test",
                Ingridients = new List<Ingridient>
                {
                    new Ingridient
                    {
                        Calories = 500,
                        Name ="nnn",
                        IngridientSet = 23,
                        Size = "WWW",
                        Weight = 100
                    }
                }
            };

            db.Meals.Add(meal);
            db.SaveChanges();
        }

        public static string ExtractTextFromPdf(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 6; i <= 53/*reader.NumberOfPages*/; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }
    }
}
