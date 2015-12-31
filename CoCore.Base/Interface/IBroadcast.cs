using System;

namespace CoCore.Base
{
	public interface IBroadcast
	{

		void SubscribeOnBroadcast();

		void UnscribeOnBroadcast();

		void Register<T>(Action<T> action, string token = null);

		void UnRegister<T>(string token = null);

		void Send<T>(T obj, string token = null);
	}
}

