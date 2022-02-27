using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility
{
	public interface IAccount
	{
		public string Id { get; }
		public string Name { get; }
	}
}
