using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using movies;


namespace movies
{
    public class Program
    {
        static void Main(string[] args)
        {

            Stopwatch stopwatch = new Stopwatch();

            /////////////
            string getWord(string line, int wordIndex, char separator, out int wordIndexLocation, int startIndex = 0)
            {
                wordIndexLocation = 0;
                int currentWordIndex = 0;

                while (startIndex < line.Length)
                {
                    if (line[startIndex] != separator)
                    {
                        int wordStart = startIndex;

                        while (startIndex < line.Length && line[startIndex] != separator)
                        {
                            startIndex++;
                        }

                        currentWordIndex++;
                        if (currentWordIndex == wordIndex)
                        {
                            wordIndexLocation = startIndex + 1;
                            return line.Substring(wordStart, startIndex - wordStart);
                        }
                    }
                    else
                    {
                        currentWordIndex++;
                    }

                    startIndex++;
                }

                return $"Меньше {wordIndex} слов в строке";
            }
            Console.WriteLine("Поехали!");
            stopwatch.Start();
            Dictionary<string, Movie> code_movies = new Dictionary<string, Movie>();
            Dictionary<string, string> tittle_to_moviescode = new Dictionary<string, string>();

            string filePath1 = "C:\\Users\\Пользователь\\Прога_фильмы\\MovieCodes_IMDB.tsv";
            // Открываем файл для чтения
            using (StreamReader reader = new StreamReader(filePath1))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string region = getWord(line, 4, '\t', out int ind4);
                    string language = getWord(line, 1, '\t', out int ind5, ind4);

                    if ((region == "RU" || region == "US" || region == "GB") || (language == "ru" || language == "en"))
                    {
                        string movieCode1 = getWord(line, 1, '\t', out int ind1);
                        string movieTitle = getWord(line, 2, '\t', out int ind3, ind1);
                        if (!tittle_to_moviescode.ContainsKey(movieTitle))
                        { tittle_to_moviescode.Add(movieTitle, movieCode1); }


                        if (code_movies.TryGetValue(movieCode1, out Movie movie))
                        {
                            code_movies[movieCode1].movieTitles.Add(movieTitle);

                        }
                        else
                        {
                            code_movies.Add(movieCode1, new Movie(movieTitle, movieCode1));
                        }


                    }
                }

                /*
                foreach (var code in tittle_to_moviescode)
                {
                    Console.WriteLine(code.Key + " " +  code.Value);
                }
                */
                /*
                foreach (var code in code_movies)
                {
                    Console.WriteLine(code.Key + " " + code.Value.movieTitles.First());
                }
                */
            }
            Console.WriteLine("считали первый файл" + stopwatch.Elapsed);

            ///////////////////////////////////Рейтинг по коду фильма

            string filePath2 = "C:\\Users\\Пользователь\\Прога_фильмы\\Ratings_IMDB.tsv";
            using (StreamReader reader = new StreamReader(filePath2))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string movieCode2 = getWord(line, 1, '\t', out int ind1);
                    string movieRating = getWord(line, 1, '\t', out int ind2, ind1);

                    if (code_movies.TryGetValue(movieCode2, out Movie movie))
                    {
                        code_movies[movieCode2].movieRating = movieRating;
                    }
                }
                /*
                foreach (var code in code_movies)
                {
                    Console.WriteLine(code.Key + " " + code.Value.movieRating);
                }
                */
            }
            Console.WriteLine("считали второй файл" + stopwatch.Elapsed);


            ////////////////////////////////Тэги и их айди
            string filePath3 = "C:\\Users\\Пользователь\\Прога_фильмы\\TagCodes_MovieLens.csv";

            Dictionary<string, string> tagCodes = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(filePath3))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string idTag = getWord(line, 1, ',', out int ind1);
                    string tagName = getWord(line, 1, ',', out int ind2, ind1);


                    tagCodes[idTag] = tagName;
                }
            }
            Console.WriteLine("считали третий файл" + stopwatch.Elapsed);

            ////////////////////////////////////////////////
            string filePath4 = "C:\\Users\\Пользователь\\Прога_фильмы\\links_IMDB_MovieLens.csv";

            Dictionary<string, string> idAndCode = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(filePath4))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string movieId = getWord(line, 1, ',', out int ind1);
                    string imdbId = "tt" + getWord(line, 1, ',', out int ind2, ind1);


                    idAndCode.Add(movieId, imdbId);
                }
                /*
                    foreach (var code in idAndCode)
                    {
                        Console.WriteLine(code.Key + " " + code.Value);
                    }
                */
            }
            Console.WriteLine("считали четвёртый файл" + stopwatch.Elapsed);


            /////////////////////////айди фильма по айди тэга
            string filePath5 = "C:\\Users\\Пользователь\\Прога_фильмы\\TagScores_MovieLens.csv";
            Dictionary<string, HashSet<string>> tagToMovieId = new Dictionary<string, HashSet<string>>();

            using (StreamReader reader = new StreamReader(filePath5))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string relevance = getWord(line, 3, ',', out int ind3);


                    if (Convert.ToInt32(relevance[2]) >= Convert.ToInt32('5'))
                    {
                        string movieID = getWord(line, 1, ',', out int ind1);
                        string idTag = getWord(line, 1, ',', out int ind2, ind1);


                        if (idAndCode.TryGetValue(movieID, out string codeMovie))
                        {
                            if (code_movies.TryGetValue(codeMovie, out Movie movie))
                            {
                                code_movies[codeMovie].tag.Add(tagCodes[idTag]);
                            }

                            if (tagToMovieId.TryGetValue(tagCodes[idTag], out HashSet<string> codes))
                            {
                                codes.Add(codeMovie);
                            }

                            else
                            {
                                tagToMovieId.Add(tagCodes[idTag], new HashSet<string>() { codeMovie });
                            }
                        }

                    }
                }
                /*
                //по тэгу коды фильмов
                foreach (var tag in tagToMovieId)
                    { 
                        Console.WriteLine(tag.Key + " " + tag.Value.ToArray<string>().First());
                    }
                /*
                 //По коду фильма его тэг
                foreach (var movie in code_movies)
                {
                    Console.WriteLine(movie.Key + " " + movie.Value.tag.Count);
                }
                */
            }
            Console.WriteLine("считали пятый файл" + stopwatch.Elapsed);

            //////////////////////////////////////////
            string filePath6 = "C:\\Users\\Пользователь\\Прога_фильмы\\ActorsDirectorsNames_IMDB.txt";

            Dictionary<string, Person> nconstNamePerson = new Dictionary<string, Person>();

            //Dictionary<string, List<string>> movieActors = new Dictionary<string, List<string>>();

            using (StreamReader reader = new StreamReader(filePath6))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string nconst = getWord(line, 1, '\t', out int ind1);
                    string primaryName = getWord(line, 1, '\t', out int ind2, ind1);

                    nconstNamePerson[nconst] = new Person(primaryName);
                    //nconstNameActor[nconst] = primaryName;
                }
                /*
                foreach (var code in nconstNamePerson)
                {
                    Console.WriteLine(code.Key + " " + code.Value);
                }
                */
                //////////////////////////////////////////////////
                ///
                //сверху всё ок
                Console.WriteLine("считали седьмой файл" + stopwatch.Elapsed);

            }

            string filePath7 = "C:\\Users\\Пользователь\\Прога_фильмы\\ActorsDirectorsCodes_IMDB.tsv";

            //Dictionary<string, string> actorDirectorCodes = new Dictionary<string, string>();
            //Dictionary<string, string> ncontCategory = new Dictionary<string, string>();


            using (StreamReader reader = new StreamReader(filePath7))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string category = getWord(line, 4, '\t', out int ind4);
                    if (category == "director" || category == "actor")
                    {
                        string tconst = getWord(line, 1, '\t', out int ind1);
                        string nconst = getWord(line, 2, '\t', out int ind2, ind1);

                        //actorDirectorCodes[tconst] = nconst;
                        //ncontCategory[nconst] = category;

                        if (category == "actor")
                        {
                            if (code_movies.TryGetValue(tconst, out Movie movie) && nconstNamePerson.TryGetValue(nconst, out Person actor))
                            {
                                movie.actors.Add(nconst);
                                actor.AddMovie(tconst);
                            }
                        }

                        if (category == "director")
                        {
                            if (code_movies.TryGetValue(tconst, out Movie movie) && nconstNamePerson.TryGetValue(nconst, out Person director))
                            {
                                if (movie.director == null)
                                {
                                    movie.director = new List<string>();

                                }
                                movie.director.Add(nconst);
                                director.AddMovie(tconst);
                            }
                            else
                            { continue; }
                        }
                    }
                }
                /*
                //tconst и соответствующий nconst
                foreach (var code in actorDirectorCodes)
                {
                    Console.WriteLine(code.Key + " " + code.Value);
                }
                */

                /*
                //nconst и category
                foreach (var code in ncontCategory)
                {
                    Console.WriteLine(code.Key + " " + code.Value);
                }
                */

            }
            Console.WriteLine("считали шестой файл" + stopwatch.Elapsed);


            Console.Write("Если вы хотите получить информацию о фильме по его названию, нажмите 1; \t " +
                "Если хотите получить список фильмов, в которых снимался актёр, нажмите 2; \t" +
                "Если хотите получить список фильмов по тэгу, нажмите 3 \t");

            string userInput = Console.ReadLine();
            if (userInput == "1")
            {
                Console.WriteLine("Введите название фильма");
                string userInputFilm = Console.ReadLine();
                if (tittle_to_moviescode.TryGetValue(userInputFilm, out string movieCode))
                {
                    if (code_movies.TryGetValue(movieCode, out Movie movie))
                    {
                        Console.WriteLine($"Название фильма: {movie.movieTitles.First()}");
                        Console.WriteLine($"Тэг:");
                        foreach (var tag in movie.tag)
                        {
                            Console.WriteLine($" {tag}");
                        }
                        Console.WriteLine($"Рейтинг: {movie.movieRating}");
                        Console.WriteLine("Список актеров:");
                        foreach (var actor_code in movie.actors)
                        {
                            Console.WriteLine($"  {nconstNamePerson[actor_code].Name}");
                        }
                        Console.WriteLine("Режиссер:");
                        foreach (var director_code in movie.director)
                        {
                            Console.WriteLine($"  {nconstNamePerson[director_code].Name}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Фильм с названием {userInputFilm} не найден.");
                }
            }


            if (userInput == "2")
            {
                Console.WriteLine("Введите имя актёра");
                string userInputActor = Console.ReadLine();

                // Находим экземпляр класса Person по имени актёра
                var actorPerson = nconstNamePerson.Values.FirstOrDefault(person => person.Name == userInputActor);

                if (actorPerson != null)
                {
                    Console.WriteLine($"Фильмы, в которых снимался актёр {userInputActor}:");
                    foreach (var movieCode in actorPerson.CodesOfMovies)
                    {
                        if (code_movies.TryGetValue(movieCode, out Movie movie))
                        {
                            Console.WriteLine($"- {movie.movieTitles.First()}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Актёр с именем {userInputActor} не найден.");
                }
            }

            if (userInput == "3")
            {
                Console.WriteLine("Введите тег:");
                string userInputTag = Console.ReadLine();

                if (tagToMovieId.TryGetValue(userInputTag, out HashSet<string> movieCodes))
                {
                    Console.WriteLine($"Фильмы с тегом {userInputTag}:");
                    foreach (var movieCode in movieCodes)
                    {
                        if (code_movies.TryGetValue(movieCode, out Movie movie))
                        {
                            Console.WriteLine($"- {movie.movieTitles.First()}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Фильмы с тегом {userInputTag} не найдены.");
                }
            }











        }
    }
}