using System.Net;
using System.Net.Http.Json;

namespace SmartWeightLib.Models.Api
{
    public class SimpleResponse
    {
        public HttpResponseMessage Response { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode => Response.StatusCode;
        public T? GetContent<T>() => Response.Content.ReadFromJsonAsync<T>().Result;
        public bool IsSuccess => Response.IsSuccessStatusCode;

        public SimpleResponse(HttpResponseMessage response)
        {
            Response = response;
            Message = response.Content.ReadAsStringAsync().Result;
        }
    }
}
