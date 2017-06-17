using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DietaParserTest
{
    class DayParser
    {
        public int DayNumber { get; set; }

        public DateTime Day { get; set; }

        public Meal Breakfast { get; set; }

        public Meal Lunch { get; set; }

        public Meal Dinner { get; set; }

        public Meal Tea { get; set; }

        public Meal Supper { get; set; }

        public Meal Current { get; set; }
        
        private int ParseStage = -1;

        private Dictionary<int, Func<Meal>> ParseStages;// = new Dictionary<int, Func<Meal>>
        
        public DayParser()
        {
            Breakfast = new Meal();
            Lunch = new Meal();
            Dinner = new Meal();
            Tea = new Meal();
            Supper = new Meal();

            ParseStages = new Dictionary<int, Func<Meal>>
            {
                { 0, () => Breakfast },
                { 1, () => Lunch },
                { 2, () => Dinner },
                { 3, () => Tea },
                { 4, () => Supper },
            };
        }

        public bool TryParseDayLine(string dayLine)
        {
            var numbersMatch = Regex.Match(dayLine, @"Dzień ([\d])+: ([\d]{2})\.([\d]{2}), .*");

            if (!numbersMatch.Success)
            {
                return false;
            }

            DayNumber = numbersMatch.ToInt(1);
            Day = new DateTime(2017, int.Parse(numbersMatch.Groups[3].Value), int.Parse(numbersMatch.Groups[2].Value));

            Breakfast.Day = Day;
            Lunch.Day = Day;
            Dinner.Day = Day;
            Tea.Day = Day;
            Supper.Day = Day;

            return true;
        }

        public bool TryParseMealHour(string line)
        {            
            if (!Regex.IsMatch(line, @"\d\d:\d\d.*"))
            {
                return false;
            }

            ParseStage++;
            Current = ParseStages[ParseStage]();

            Current.Hour = int.Parse(line.Substring(0, 2));
            return true;
        }

        public void ParseIngridient(string line)
        {
            var match = Regex.Match(line, @"(.*) (\d+)g (.*)Zestaw (\d+) ≈ (\d+) kcal");

            if (!match.Success)
            {
                throw new Exception($"Failed to match ingridient from {line}");
            }

            var ingridient = new Ingridient
            {
                Name = match.Groups[1].Value,
                Weight = match.ToInt(2),
                Size = match.Groups[3].Value.TrimEnd(' '),
                IngridientSet = match.ToInt(4),
                Calories = match.ToInt(5)
            };

            if (ingridient.Name.Length >99 || ingridient.Size.Length>99)
            {
                Console.WriteLine(ingridient.Name);
                Console.WriteLine(ingridient.Size);
            }

            Current.Ingridients.Add(ingridient);

            //var ingridient = line.Substring(0, match.Index - 1);
            var weight = match.Value;

            //Console.WriteLine($"Składnik: {ingridient}, waga {weight}");
        }

        public bool TryParseDescription(string line)
        {
            if (Regex.IsMatch(line, @"\d\d:\d\d.*"))
            {
                return false;
            }

            if (IsKnownPart(line)) // dinner
            {
                return false;
            }

            Current.Description += line;
            return true;
        }

        private bool IsKnownPart(string line)
        {
            if (line.StartsWith("Dzień"))
            {
                return true;
            }

            if (line.StartsWith("Analizę i badania dla Piotr Auguścik"))
            {
                return true;
            }

            return false;
        }
    }
}
