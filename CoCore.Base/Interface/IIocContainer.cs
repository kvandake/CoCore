using System;

namespace CoCore.Base
{
	public interface IIocContainer : IDisposable
	{
		void Register(Type type,string name = null, params object[] parameters);

		void Register<TImplement> (Func<TImplement> lazyInit);

		T Resolve<T>(string name = null);
	}
}

