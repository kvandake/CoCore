using System.Windows.Input;

namespace CoCore.Base
{
	public class GroupCell
	{
		public string Tag { get; set;}

		public GroupCellStyle CellStyle { get; set;}

		public string PrimaryText { get; set;}

		public string SecondaryText { get; set;}

		public object Data { get; set;}

		public bool IsSelected { get; set;}

		public ICommand Command { get; set;}

		public ICommand AccessoryCommand { get; set;}

		public float Height {get;set;}

	}
}

