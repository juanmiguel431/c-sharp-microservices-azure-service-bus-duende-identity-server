using System.Net.Http.Headers;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Services;

public class BaseService : IBaseService
{
    public ResponseDto ResponseDto { get; set; }
    private HttpClient HttpClient { get; }

    protected BaseService(HttpClient httpClient)
    {
        ResponseDto = new ResponseDto();
        HttpClient = httpClient;
    }

    public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var message = new HttpRequestMessage();
            // message.Headers.Add("Accept", "application/json");

            message.RequestUri = new Uri(apiRequest.Url, UriKind.Relative);

            // HttpClient.DefaultRequestHeaders.Clear();

            if (apiRequest.Data != null)
            {
                var stringJson = JsonConvert.SerializeObject(apiRequest.Data);
                message.Content = new StringContent(stringJson, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(apiRequest.AccessToken))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
            }

            switch (apiRequest.ApiType)
            {
                case Sd.ApiType.Get:
                    message.Method = HttpMethod.Get;
                    break;
                case Sd.ApiType.Post:
                    message.Method = HttpMethod.Post;
                    break;
                case Sd.ApiType.Put:
                    message.Method = HttpMethod.Put;
                    break;
                case Sd.ApiType.Patch:
                    message.Method = HttpMethod.Patch;
                    break;
                case Sd.ApiType.Delete:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            var apiResponse = await HttpClient.SendAsync(message);

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

            return apiResponseDto;
        }
        catch (Exception e)
        {
            var dto = new ResponseDto
            {
                DisplayMessage = "Error",
                ErrorMessages = new List<string>() { e.Message },
                IsSuccess = false
            };

            var res = JsonConvert.SerializeObject(dto);
            var apiResponseDto = JsonConvert.DeserializeObject<T>(res);

            return apiResponseDto;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

}