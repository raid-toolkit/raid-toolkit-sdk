using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Common;

public static class SingletonProcess
{
    private static EventWaitHandle? GlobalHandle;
    public static readonly string Identifier = @"Global\4d5ed15b-cd8c-4fea-a086-e3675f18d34d";

    public static bool IsRunning
    {
        get
        {
            EventWaitHandle? tempHandle = null;
            try
            {
                tempHandle = new(false, EventResetMode.ManualReset, Identifier, out bool created);
                return !created;
            }
            finally
            {
                tempHandle?.Dispose();
            }
        }
    }

    public static bool TryAcquireSingleton()
    {
        // already aquired
        if (GlobalHandle != null)
            return true;

        try
        {
            GlobalHandle = new(false, EventResetMode.ManualReset, Identifier, out bool created);
            if (created) return true;
        }
        catch (Exception) { }

        // dispose immediately if we didn't aquire it
        GlobalHandle?.Dispose();
        GlobalHandle = null;
        return false;

    }

    public static async Task TryAcquireSingletonWithTimeout(int timeoutMs)
    {
        Stopwatch sw = new();
        sw.Start();
        do
        {
            if (TryAcquireSingleton())
                return;
            await Task.Delay(1000);
        }
        while (sw.ElapsedMilliseconds < timeoutMs);

        throw new TimeoutException();
    }
}
