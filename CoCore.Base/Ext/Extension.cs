using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace CoCore.Base
{
	public static class Extension
	{
		/// <summary>
		/// Checks to see if an enumerated value contains a type.
		/// </summary>
		/// <returns><c>true</c> if has type value; otherwise, <c>false</c>.</returns>
		/// <param name="type">Type.</param>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static bool Has<T>(this Enum type, T value) {
			try {
				return (((int)(object)type & (int)(object)value) == (int)(object)value);
			} catch {
				return false;
			}
		}

		/// <summary>
		/// Creates the type of the for.
		/// </summary>
		/// <returns>The for type.</returns>
		/// <param name="type">Type.</param>
		/// <param name="parameters">Parameters.</param>
		public static object CreateForType(Type type,object[] parameters = null)
		{
			var typeInfo = type.GetTypeInfo ();
			var listParams = parameters.Select (x => x.GetType ()).ToList ();
			foreach (var constructor in typeInfo.DeclaredConstructors) {
				var declareParams = constructor.GetParameters ().Select (x => x.ParameterType).ToList ();
				if (listParams.Count == declareParams.Count) {
					if (DoListMatch (listParams, declareParams)) {
						return CreateForConstructorInfo (type, constructor, parameters);
					}
				}
			}
			return CreateForConstructorInfo (type, typeInfo.DeclaredConstructors.ElementAt (0), parameters);
			//TODO add create for parameter (multi)
		}


		/// <summary>
		/// Creates for constructor info.
		/// </summary>
		/// <returns>The for constructor info.</returns>
		/// <param name="type">Type.</param>
		/// <param name="constructor">Constructor.</param>
		/// <param name="parameters">Parameters.</param>
		internal static object CreateForConstructorInfo(Type type, ConstructorInfo constructor,object[] parameters)
		{
			var constructorParameters = constructor.GetParameters ();
			if (constructorParameters.Length == 0) {
				return Activator.CreateInstance (type);
			}
			var sortParams = new object[constructorParameters.Length];
			for (int i = 0; i < constructorParameters.Length; i++) {
				var parameter = constructorParameters [i];
				var paramType = parameter.ParameterType;
				var findItem = parameters.FirstOrDefault (x => x.GetType () == paramType);
				sortParams [i] = findItem ?? GetDefault (paramType);
			}
			return constructor.Invoke (sortParams);
		}

		public static object GetDefault(Type type)
		{
			return type.GetTypeInfo ().IsValueType ? Activator.CreateInstance (type) : null;
		}

		internal static bool DoListMatch(ICollection<Type> first, ICollection<Type> second)
		{
			return 
				first.Count == second.Count &&
			first.All (second.Contains) &&
			second.All (first.Contains);
		}
	}
}

