namespace System.Threading.Tasks;

public static class TaskExtensions
{
	public static T GetResultSync<T>(this Task<T> task)
	{
		task.Wait();
		if (task.Status == TaskStatus.Canceled)
			throw (Exception?)task.Exception ?? new TaskCanceledException();
		if (task.Status == TaskStatus.Faulted)
			throw (Exception?)task.Exception ?? new IndexOutOfRangeException();
		return (T)task.Result;
	}
}
