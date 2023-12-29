using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace TFA.Scraper.Extensions;

internal static class HttpClientExtensions
{
    internal static Task<HttpResponseMessage> ExecutePostAsync<TData>(this HttpClient client, string url, TData data) 
        where TData : class
    {
        return client.PostAsync(new Uri(url), new StringContent(
            JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            }),
            Encoding.UTF8,
            Application.Json));
    }
}
