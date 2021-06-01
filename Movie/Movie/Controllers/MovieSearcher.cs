using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Movie.Models;
using Movie.Clients;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Movie.Extensions;
using Movie.Client;
using System.Net.Http;

namespace Movie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieSearcher : ControllerBase
    {
        private readonly ILogger<MovieSearcher> _logger;

        private readonly RandomFilmClient _randomFilmClient;

        private readonly IDynamoDBClient _dynamoDBClient;

        public MovieSearcher(ILogger<MovieSearcher> logger, RandomFilmClient randomFilmClient, IDynamoDBClient dynamoDBClient)
        {
            _logger = logger;
            _randomFilmClient = randomFilmClient;
            _dynamoDBClient = dynamoDBClient;
        }

        [HttpGet("random_film")]
        public async Task<RandomFilm> GetRandomFilm()
        {
            var rand = new Random();
            int id = rand.Next(1, 30000);
            var Film = await _randomFilmClient.GetRandomFilm(id);

            return Film;
        }

        [HttpGet("bygenre_from_IMDb")]
        public async Task<List<string>> Getdygenre([FromQuery] string genre)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://imdb8.p.rapidapi.com/title/get-popular-movies-by-genre?genre=%2Fchart%2Fpopular%2Fgenre%2F{genre}"),
                Headers =
    {
        { "x-rapidapi-key", "eadc0d2707mshfdb2c05756cc0dfp1de75ajsn3be9cada93b2" },
        { "x-rapidapi-host", "imdb8.p.rapidapi.com" },
    },
            };

            var response = await client.SendAsync(request);
            
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<string>>(content);
            return result;
        }

        [HttpGet("search_by_tittle_in_IMDb")]
        public async Task<IMDb> GetFilmByName([FromQuery] string tittle)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://movie-database-imdb-alternative.p.rapidapi.com/?i={tittle}&r=json"),
                Headers =
    {
        { "x-rapidapi-key", "eadc0d2707mshfdb2c05756cc0dfp1de75ajsn3be9cada93b2" },
        { "x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com" },
    },
            };
            var response = await client.SendAsync(request) ;
            
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<IMDb>(content);
            return result;
        }

        [HttpGet("popular_list")]
        public async Task<ListOfMovies> GetListPopularMovies([FromQuery] int count_of_films, double minimum_rating)
        {
            var Film = await _randomFilmClient.GetListPopularMovies(count_of_films, minimum_rating);

            return Film;
        }

        [HttpGet("choice_list_of_movies")]
        public async Task<ListOfMovies> GetListOfMovies([FromQuery] int count_of_films, double minimum_rating, string genre)
        {
            var Film = await _randomFilmClient.GetListMovies(count_of_films, minimum_rating, genre);

            return Film;
        }

        [HttpGet("filmbyid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFavouriteFilmbyId([FromQuery] string Id)
        {
            var result = await _dynamoDBClient.GetDatabyId(Id);

            if (result == null)
                return NotFound("this record doesn`t exists in database");
            

            var filmResponse = new Film
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                genre = result.genre,
                description_full = result.description_full,
                large_cover_image = result.large_cover_image,
                year = result.year,
                url = result.url,
                runtime = result.runtime
            };

            return Ok(filmResponse);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorites([FromBody] Film film)
        {
            var data = new filmsDBRepository
            {
                Id = film.Id,
                UserId = film.UserId,
                Name = film.Name,
                url = film.url,
                genre= film.genre,
                description_full = film.description_full,
                large_cover_image = film.large_cover_image,
                year = film.year,
                runtime = film.runtime
            };

            var result = await _dynamoDBClient.PostDataToDB(data);

            if (result== false)
            {
                return BadRequest("Cannot insert value to database. Please look console line!!!");
            }

            return Ok("Value has been successfully added to datebase :)");
        }

        [HttpGet("all_favorites")]
        public async Task<IActionResult> GetAll(string userId)
        {
            var response = await _dynamoDBClient.GetAll(userId);

            if (response == null)
                return NotFound("There are no records in db");

            var result = response.Select(x => new Film()
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                url = x.url,
                genre = x.genre,
                description_full = x.description_full,
                large_cover_image = x.large_cover_image,
                year = x.year,
                runtime = x.runtime
            }).ToList();
            return Ok(result);
        }
        [HttpGet("all_favorites_all")]
        public async Task<IActionResult> GetAll_1()
        {
            var response = await _dynamoDBClient.GetAll_1();
            if (response == null)
                return NotFound("There are no records in db");

            var result = response.Select(x => new Film()
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                url = x.url,
                genre = x.genre,
                description_full = x.description_full,
                large_cover_image = x.large_cover_image,
                year = x.year,
                runtime = x.runtime
            }).ToList();
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFromFavorites([FromQuery] string Id)
        {
            var result = await _dynamoDBClient.Deelete(Id);
            
            if (result == false)
            {
                return BadRequest("We cannot delete this film. Please look console line!!!");
            }
            return Ok("Film has been successfully removed from datebase :)");
        }
  }
}
