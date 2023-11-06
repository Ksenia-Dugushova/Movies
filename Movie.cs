using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movies
{
	internal class Movie
	{
		public List<string> movieTitles { get; set; }

		public List<string> actors { get; set; }
		//public HashSet<string> actors = new HashSet<string>();
		public List<string> director { get; set; }
		//public string director = "";
		public HashSet<string> tag = new HashSet<string>();
		public string movieRating = "";

		public Movie(string title, string code)
		{
			movieTitles = new List<string> { title };
			MovieCode = code;
			//
			actors = new List<string>();
		}

		public string MovieCode { get; }

		public string idTag = "";
		/*
        public void DisplayMovieInfo()
        {
            Console.WriteLine($"Название фильма: {movieTitles.FirstOrDefault()}");
            Console.WriteLine("Актеры:");
            foreach (var actor in actors)
            {
                Console.WriteLine($"- {actor}");
            }
        }
        */
	}
}
