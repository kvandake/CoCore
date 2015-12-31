using System.Collections.Generic;
using System.Linq;

namespace CoCore.Base
{
	public sealed class Intent
	{
		readonly INavigation _navigation;


		/// <summary>
		/// remove intent after navigate
		/// </summary>
		/// <value><c>true</c> if remove to navigate; otherwise, <c>false</c>.</value>
		public bool RemoveToNavigate { get ; set;}
		
		IDictionary<string,object> _data;


		IDictionary<string, object> Data => _data ?? (_data = new Dictionary<string,object> ());

	    public object[] Values => Data.Values.ToArray ();


	    public Intent(INavigation navigation){
			_navigation = navigation;
		}


		public bool ContainsKey(string key){
			return Data.ContainsKey (key);
		}

		public T Get<T>(string key) {
			return Data.ContainsKey (key) ? (T)Data [key] : default(T);
		}


		public void Put(string key, object value){
			Data [key] = value;
		}

		public void Remove(string key){
			if (Data.ContainsKey (key)) {
				Data.Remove (key);
			}
		}


		public void Navigate(string key,params string[] parameters){
			_navigation.Navigate (key,this,parameters);
		}

		public bool GoBack(){
			return _navigation.GoBack ();
		}
			
		public bool GoBackWithResult(){
			return _navigation.GoBackWithResult (this);
		}
	}
}

