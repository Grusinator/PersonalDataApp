using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using PersonalDataApp.Models;
using System.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace PersonalDataApp.Services
{
    public class GraphqlHandler
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        GraphQLClient graphQLClient { get; set; }

        private string token = string.Empty;
        public string Token
        {
            get { return token; }
            set
            {
                graphQLClient.DefaultRequestHeaders.Add("Authorization", "JWT " + value);
                token = value;
            }
        }

        //static string url = "http://personal-data-api.herokuapp.com/graphql/";
        static string url = "http://192.168.1.112:8000/graphql/";
        static readonly string userAgent = "XamarinApp";

        public GraphqlHandler()
        {
            graphQLClient = new GraphQLClient(url);
        }

        public async Task<String> Signup(string username, string password, string email)
        {
            var uploadAudioRequest = new GraphQLRequest
            {
                Query = @"
                mutation CreateUserMutation($username: String!, $password: String!, $email: String!) {
                    createUser(username: $username, password: $password, email: $email){
		                user{
			                username
			                password
			                email
		                }
	                }
                }",
                OperationName = "CreateUserMutation",
                Variables = new
                {
                    username = username,
                    password = password,
                    email = email
                }
            };
            try
            {
                var graphQLResponse = await graphQLClient.PostAsync(uploadAudioRequest);

                if (graphQLResponse.Errors != null)
                {
                    throw new HttpRequestException(graphQLResponse.Errors[0].Message);
                }
                if (username == graphQLResponse.Data.createUser.user.username.Value)
                {
                    return username;
                }
            }
            catch( HttpRequestException e)
            {
                throw e;
            }
            return null;
        }

        public async Task<String> Login(string username, string password)
        {
            var loginRequest = new GraphQLRequest
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
                var graphQLResponse = await graphQLClient.PostAsync(loginRequest);

                if (graphQLResponse.Errors != null)
                {
                    throw new HttpRequestException(graphQLResponse.Errors[0].Message);
                }

                Token = graphQLResponse.Data.tokenAuth.token.Value;

            }
            catch( HttpRequestException e)
            {
                throw e;
            }

            return Token;
        }

        private User dynamicProfileToUser(dynamic profile, string token = null)
        {
            return new User()
            {
                Username = profile.user.username,
                FirstName = profile.user.firstName,
                LastName = profile.user.lastName,
                Email = profile.user.email,
                Language = profile.language,
                Birthdate = profile.birthdate,
                AudioThreshold = profile.audioThreshold,
                Token = token ?? Token  //keep the token
            };
        }

        public async Task<User> GetUser()
        {
            var getUserRequest = new GraphQLRequest
            {
                Query = @"
                query GetUserQueue{
	                profile
                    {
		                language
		                birthdate
		                audioThreshold
                        user 
                        {
		                    username
		                    email
		                    lastName
		                    firstName
	                    }
	                }
                }",
                OperationName = "GetUserQueue",
                Variables = new
                {
                }
            };
            try
            {
                var graphQLResponse = await graphQLClient.PostAsync(getUserRequest);

                if (graphQLResponse.Errors != null)
                {
                    throw new HttpRequestException(graphQLResponse.Errors[0].Message);
                }

                dynamic profile = graphQLResponse.Data.profile;
                return dynamicProfileToUser(profile);
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        public async Task<User> UpdateProfileAsync(User user)
        {
            var UpdateProfileRequest = new GraphQLRequest
            {
                Query = @"
                mutation UpdateProfileMutation($birthdate: Date, $language: Languages!, $audio_threshold: Float) {
	            updateProfile(
		            birthdate: $birthdate, 
		            language: $language, 
		            audioThreshold: $audio_threshold){
			            profile{
				            language
				            birthdate
				            audioThreshold
				            user
                            {
					            username
                                firstName
                                lastName
                                email
					        }
				        }
		            }
	            }",
                OperationName = "UpdateProfileMutation",
                Variables = new
                {
                    birthdate = user.Birthdate.Date.ToString("yyyy-MM-dd"),
                    language = user.Language,
                    audio_threshold = user.AudioThreshold
                }
            };
            try
            {
                var graphQLResponse = await graphQLClient.PostAsync(UpdateProfileRequest);

                if (graphQLResponse.Errors != null)
                {
                    throw new HttpRequestException(graphQLResponse.Errors[0].Message);
                }
                
                dynamic profile = graphQLResponse.Data.updateProfile.profile;
                return dynamicProfileToUser(profile);
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }





        public async Task<List<Datapoint>> GetAllDatapoints()
        {
            var uploadAudioRequest = new GraphQLRequest
            {
                Query = @"
                query GetAllItemsQueue{
                    allDatapoints{
		                id
		                datetime
		                textFromAudio
		                sourceDevice
                    }
                }",
                OperationName = "GetAllItemsQueue",
                Variables = new
                {
                }
            };
            
            try
            {

                var graphQLResponse = await graphQLClient.PostAsync(uploadAudioRequest);

                List<Datapoint> list = new List<Datapoint>();
                foreach (var obj in graphQLResponse.Data.allDatapoints)
                {
                    list.Add(
                        new Datapoint()
                        {
                            Id = obj.id,
                            Datetime = obj.datetime,
                            TextFromAudio = obj.textFromAudio
                        }
                    );
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        internal void UpdateAuthToken(string token)
        {
            if (token != null && token != Token)
            {
                Token = token;
            }
        }

        //public async Task UploadDatapointWIthoutFIle()
        //{
        //    GraphQLRequest uploadAudioRequest = new GraphQLRequest
        //    {
        //        Query = @"
        //            mutation createDatapointMutation(
        //             $datetime: DateTime, 
        //             $category:CategoryTypes,
        //             $source_device: String!,
        //             $value: Float,
        //             $text_from_audio: String,
        //             $files: Upload
        //            ) {
        //            createDatapoint(
        //          datetime:$datetime, 
        //          category: $category,
        //          sourceDevice:$source_device,
        //          value:$value,
        //          textFromAudio:$text_from_audio,
        //          files:$files
        //         ){
        //                id
        //          category
        //          owner
        //          {
        //           username
        //          }
        //            }
        //        }",
        //        OperationName = "createDatapointMutation",
        //        Variables = new
        //        {
        //            category = "food_picture",
        //            source_device = "Android",
        //            value = 3,
        //            text_from_audio = "null"
        //        }
        //    };

        //    var graphQLResponse = await graphQLClient.PostAsync(uploadAudioRequest);

        //    var succeeded = graphQLResponse.Data.createDatapoint.category.Value;

        //}

        public async Task<Datapoint> UploadDatapointAsync(Datapoint obj, string filepath = null)
        {
            string variables = serializeVariablesFromObject(obj, add_files:1);

            //string Query = @"
            //        mutation(
	           //         $datetime: DateTime, 
	           //         $category:CategoryTypes,
	           //         $source_device: String!,
	           //         $value: Float,
	           //         $text_from_audio: String,
	           //         $files: Upload
            //        ) {
            //        createDatapoint(
		          //      datetime:$datetime, 
		          //      category: $category,
		          //      sourceDevice:$source_device,
		          //      value:$value,
		          //      textFromAudio:$text_from_audio,
		          //      files:$files
	           //     ){
            //            datapoint
            //            {
            //                id
		          //          category
		          //          owner
		          //          {
			         //           username
		          //          }
            //            }
            //        }
            //    }";

            var Query = "mutation testmutation($datetime:DateTime, $category:CategoryTypes, $source_device:String!, $value:Float, $text_from_audio:String, $files:Upload!) {createDatapoint(datetime:$datetime, category:$category, sourceDevice:$source_device, value:$value, textFromAudio:$text_from_audio, files:$files){ datapoint{ id datetime category sourceDevice textFromAudio }}}";

            string jsonString = await uploadFileGenericAsync(Query, variables, filepath);

            if (jsonString.Contains("errors"))
            {
                return null;
            }

            return DeserializeDatapoint(jsonString);
        }

        private string serializeVariablesFromObject(Datapoint obj, int add_files = 0)
        {

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = new UnderscorePropertyNamesContractResolver()
            });

            //List<System.Reflection.PropertyInfo> problist = obj.GetType().GetProperties().ToList();

            //List<String> newlist = problist.Select(x => x.Name).ToList();

            //newlist.ForEach(x => json = json.Replace(x, x.ToUnderscoreCase()));

            string replaceString = "}";
            if(add_files == 1 )
            {
                replaceString = ",\"files\": null }";
            }
            else if(add_files == 2)
            {
                replaceString = ",\"files\": [null, null] }";
            }

            json = "\"variables\": " + json;
            return json.Replace("}", replaceString);
        }

        private static Datapoint DeserializeDatapoint(string jsonString)
        {
            dynamic dDatapoint = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

            var ddp = dDatapoint.data.createDatapoint.datapoint;

            Datapoint datapoint = new Datapoint()
            {
                Datetime = ddp.datetime,
                Category = ddp.category,
                SourceDevice = ddp.sourceDevice,
                TextFromAudio = ddp.textFromAudio
            };

            return datapoint;
        }


        private async Task<string> uploadFileGenericAsync(string query, string variables, string filepath = "")
        {
            string filename = Path.GetFileName(filepath);


            Dictionary<string, Object> postParameters = new Dictionary<string, object>
            {
                {"operations" , "{ \"query\": \"" + query + "\"," + variables + "}"},
                {"map", "{\"0\":[\"variables.files\"]}" },
                { "0", filepath != null ? new FileParameter(File.ReadAllBytes(filepath), filename) : null },
          };

            var response = await MultipartFormDataPostAsync(url, userAgent, postParameters);

            return response.ToString();
        }

        private async Task<string> upload2FilesGenericAsync(string query, string variables, string filepath1 = "", string filepath2 = "")
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

            var response = await MultipartFormDataPostAsync(url, userAgent, postParameters);

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

            var response = MultipartFormDataPostAsync(url, userAgent, postParameters);
            return true;
        }

        private bool uploadFile(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            Dictionary<string, Object> postParameters = new Dictionary<string, object>
            {
                {"operations" , "{ \"query\": \"mutation ($file: Upload!) { uploadFile(file: $file) { success } }\",\"variables\":{\"file\": null} }" },
                {"map", "{\"0\":[\"variables.file\"]}" },
                { "0", new FileParameter(File.ReadAllBytes(filepath), filename) }
            };

            var response = MultipartFormDataPostAsync(url, userAgent, postParameters);
            return true;
        }


        // Implements multipart/form-data POST in C# http://www.ietf.org/rfc/rfc2388.txt
        // http://www.briangrinstead.com/blog/multipart-form-post-in-c

        
        private async Task<string> MultipartFormDataPostAsync(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return await PostFormAsync(postUrl, userAgent, contentType, formData);
        }

        private async Task<string> PostFormAsync(string postUrl, string userAgent, string contentType, byte[] formData)
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
            if (Token != null)
            {
                request.Headers.Add("Authorization", "JWT " + Token);
            }
           

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            var loWebResponse = await request.GetResponseAsync();

            HttpWebResponse response = (HttpWebResponse)loWebResponse;

            StreamReader loResponseStream = new StreamReader(loWebResponse.GetResponseStream(), encoding);

            string lcHtml = loResponseStream.ReadToEnd();

            if (lcHtml.Contains("error"))
            {
                ;
            }

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

    public class UnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public UnderscorePropertyNamesContractResolver() : base()
        {
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(propertyName, @"(\w)([A-Z])", "$1_$2").ToLower();
        }
    }
}