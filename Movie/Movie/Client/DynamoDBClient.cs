using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Movie.Extensions;
using Movie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movie.Client
{
    public class DynamoDBClient : IDynamoDBClient, IDisposable
    {
        public string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;
        public DynamoDBClient(IAmazonDynamoDB dynamoDB)
        {
            _dynamoDb = dynamoDB;
            _tableName = Constants.TableName;
        }
        public async Task<bool> Deelete(string Id)
        {
            var Item = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue{ S = Id} }
                }
            };
            try 
            {
                await _dynamoDb.DeleteItemAsync(Item);
                return true; 
            }
            catch (Exception e)
            {
                Console.WriteLine("Oh sheat, you have problem\n" + e);
                return false;
            }
        }

        public async Task<filmsDBRepository> GetDatabyId(string Id)
        {            
                var Item = new GetItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        {"Id", new AttributeValue{ S = Id } }
                    }
                };

                var response = await _dynamoDb.GetItemAsync(Item);
                if (response.Item == null || !response.IsItemSet)
                    return null;

                var result = response.Item.ToClass<filmsDBRepository>();

                return result;
        }

        public async Task<bool> PostDataToDB(filmsDBRepository data)
        {
            var request = new PutItemRequest()
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue { S=data.Id } },
                    {"UserId", new AttributeValue { S=data.UserId} },
                    {"Name", new AttributeValue { S=data.Name } },
                    {"description_full", new AttributeValue { S=data.description_full} },
                    {"url", new AttributeValue { S=data.url } },
                    { "genre", new AttributeValue{ S=data.genre} },
                    {"large_cover_image", new AttributeValue { S=data.large_cover_image } },
                    {"year", new AttributeValue { S=data.year } },
                    {"runtime", new AttributeValue { S=data.runtime } }
                }
            };

            try 
            {
                var response = await _dynamoDb.PutItemAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Console.WriteLine("Oh sheat, you have problem\n" + e);
                return false;
            }
        }

        public async Task<List<filmsDBRepository>> GetAll(string userId)
        {
            var result = new List<filmsDBRepository>();

            var request = new ScanRequest
            {
                TableName = _tableName
            };

            var response = await _dynamoDb.ScanAsync(request);
            if (response.Items == null || response.Items.Count == 0)
                return null;
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                //result.Add(item.ToClass<filmsDBRepository>());
                  var k = item.ToClass<filmsDBRepository>();
                  if (k.UserId == userId) result.Add(k);
            }
            return result;
        }

        public void Dispose()
        {
            _dynamoDb.Dispose();
        }

        public async Task<List<filmsDBRepository>> GetAll_1()
        {
            var result = new List<filmsDBRepository>();

            var request = new ScanRequest
            {
                TableName = _tableName
            };

            var response = await _dynamoDb.ScanAsync(request);
            if (response.Items == null || response.Items.Count == 0)
                return null;
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                result.Add(item.ToClass<filmsDBRepository>());
            }
            return result;
        }
    }
}
