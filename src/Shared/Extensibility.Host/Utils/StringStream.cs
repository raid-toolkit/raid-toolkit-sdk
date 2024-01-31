using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Utils;

public class StreamString
{
    private readonly Stream ioStream;
    private readonly UnicodeEncoding streamEncoding;

    public StreamString(Stream ioStream)
    {
        this.ioStream = ioStream;
        streamEncoding = new UnicodeEncoding();
    }

    public async Task<string> ReadStringAsync()
    {
        byte[] byteSize = new byte[sizeof(int)];
        await ioStream.ReadAsync(byteSize);
        int len = BitConverter.ToInt32(byteSize, 0);
        if (len < 0)
            return string.Empty;
        var inBuffer = new byte[len];
        await ioStream.ReadAsync(inBuffer.AsMemory(0, len));
        return streamEncoding.GetString(inBuffer);
    }

    public async Task<int> WriteStringAsync(string outString)
    {
        byte[] outBuffer = streamEncoding.GetBytes(outString);
        int len = outBuffer.Length;
        await ioStream.WriteAsync(BitConverter.GetBytes(len));
        await ioStream.WriteAsync(outBuffer.AsMemory(0, len));
        await ioStream.FlushAsync();

        return outBuffer.Length + 2;
    }
}
