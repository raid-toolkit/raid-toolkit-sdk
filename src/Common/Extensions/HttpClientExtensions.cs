using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace System.Net.Http;

public static class HttpClientExtensions
{
	public static async Task<T> GetObjectAsync<T>(this HttpClient client, Uri uri)
	{
		string response = await client.GetStringAsync(uri).ConfigureAwait(false);
		return JsonConvert.DeserializeObject<T>(response)!;
	}

#if !NET5_0_OR_GREATER
	public static Task<Stream> ReadAsStreamAsync(this HttpContent content, CancellationToken _ = default)
	{
		return content.ReadAsStreamAsync();
	}
	public static Task CopyToAsync(this Stream source, Stream destination, CancellationToken cancellationToken = default)
	{
		return source.CopyToAsync(destination, 81920, cancellationToken);
	}
#endif


	public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
	{
		// Get the http headers first to examine the content length
		using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
		var contentLength = response.Content.Headers.ContentLength;

		using var download = await response.Content.ReadAsStreamAsync(cancellationToken);

		// Ignore progress reporting when no progress reporter was 
		// passed or when the content length is unknown
		if (progress == null || !contentLength.HasValue)
		{
			await download.CopyToAsync(destination, cancellationToken);
			return;
		}

		// Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
		var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
		// Use extension method to report progress while downloading
		await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
		progress.Report(1);
	}

	public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
	{
		if (source == null)
			throw new ArgumentNullException(nameof(source));
		if (!source.CanRead)
			throw new ArgumentException("Has to be readable", nameof(source));
		if (destination == null)
			throw new ArgumentNullException(nameof(destination));
		if (!destination.CanWrite)
			throw new ArgumentException("Has to be writable", nameof(destination));
		if (bufferSize < 0)
			throw new ArgumentOutOfRangeException(nameof(bufferSize));

		var buffer = new byte[bufferSize];
		long totalBytesRead = 0;
		int bytesRead;
		while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
		{
			await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
			totalBytesRead += bytesRead;
			progress?.Report(totalBytesRead);
		}
	}
}
