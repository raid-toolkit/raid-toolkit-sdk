using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using GitHub.Schema;

using Raid.Toolkit.Common;

namespace GitHub;

public class Updater : IDisposable
{
    private readonly Uri UpdateUri = new($"https://api.github.com/repos/{RegistrySettings.Repository}/releases/latest");
    private readonly Uri ReleasesUri = new($"https://api.github.com/repos/{RegistrySettings.Repository}/releases");
    private readonly HttpClient Client;
    private bool disposedValue;

    public bool? InstallPrereleases { get; set; }

    private bool InstallPrereleasesEffective
    {
        get
        {
            if (InstallPrereleases.HasValue)
                return InstallPrereleases.Value;
            return RegistrySettings.InstallPrereleases;
        }
    }

    public Updater()
    {
        Client = new HttpClient();
        Client.DefaultRequestHeaders.UserAgent.Add(new("RaidToolkit", Assembly.GetExecutingAssembly().GetName().Version?.ToString(2)));
    }

    public async Task<Release?> GetLatestRelease()
    {
        if (!InstallPrereleasesEffective)
        {
            return await Client.GetObjectAsync<Release>(UpdateUri);
        }
        Release[] allReleases = await Client.GetObjectAsync<Release[]>(ReleasesUri);
        Release? latestPrerelease = allReleases.FirstOrDefault(release => release.Prerelease);
        return latestPrerelease;
    }

    public static bool IsValidRelease(Release release)
    {
        return release.Assets.Any(item => item.Name == "RaidToolkitSetup.exe");
    }

    public async Task<Stream> DownloadSetup(Release release, IProgress<float>? progress)
    {
        Asset? asset = release.Assets.FirstOrDefault(item => item.Name == "RaidToolkitSetup.exe");
        if (asset == null || asset.BrowserDownloadUrl == null)
            throw new FileNotFoundException("Release is missing required assets");

        Stream memoryStream = new MemoryStream();
        await Client.DownloadAsync(asset.BrowserDownloadUrl.ToString(), memoryStream, progress);
        return memoryStream;
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
