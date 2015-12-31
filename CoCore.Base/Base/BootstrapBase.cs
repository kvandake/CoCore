namespace CoCore.Base
{
	public abstract class BootstrapBase
	{
	    static volatile BootstrapBase _locator;

		internal static BootstrapBase Locator => _locator;

	    public IIocContainer IocContainer { get; }


	    protected BootstrapBase (IIocContainer container)
		{
			IocContainer = container;
			_locator = this;
		}
	}
}

