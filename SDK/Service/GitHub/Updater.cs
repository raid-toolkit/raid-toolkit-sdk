using System;
using System.Net.Http;
using System.Threading.Tasks;
using GitHub.Schema;
using Newtonsoft.Json;

namespace GitHub
{
    public class Updater
    {
        private Uri UpdateUri = new($"https://api.github.com/repos/raid-toolkit/raid-toolkit-sdk/releases/latest");
        private HttpClient Client;
        public Updater()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.Add(new("RaidToolkit", "1.0"));
        }

        public async Task<Release> GetLatest()
        {
            return JsonConvert.DeserializeObject<Release>(await Client.GetStringAsync(UpdateUri));
        }
    }
}