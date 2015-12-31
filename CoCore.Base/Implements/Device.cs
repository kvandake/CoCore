using System;
using Plugin.DeviceInfo;

namespace CoCore.Base
{
	public static class Device
	{
		public static T OnPlatform<T>(Func<T> iosFunc, Func<T> androidFunc,Func<T> windowsFunc){
			switch (CrossDeviceInfo.Current.Platform) {
			case Plugin.DeviceInfo.Abstractions.Platform.Android:
				return androidFunc();
			case Plugin.DeviceInfo.Abstractions.Platform.iOS:
				return iosFunc();
			case Plugin.DeviceInfo.Abstractions.Platform.Windows:
				return windowsFunc();
			default:
				return default(T);
			}
		}
	}
}

