using System;
using Verse;

namespace Lakuna.WellMet.Utility {
	internal static class WellMetLogger {
		private const string Prefix = "Well Met encountered an exception: ";

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

		internal static void LogErrorMessage(string e, WellMetLoggerCategory category = WellMetLoggerCategory.Unrestricted) {
			if (category == WellMetLoggerCategory.Unrestricted) {
				Log.Error(Prefix + e);
				return;
			}

			Log.ErrorOnce(Prefix + e, (int)category);
		}

		internal static void LogMessage(string message) => Log.Message(message);
	}
}
