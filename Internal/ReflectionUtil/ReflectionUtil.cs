using System;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework
{
	public static class ReflectionUtil
	{
		public static Type[] FindClassWithInterface<T> ()
		{
			return FindClassWithInterface (typeof(T));
		}

		public static Type[] FindClassWithInterface (Type type)
		{

			List<Type> list = new List<Type> ();
			Type[] resultTypes = System.Reflection.Assembly.GetAssembly (type).GetTypes ();
			for (int i = 0; i < resultTypes.Length; i++) {
				if (type.IsAssignableFrom (resultTypes [i]) && !resultTypes [i].IsInterface) {
					list.Add (resultTypes [i]);
				}
			}
			return list.ToArray ();
		}

		public static Type[] FindSubClass<T> ()
		{
			return FindSubClass (typeof(T));
		}

		public static Type[] FindSubClass (Type type)
		{
			List<Type> list = new List<Type> ();
			Type[] resultTypes = System.Reflection.Assembly.GetAssembly (type).GetTypes ();



			for (int i = 0; i < resultTypes.Length; i++) {
			
				if (resultTypes [i].IsSubclassOf (type)

				  && !(type.IsAbstract && resultTypes [i] == type)) {
					list.Add (resultTypes [i]);
				}
			}
			return list.ToArray ();
		}
	}
}

