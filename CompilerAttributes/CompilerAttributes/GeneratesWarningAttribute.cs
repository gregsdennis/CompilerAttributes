using System;

namespace CompilerAttributes
{
	/// <summary>
	/// Directs the compiler to generate warnings for usages of identifiers decorated with an attribute that is decorated with this attribute.
	/// </summary>
	public sealed class GeneratesWarningAttribute : Attribute
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
		public GeneratesWarningAttribute(string messageFormat)
		{
			//Id = code;
			MessageFormat = messageFormat;
		}
	}
}