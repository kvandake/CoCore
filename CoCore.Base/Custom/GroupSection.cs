using System.Collections.Generic;

namespace CoCore.Base
{
    public class GroupSection : ExtendedObservableCollection<GroupCell>
    {

        public float Height { get; set; }

        public string Title { get; set; }


        public GroupSection()
        {
        }


        public GroupSection(IEnumerable<GroupCell> items) : base(items)
        {
        }


        public GroupSection(string title)
        {
            Title = title;
        }

    }
}

