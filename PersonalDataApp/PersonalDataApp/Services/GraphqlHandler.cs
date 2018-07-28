using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDataApp
{
    public class GraphqlHandler
    {
        GraphQLClient graphQLClient { get; set; }

        public string token {get; set;}

        public GraphqlHandler()
        {
            graphQLClient = new GraphQLClient("http://192.168.100.102:8000/graphql/");
        }

        public async Task<String> Login(string username, string password)
        {
            var uploadAudioRequest = new GraphQLRequest
            {
                Query = @"
                mutation LoginMutation($username: String!, $password: String!) {
                    tokenAuth(username: $username, password: $password) {
                        token
                    }
                }",
                OperationName = "LoginMutation",
                Variables = new
                {
                    username = username,
                    password = password
                }
            };

            var graphQLResponse = await graphQLClient.PostAsync(uploadAudioRequest);

            token = graphQLResponse.Data.tokenAuth.token.Value;

            if (token != null)
            {
                graphQLClient.DefaultRequestHeaders.Add("Authorization", "JWT " + token);
            }

            

            return token;
        }

        public async Task UploadAudio()
        {
            var uploadAudioRequest = new GraphQLRequest
            {
                Query = @"
                    mutation createDatapointMutation(
	                    $datetime: DateTime, 
	                    $category:CategoryTypes,
	                    $source_device: String!,
	                    $value: Float,
	                    $text_from_audio: String,
	                    $files: Upload
                    ) {
                    createDatapoint(
		                datetime:$datetime, 
		                category: $category,
		                sourceDevice:$source_device,
		                value:$value,
		                textFromAudio:$text_from_audio,
		                files:$files
	                ){
                        id
		                category
		                owner
		                {
			                username
		                }
                    }
                }",
                OperationName = "createDatapointMutation",
                Variables = new
                {
                    category = "food_picture",
                    source_device = "Android",
                    value = 3,
                    text_from_audio = "null"
                }
            };

            var graphQLResponse = await graphQLClient.PostAsync(uploadAudioRequest);

            var succeeded = graphQLResponse.Data.createDatapoint.category.Value;

        }


    }

}

    /*
    public class GraphResult<T>
    {
        public T Data { get; set; }
    }

    public class GraphClient
    {
        private readonly HttpClient _client;

        public GraphClient()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer your-bearer-token-goes-here");
            _client.DefaultRequestHeaders.Add("User-Agent", "Xamarin-GraphQL-Demo");
        }

        public async Task<T> Query<T>(string query)
        {
            var graphQuery = new { query };
            var content = new StringContent(JsonConvert.SerializeObject(graphQuery), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://api.github.com/graphql", content);
            var json = await response.Content.ReadAsStringAsync();

            var graphResult = JsonConvert.DeserializeObject<GraphResult<T>>(json);

            return graphResult.Data;
        }
    }

    public class Repository
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class RepositoryEdge
    {
        public Repository Node { get; set; }
    }

    public class RepositoryConnection
    {
        public IList<RepositoryEdge> Edges { get; set; }
    }

    public class RepositoryQueryResult
    {
        public RepositoryConnection Xamarin { get; set; }
    }

}
*/