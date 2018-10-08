using System;

namespace CompilerAttributes
{
	/// <summary>
	/// Directs the compiler to generate errors for usages of identifiers decorated with an attribute that is decorated with this attribute.
	/// </summary>
	/// <remarks>
	/// Ideal use cases for this are elements that you must expose but don't want clients to use.
	/// </remarks>
	public sealed class GeneratesErrorAttribute : Attribute
	{
		//public int Id { get; }
		/// <summary>
		/// The message format output by the compiler.
		/// </summary>
		/// <remarks>
		/// The identifier name will be supplied for the first argument (i.e. <code>{0}</code>).
		/// </remarks>
		public string MessageFormat { get; }

		/// <summary>
		/// Directs the compiler to generate warnings for usages of identifiers decorated with an attribute that is decorated with this attribute.
		/// </summary>
		/// <param name="messageFormat">The message output by the compiler.</param>
		public GeneratesErrorAttribute(string messageFormat)
		{
			//Id = code;
			MessageFormat = messageFormat;
		}
	}
}