using System;

namespace CoCore.Base
{
	public interface IMessagePresenter
	{

		string Header { get; set;}

		string OkText { get; set;}

		string CancelText { get; set;}

		void Message(string text, string description,Action okClick,Action cancelClick, params object[] parameters);

		void Toast(string text, params object[] parameters);
	}
}

