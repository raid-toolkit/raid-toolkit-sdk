using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using GitHub.Schema;
using Raid.Toolkit.Common;

namespace GitHub
{
    public class Updater : IDisposable
    {
        private readonly Uri UpdateUri = new($"https://api.github.com/repos/raid-toolkit/raid-toolkit-sdk/releases/latest");
        private readonly Uri ReleasesUri = new($"https://api.github.com/repos/raid-toolkit/raid-toolkit-sdk/releases");
        private HttpClient Client;
        private bool disposedValue;

        public Updater()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.Add(new("RaidToolkit", Assembly.GetExecutingAssembly().GetName().Version.ToString(2)));
        }

        public async Task<Release> GetLatestRelease()
        {
            if (!RegistrySettings.InstallPrereleases)
            {
                return await Client.GetObjectAsync<Release>(UpdateUri);
            }
            Release[] allReleases = await Client.GetObjectAsync<Release[]>(ReleasesUri);
            Release latestPrerelease = allReleases.FirstOrDefault(release => release.Prerelease);
            return latestPrerelease;
        }

        public Task<Stream> DownloadRelease(Release release)
        {
            Asset asset = release.Assets.FirstOrDefault(asset => asset.Name == "Raid.Toolkit.exe");
            return asset == null
                ? throw new FileNotFoundException("Update is missing required assets")
                : Client.GetStreamAsync(asset.BrowserDownloadUrl);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Client.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
