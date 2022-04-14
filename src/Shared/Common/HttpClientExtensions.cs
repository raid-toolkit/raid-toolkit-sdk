using System.Threading.Tasks;
using Newtonsoft.Json;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetObjectAsync<T>(this HttpClient client, Uri uri)
        {
            string response = await client.GetStringAsync(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}
