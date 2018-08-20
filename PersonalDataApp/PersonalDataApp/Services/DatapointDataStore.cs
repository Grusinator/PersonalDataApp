using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PersonalDataApp.Models;

[assembly: Xamarin.Forms.Dependency(typeof(PersonalDataApp.Services.MockDataStore))]
namespace PersonalDataApp.Services
{
    public class DatapointDataStore : IDataStore<Datapoint>
    {
        List<Datapoint> datapoints;
        GraphqlHandler gqlHandler;

        public DatapointDataStore(GraphqlHandler gqlHandler = null)
        {
            datapoints = new List<Datapoint>();

            this.gqlHandler = gqlHandler ?? new GraphqlHandler();

        }

        public async Task<bool> AddItemAsync(Datapoint item)
        {
            datapoints.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Datapoint item)
        {
            var _item = datapoints.Where((Datapoint arg) => arg.Id == item.Id).FirstOrDefault();
            datapoints.Remove(_item);
            datapoints.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var _item = datapoints.Where((Datapoint arg) => arg.Id == Convert.ToInt32(id)).FirstOrDefault();
            datapoints.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Datapoint> GetItemAsync(string id)
        {
            return await Task.FromResult(datapoints.FirstOrDefault(s => s.Id == Convert.ToInt32(id)));
        }

        public async Task<IEnumerable<Datapoint>> GetItemsAsync(bool forceRefresh = false)
        {
            datapoints = await gqlHandler.GetAllDatapoints();
            datapoints.Reverse();
            return await Task.FromResult(datapoints);
        }
    }
}