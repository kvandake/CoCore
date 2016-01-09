using System;
using System.Collections.Generic;
using System.Threading;

namespace CoCore.Base
{
	public interface IMessagePresenter
	{

		string Header { get; set;}

		string OkText { get; set;}

		string CancelText { get; set;}

		void Message(string text, string description,Action okClick,Action cancelClick, params object[] parameters);

		void MessageForSelectItem<T>(string header, string description,List<MessageData<T>> items, Action cancelClick, params object[] parameters);

		void Dismiss();

		void Toast(string text, params object[] parameters);
	}
}

