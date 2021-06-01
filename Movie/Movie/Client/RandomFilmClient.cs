using Movie.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Movie.Clients
{
    public class RandomFilmClient
    {
        private HttpClient _client;
        private static string _adress;
        public RandomFilmClient()
        {
            _adress = Constants.adress;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_adress);
        }

        public async Task<RandomFilm> GetRandomFilm(int id)
        {
            var response = await _client.GetAsync($"/api/v2/movie_details.json?movie_id={id}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<RandomFilm>(content);

            return result;
        }

        public async Task<ListOfMovies> GetListMovies(int count_of_films, double minimum_rating, string genre)
        {
            var response = await _client.GetAsync($"/api/v2/list_movies.json?limit={count_of_films}&minimum_rating={minimum_rating}&genre={genre}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<ListOfMovies>(content);

            return result;
        }
        public async Task<ListOfMovies> GetListPopularMovies(int count_of_films, double minimum_rating)
        {
            var response = await _client.GetAsync($"/api/v2/list_movies.json?limit={count_of_films}&minimum_rating={minimum_rating}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<ListOfMovies>(content);

            return result;
        }
    }
}
