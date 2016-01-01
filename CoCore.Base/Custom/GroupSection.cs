using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CoCore.Base
{
    public class GroupSection : ObservableCollection<GroupCell>
    {

        public string Title { get; set; }


        public GroupSection()
        {
        }

        public GroupSection(string title)
        {
            Title = title;
        }




        public void UpdateCell(GroupCell cell)
        {
            var index = IndexOf(cell);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new[] {cell}, new[] {cell}, index));

        }


        public void UpdateCell(int index)
        {
            var cell = this[index];
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new[] { cell }, new[] { cell }, index));
        }

    }
}

