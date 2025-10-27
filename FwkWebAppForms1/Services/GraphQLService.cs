using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FwkWebAppForms1.Services
{
    public class GraphQLService
    {
        private readonly string _graphqlEndpoint;
        private const int TimeoutSeconds = 30;

        public GraphQLService(string graphqlEndpoint)
        {
            _graphqlEndpoint = graphqlEndpoint ?? throw new ArgumentNullException(nameof(graphqlEndpoint));
        }

        /// <summary>
        /// Execute a GraphQL query and return the response
        /// </summary>
        public GraphQLResponse ExecuteQuery(string query, object variables = null)
        {
            try
            {
                var requestPayload = new
                {
                    query = query,
                    variables = variables
                };

                string jsonPayload = JsonConvert.SerializeObject(requestPayload);

                using (WebClient client = new WebClient())
                {
                    // Set timeout
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("Content-Type", "application/json");

                    string responseContent = client.UploadString(_graphqlEndpoint, "POST", jsonPayload);
                    var result = JsonConvert.DeserializeObject<JObject>(responseContent);

                    if (result == null)
                    {
                        return new GraphQLResponse
                        {
                            Success = false,
                            Error = "Empty response from GraphQL endpoint"
                        };
                    }

                    if (result["errors"] != null)
                    {
                        return new GraphQLResponse
                        {
                            Success = false,
                            Error = result["errors"].ToString()
                        };
                    }

                    return new GraphQLResponse
                    {
                        Success = true,
                        Data = result["data"]
                    };
                }
            }
            catch (WebException webEx)
            {
                string errorMessage = webEx.Message;
                try
                {
                    if (webEx.Response != null)
                    {
                        using (var reader = new System.IO.StreamReader(webEx.Response.GetResponseStream()))
                        {
                            string responseText = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(responseText))
                            {
                                errorMessage = responseText;
                            }
                        }
                    }
                }
                catch { }

                return new GraphQLResponse
                {
                    Success = false,
                    Error = $"HTTP Error: {errorMessage}"
                };
            }
            catch (TimeoutException ex)
            {
                return new GraphQLResponse
                {
                    Success = false,
                    Error = $"Request timed out after {TimeoutSeconds} seconds: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new GraphQLResponse
                {
                    Success = false,
                    Error = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Execute a GraphQL query asynchronously and return the response
        /// </summary>
        public async Task<GraphQLResponse> ExecuteQueryAsync(string query, object variables = null)
        {
            return await Task.Run(() => ExecuteQuery(query, variables));
        }
    }

    public class GraphQLResponse
    {
        public bool Success { get; set; }
        public JToken Data { get; set; }
        public string Error { get; set; }
    }
}
