using System;

namespace CoCore.Base
{
	[Flags]
	public enum GroupCellStyle
	{
		None = 1,
		Primary = 2,
		Secondary = 4,
		RowClick = 8,
		Detail = 16,
		Disclosure = 32,
		CheckMark = 64,
		Custom = 128
	}
}

