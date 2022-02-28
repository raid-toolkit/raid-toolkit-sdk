using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.DataServices
{
	public interface IDataServiceSettings
	{
		public string InstallationPath { get; }
		public string StoragePath { get; }
	}
}
