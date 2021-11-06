using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using GitHub.Schema;

namespace GitHub
{
    public class Updater
    {
        private Uri UpdateUri = new($"https://api.github.com/repos/raid-toolkit/raid-toolkit-sdk/releases/latest");
        private HttpClient Client;

        public Updater()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.Add(new("RaidToolkit", Assembly.GetExecutingAssembly().GetName().Version.ToString(2)));
        }

        public Task<Release> GetLatestRelease()
        {
            return Client.GetObjectAsync<Release>(UpdateUri);
        }

        public Task<Stream> DownloadRelease(Release release)
        {
            Asset asset = release.Assets.FirstOrDefault(asset => asset.Name == "Raid.Service.exe");
            if (asset == null)
                throw new FileNotFoundException("Update is missing required assets");

            return Client.GetStreamAsync(asset.BrowserDownloadUrl);
        }
    }
}