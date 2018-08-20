using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PersonalDataApp.Models;

[assembly: Xamarin.Forms.Dependency(typeof(PersonalDataApp.Services.MockDataStore))]
namespace PersonalDataApp.Services
{
    public class DatapointMockDataStore : IDataStore<Datapoint>
    {
        List<Datapoint> datapoints;

        public DatapointMockDataStore()
        {
            datapoints = new List<Datapoint>();
            var mockItems = new List<Datapoint>
            {
                new Datapoint { Category = "test", TextFromAudio = "blah bla" },
                new Datapoint { Category = "test", TextFromAudio = "blah bla2"  }
            };

            foreach (var item in mockItems)
            {
                datapoints.Add(item);
            }
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
            return await Task.FromResult(datapoints);
        }
    }
}