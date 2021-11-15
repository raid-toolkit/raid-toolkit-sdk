using System;
using Raid.Client;

namespace Sample
{
	class Program
	{
		static async void Main(string[] args)
		{
			RaidToolkitClient client = new();
			client.Connect();
			await client.EnsureInstalled();
		}
	}
}
