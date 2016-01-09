using System;

namespace CoCore.Base
{
	public class MessageData<T>
	{
		public string Title {get;set;}

		public T Data {get;set;}

		public Action<T> Selected {get;set;}
	}
}

