using System.Net.Http.Json;

namespace SmartWeightLib.Models.Api
{
    public class ApiClient
    {
        protected readonly string ApiUrl;

        private readonly HttpClient _client;
        private readonly Dictionary<Endpoints, string> _endpoints;

        public ApiClient()
        {
            ApiUrl = "https://localhost:7065/api";
            _client = new HttpClient();
            _endpoints = new Dictionary<Endpoints, string>()
            {
                {Endpoints.CONNECTIONS, "connections"},
                {Endpoints.MEASUREMENTS, "measurements"},
                {Endpoints.MEASUREMENTS_OVERVIEW, "measurements/overview"},
                {Endpoints.MEASUREMENTS_PARTIALS, "measurements/partials"},
                {Endpoints.USERS, "users"},
                {Endpoints.USERS_LOGIN, "users/login"},
                {Endpoints.WEIGHTS, "weights"},
            };
        }
        public ApiClient(string? apiUrl, Dictionary<Endpoints, string>? endpoints, HttpClient? client) : base()
        {
            ApiUrl = apiUrl ?? ApiUrl!;
            _client = client ?? _client!;
            _endpoints = endpoints ?? _endpoints!;
        }

        #region Request => HttpResponseMessage
        public async Task<SimpleResponse> Post<Content>(Endpoints endpoint, string url = "", Content? content = default) => await Request(HttpMethod.Post, endpoint, url, content);
        public async Task<SimpleResponse> Post<Content>(Endpoints endpoint, Content? content = default) => await Request(HttpMethod.Post, endpoint, "", content);

        public async Task<SimpleResponse> Get(Endpoints endpoint, string url = "") => await Request<object>(HttpMethod.Get, endpoint, url);
        public async Task<SimpleResponse> Put<Content>(Endpoints endpoint, string url = "", Content? content = default) => await Request(HttpMethod.Put, endpoint, url, content);
        public async Task<SimpleResponse> Delete(Endpoints endpoint, string url = "") => await Request<object>(HttpMethod.Delete, endpoint, url);

        private async Task<SimpleResponse> Request<Content>(HttpMethod method, Endpoints endpoint, string url = "", Content? content = default) =>
            new SimpleResponse(
                await _client.SendAsync(
                    new HttpRequestMessage(method, $"{ApiUrl}/{_endpoints[endpoint]}/{url}")
                    {
                        Content = content is not null ? JsonContent.Create(content) : null
                    }
                )
            );
        #endregion

        //#region Request => string
        //public async Task<string> PostString<Content>(Endpoints endpoint, string url, Content? content) => await RequestString(HttpMethod.Post, endpoint, url, content);
        //public async Task<string> GetString(Endpoints endpoint, string url) => await RequestString<object>(HttpMethod.Get, endpoint, url);
        //public async Task<string> PutString<Content>(Endpoints endpoint, string url, Content? content) => await RequestString<Content>(HttpMethod.Put, endpoint, url, content);
        //public async Task<string> DeleteString(Endpoints endpoint, string url) => await RequestString<object>(HttpMethod.Delete, endpoint, url);

        //private async Task<string> RequestString<Content>(HttpMethod method, Endpoints endpoint, string url, Content? content = default)
        //{
        //    SimpleResponse response = await Request(method, endpoint, url, content);
        //    return response.Message;
        //}
        //#endregion
    }
}
