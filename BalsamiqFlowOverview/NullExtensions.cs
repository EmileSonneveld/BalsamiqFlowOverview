using System.Runtime.CompilerServices;

namespace BalsamiqFlowOverview
{
	/// <summary>
	/// To cast nullable to non-nullable types
	/// From here https://stackoverflow.com/a/70430634/1448736
	/// </summary>
	public static class NullExtensions
	{
		public static T ThrowIfNull<T>(
			this T? argument,
			string? message = default,
			[CallerArgumentExpression("argument")] string? paramName = default
		) where T : notnull
		{
			if (argument is null)
			{
				throw new ArgumentNullException(paramName, message);
			}
			else
			{
				return argument;
			}
		}

		public static T ThrowIfNull<T>(
			this T? argument,
			string? message = default,
			[CallerArgumentExpression("argument")] string? paramName = default
		) where T : unmanaged
		{
			if (argument is null)
			{
				throw new ArgumentNullException(paramName, message);
			}
			else
			{
				return (T)argument;
			}
		}
	}
}
