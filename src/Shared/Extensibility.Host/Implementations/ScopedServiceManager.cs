using Raid.Toolkit.Extensibility.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Services
{
	public class ScopedServiceManager : IScopedServiceManager
	{
		public void AddMessageScopeHandler(IMessageScopeHandler handler)
		{
			throw new NotImplementedException();
		}

		public ValueTask ProcessMessage(ISocketSession session, string message)
		{
			throw new NotImplementedException();
		}

		public void RemoveMessageScopeHandler(IMessageScopeHandler handler)
		{
			throw new NotImplementedException();
		}
	}
}
