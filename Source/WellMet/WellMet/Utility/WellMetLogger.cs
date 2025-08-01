using System;
using Verse;

namespace Lakuna.WellMet.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for writing information to the log.
	/// </summary>
	internal static class WellMetLogger {
		/// <summary>
		/// The prefix that is placed before all logged messages from this mod.
		/// </summary>
		private const string Prefix = "Well Met encountered an exception: ";

		/// <summary>
		/// Write information about an exception to the log.
		/// </summary>
		/// <param name="e">The exception.</param>
		/// <param name="description">A description of the exception.</param>
		/// <param name="category">The exception's category. If this isn't the default category, only one message of this category can be printed.</param>
		/// <exception cref="ArgumentNullException">When no exception is given.</exception>
		internal static void LogException(Exception e, string description = "No description provided.", WellMetLoggerCategory category = WellMetLoggerCategory.Unrestricted) {
			if (e == null) {
				throw new ArgumentNullException(nameof(e));
			}

			string output = Prefix + description + "\n";

			Exception innerException = e;
			while (innerException != null) {
				output += "\n> " + innerException.Message;
				innerException = innerException.InnerException;
			}

			output += "\n\nStack trace:\n" + e.StackTrace + "\n\n";

			if (category == WellMetLoggerCategory.Unrestricted) {
				Log.Error(output);
				return;
			}

			Log.ErrorOnce(output, (int)category);
		}

		/// <summary>
		/// Write information about an error with no corresponding exception to the log.
		/// </summary>
		/// <param name="e">The error message.</param>
		/// <param name="category">The error's category. If this isn't the default category, only one message of this category can be printed.</param>
		internal static void LogErrorMessage(string e, WellMetLoggerCategory category = WellMetLoggerCategory.Unrestricted) {
			if (category == WellMetLoggerCategory.Unrestricted) {
				Log.Error(Prefix + e);
				return;
			}

			Log.ErrorOnce(Prefix + e, (int)category);
		}

		/// <summary>
		/// Write a non-error, non-exception message to the log.
		/// </summary>
		/// <param name="message">The message.</param>
		internal static void LogMessage(string message) => Log.Message(message);
	}
}
