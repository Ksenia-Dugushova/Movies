using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movies
{
    internal class Actor
    {
        public string Name { get; set; }
        //public string Code { get; set; }
        public HashSet<string> CodesOfMovies { get; set; }
        public Actor(string name)
        {
            Name = name;
            CodesOfMovies = new HashSet<string>();
        }
        public void AddMovie(string movie) { CodesOfMovies.Add(movie); }
        public void Print(Dictionary<string, Movie> MovieByCode)
        {
            Console.WriteLine(Name);
            Console.Write("Участвовал в фильмах: ");
            int q = 0;
            foreach (var code in CodesOfMovies)
            {
                if (q > 0) Console.Write(" | ");
                Console.Write(MovieByCode[code].movieTitles.First());
                q++;
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
