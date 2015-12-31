using System.Collections.ObjectModel;

namespace CoCore.Base
{
	public class GroupSection : ObservableCollection<GroupCell>
	{

		public string Title { get; set;}


		public GroupSection(){
		}

		public GroupSection(string title){
			Title = title;
		}
			




	}
}

