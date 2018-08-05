﻿using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using PersonalDataApp.Models;

namespace PersonalDataApp.Services
{
    public class GraphqlHandler
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        GraphQLClient graphQLClient { get; set; }

        public string token {get; set;}

        static string url = "http://personal-data-api.herokuapp.com/graphql/";
        static string userAgent = "XamarinApp";

        public GraphqlHandler()
        {
            graphQLClient = new GraphQLClient(url);
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
            try
            {
                var graphQLResponse = await graphQLClient.PostAsync(uploadAudioRequest);
                token = graphQLResponse.Data.tokenAuth.token.Value;
            }
            catch
            {
                return null;
            }

            if (token != null)
            {
                graphQLClient.DefaultRequestHeaders.Add("Authorization", "JWT " + token);
            }

            return token;
        }

        public async Task UploadAudio()
        {
            GraphQLRequest uploadAudioRequest = new GraphQLRequest
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



        public Datapoint UploadDatapoint(Datapoint obj, string filepath1 = null, string filepath2 = null)
        {
            string variables = serializeVariablesFromObject(obj);

            string Query = @"
                    mutation(
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
                }";

            Query = "mutation testmutation($datetime:DateTime, $category:CategoryTypes, $source_device:String!, $value:Float, $text_from_audio:String, $files:Upload!) {createDatapoint(datetime:$datetime, category:$category, sourceDevice:$source_device, value:$value, textFromAudio:$text_from_audio, files:$files){ id datetime category sourceDevice }}";

            //variables = "\"variables\":{\"datetime\": null,\"category\": \"test\",\"source_device\": \"insomnia\",\"value\": null, \"text_from_audio\": null,\"files\": [null, null]}";

            //Query = @"mutation ($file: Upload!) { upload2Files(file: $file) { success } }";

            //variables = "\"variables\":{\"files\": [null, null] }";

            string jsonString = upload2FilesGeneric(Query, variables, filepath1, filepath2);

            dynamic dDatapoint = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

            var ddp = dDatapoint.data.createDatapoint;

            Datapoint datapoint = new Datapoint()
            {
                datetime = ddp.datetime,
                category = ddp.category,
                source_device = ddp.sourceDevice
            };

            return datapoint;
        }

        private string serializeVariablesFromObject(Datapoint obj, int add_files = 0)
        {

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            json = "\"variables\": " + json;
            return json.Replace("}", ",\"files\": [null, null] }");
        }


        private string upload2FilesGeneric(string query, string variables, string filepath1 = "", string filepath2 = "")
        {
            string filename1 = Path.GetFileName(filepath1);
            string filename2 = Path.GetFileName(filepath2);

            Dictionary<string, Object> postParameters = new Dictionary<string, object>
            {
                {"operations" , "{ \"query\": \"" + query + "\"," + variables + "}"},
                {"map", "{ \"0\": [\"variables.files.0\"], \"1\": [\"variables.files.1\"]}" },
                //{"map", "{\"0\":[\"variables.file\"]}" },
                { "0", filepath1 != null ? new FileParameter(File.ReadAllBytes(filepath1), filename1) : null },
                { "1", filepath2 != null ? new FileParameter(File.ReadAllBytes(filepath2), filename2) : null }
            };

            var response = MultipartFormDataPost(url, userAgent, postParameters);

            return response.ToString();
        }

        public bool upload2Files(string filepath1 = "", string filepath2 = "")
        {
            string filename1 = Path.GetFileName(filepath1);
            string filename2 = Path.GetFileName(filepath2);


            Dictionary<string, Object> postParameters = new Dictionary<string, object>
            {
                {"operations" , "{ \"query\": \"mutation ($files: Upload!) { upload2Files(files: $files) { success } }\",\"variables\":{\"files\": [null, null]} }" },
                {"map", "{ \"0\": [\"variables.files.0\"], \"1\": [\"variables.files.1\"]}" },
                //{"map", "{\"0\":[\"variables.file\"]}" },
                { "0", filepath1 != null ? new FileParameter(File.ReadAllBytes(filepath1), filename1) : null },
                { "1", filepath2 != null ? new FileParameter(File.ReadAllBytes(filepath2), filename2) : null }
            };

            var response = MultipartFormDataPost(url, userAgent, postParameters);
            return true;
        }

        public bool uploadFile(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            Dictionary<string, Object> postParameters = new Dictionary<string, object>
            {
                {"operations" , "{ \"query\": \"mutation ($file: Upload!) { uploadFile(file: $file) { success } }\",\"variables\":{\"file\": null} }" },
                {"map", "{\"0\":[\"variables.file\"]}" },
                { "0", new FileParameter(File.ReadAllBytes(filepath), filename) }
            };

            var response = MultipartFormDataPost(url, userAgent, postParameters);
            return true;
        }


        // Implements multipart/form-data POST in C# http://www.ietf.org/rfc/rfc2388.txt
        // http://www.briangrinstead.com/blog/multipart-form-post-in-c

        
        public string MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }

        private string PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
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
            //request.PreAuthenticate = true;
            //request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            request.Headers.Add("Authorization", "JWT " + token);

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            HttpWebResponse loWebResponse = (HttpWebResponse)request.GetResponse();

            StreamReader loResponseStream = new StreamReader(loWebResponse.GetResponseStream(), encoding);

            string lcHtml = loResponseStream.ReadToEnd();

            return lcHtml;
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