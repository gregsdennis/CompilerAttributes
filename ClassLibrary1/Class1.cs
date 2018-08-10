using System;
using System.Collections.Generic;
using CompilerAttributes;

namespace ClassLibrary1
{
	public class Usage
	{
		[Experimental]
		private Class1 _field = new Class1();

		public Class1 Property { get; set; }
		public Class1 this[Class1 arg1, Class1 arg2, int arg3] { get { return null; } set { } }

		public event EventHandler<Class1> Event;

		public Class1 Method<T>(Class1 arg1, Class1 arg2, int arg3)
			where T : Generic<Class1>
		{
			Class1 temp = null;
			var dictionary = new Dictionary<int, Class1>();

			Console.WriteLine(nameof(Class1));

			_field = new Class1();
			return _field;
		}

		public void Method<T1, T2>() { }
	}

	[Experimental]
	public class Class1
	{
	}

	[Experimental]
	public class ExperimentalGeneric<T>
	{
	}

	public class Derived : Class1
	{
		public Generic<Class1> Test { get; set; } = new Generic<Class1>();

		public void Main()
		{
			var usage = new Usage();
			usage.Method<int, Class1>();
		}
	}

	public class Generic<T> where T : Class1
	{
		public ExperimentalGeneric<T> GenericProperty { get; set; }
	}

	public class DerivedGeneric : Generic<Class1>
	{
	}

	[GeneratesWarning("This feature is experimental.  Please use with care.")]
	public class ExperimentalAttribute : Attribute
	{
	}
}

namespace Other
{
	public class Class1
	{
	}

	public class Usage
	{
		public Class1 Property { get; set; }
	}
}