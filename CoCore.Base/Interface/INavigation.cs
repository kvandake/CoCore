using System;

namespace CoCore.Base
{
	public interface INavigation
	{


		/// <summary>
		/// Register the specified name and viewType.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="viewType">View type.</param>
		void Register(string name, Type viewType);


		/// <summary>
		/// Remove the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		void Remove(string name);


		/// <summary>
		/// Navigate the specified name, intent and parameters.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="intent">Intent.</param>
		/// <param name="parameters">Parameters.</param>
		void Navigate(string name,Intent intent,params string[] parameters);
	
		/// <summary>
		/// Go the back.
		/// </summary>
		/// <returns><c>true</c>, if back was gone, <c>false</c> otherwise.</returns>
		bool GoBack();


		/// <summary>
		/// Go the back with result.
		/// </summary>
		/// <returns><c>true</c>, if back with result was gone, <c>false</c> otherwise.</returns>
		bool GoBackWithResult(Intent intent);


		/// <summary>
		/// Convert the view.
		/// </summary>
		/// <param name="convert">Convert.</param>
		/// <param name="customParameter">Custom parameter.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		void ConvertView<T>(Func<T,T> convert,string customParameter);


	}
}

