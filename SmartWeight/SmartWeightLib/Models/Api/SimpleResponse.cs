using System.Net;
using System.Net.Http.Json;

namespace SmartWeightLib.Models.Api
{
    public class SimpleResponse
    {
        public string Url { get; }
        public HttpResponseMessage Response { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode => Response.StatusCode;
        public T? GetContent<T>()
        {
            try
            {
                return Response.Content.ReadFromJsonAsync<T>().Result;
            } 
            catch
            {
                return default;
            }
        }
        public bool IsSuccess => Response.IsSuccessStatusCode;

        public SimpleResponse(string url, HttpResponseMessage response)
        {
            Url = url;
            Response = response;
            Message = response.Content.ReadAsStringAsync().Result;
        }
    }
}
