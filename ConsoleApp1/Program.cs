using ClassLibrary1;
using System;

namespace ConsoleApp1
{
    class Program
    {
		public static Class1 Property { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine(Property == null);
        }
    }
}
