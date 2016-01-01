using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CoCore.Base
{
	public class GroupRoot : ObservableCollection<GroupSection>
	{

	    public void UpdateSection(GroupSection section)
	    {
	        var index = IndexOf(section);
	        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
	            new[] {section}, new[] {section}, index));
	    }


	    public void UpdateSection(int index)
	    {
	        var section = this[index];
	        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
	            new[] {section}, new[] {section}, index));
	    }
	}
}

