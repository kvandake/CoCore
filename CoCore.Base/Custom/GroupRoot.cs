using System.Collections.Generic;

namespace CoCore.Base
{
	public class GroupRoot : ExtendedObservableCollection<GroupSection>
	{
	    public GroupRoot()
	    {
	    }

	    public GroupRoot(IEnumerable<GroupSection> items) : base(items)
	    {
	    }
	}
}

