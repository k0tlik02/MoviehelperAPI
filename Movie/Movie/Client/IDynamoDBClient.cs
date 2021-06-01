using Movie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movie.Client
{
    public interface IDynamoDBClient
    {
        public Task<filmsDBRepository> GetDatabyId(string Id);
        public Task<bool> PostDataToDB(filmsDBRepository data);
        public Task<List<filmsDBRepository>> GetAll_1();
        public Task<List<filmsDBRepository>> GetAll(string userId);
        public Task<bool> Deelete(string Id);
    }
}
