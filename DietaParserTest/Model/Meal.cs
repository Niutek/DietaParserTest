using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DietaParserTest
{
    class Meal
    {
        public Meal()
        {
            this.Ingridients = new Collection<Ingridient>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Ingridient> Ingridients { get; set; }

        public DateTime Day { get; set; }

        public int Hour { get; set; }
    }
}
