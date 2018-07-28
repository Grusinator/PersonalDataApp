using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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


    // Implements multipart/form-data POST in C# http://www.ietf.org/rfc/rfc2388.txt
    // http://www.briangrinstead.com/blog/multipart-form-post-in-c
    public static class multipartUpload
    {

        private static readonly Encoding encoding = Encoding.UTF8;
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }
        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
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