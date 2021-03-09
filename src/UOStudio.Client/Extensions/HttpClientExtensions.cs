using System.Threading.Tasks;
using Newtonsoft.Json;

namespace System.Net.Http
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent httpContent)
        {
            var contentString = await httpContent.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentString);
        }
    }
}
