#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using RimWorld;
using System;
#if V1_0 || V1_1 || V1_2 || V1_3
using System.Reflection;
#endif
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
using System.Text;
#endif
using Verse;

namespace Lakuna.WellMet.Utility {
	/// <summary>
	/// Miscellaneous utility methods.
	/// </summary>
	public static class MiscellaneousUtility {
		/// <summary>
		/// Equivalent to `Pawn.IsFreeNonSlaveColonist` for all versions of RimWorld.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>`Pawn.IsFreeNonSlaveColonist`.</returns>
		public static bool IsFreeNonSlaveColonist(Pawn pawn) => pawn != null
#if V1_0 || V1_1 || V1_2
			&& pawn.IsFreeColonist;
#else
			&& pawn.IsFreeNonSlaveColonist;
#endif

		/// <summary>
		/// Equivalent to `Pawn.IsAnimal` for all versions of RimWorld.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>`Pawn.IsAnimal`.</returns>
		public static bool IsAnimal(Pawn pawn) => pawn != null
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			&& pawn.RaceProps.Animal;
#else
			&& pawn.IsAnimal;
#endif

		/// <summary>
		/// Equivalent to `Pawn.IsPlayerControlled` for all versions of RimWorld.
		/// </summary>
		/// <param name="pawn">The pawn.</param>
		/// <returns>`Pawn.IsPlayerControlled`.</returns>
		public static bool IsPlayerControlled(Pawn pawn) => pawn != null
#if V1_0 || V1_1 || V1_2 || V1_3
			&& pawn.IsColonistPlayerControlled;
#elif V1_4
			&& (pawn.IsColonistPlayerControlled || pawn.IsColonyMechPlayerControlled);
#else
			&& pawn.IsPlayerControlled;
#endif

		/// <summary>
		/// Make an empty array of objects.
		/// </summary>
		/// <returns>The array.</returns>
		public static object[] EmptyArray() => EmptyArray<object>();

		/// <summary>
		/// Make an empty array of the given type.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>The array.</returns>
		public static T[] EmptyArray<T>() =>
#if V1_0
			new T[] { };
#else
			Array.Empty<T>();
#endif

		/// <summary>
		/// End the given string with a period.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <returns>The string.</returns>
		public static string EndWithPeriod(string s) =>
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			s + ".";
#else
			s.EndWithPeriod();
#endif

#if !V1_0
		/// <summary>
		/// End the given tagged string with a period.
		/// </summary>
		/// <param name="s">The tagged string.</param>
		/// <returns>The tagged string.</returns>
		public static TaggedString EndWithPeriod(TaggedString s) =>
#if V1_1 || V1_2 || V1_3 || V1_4
			s + ".";
#else
			s.EndWithPeriod();
#endif
#endif


#if V1_0 || V1_1 || V1_2 || V1_3
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(SkillRecord), "pawn");

		public static Pawn PawnOfSkillRecord(SkillRecord skill) => PawnField.GetValue(skill) as Pawn;
#else
		public static Pawn PawnOfSkillRecord(SkillRecord skill) => skill?.Pawn;
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		/// <summary>
		/// Split up a long string into lines. Almost equivalent to `str.AddLineBreaksToLongString();`.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The split-up string.</returns>
		public static string AddLineBreaksToLongString(string str) {
			StringBuilder stringBuilder = new StringBuilder((str?.Length ?? 0) + (str.Length / 100 + 3) * 2 + 1);
			_ = stringBuilder.AppendLine();
			for (int i = 0; i < str.Length; i++) {
				if (i % 100 == 0 && i != 0) {
					_ = stringBuilder.AppendLine();
				}

				_ = stringBuilder.Append(str[i]);
			}

			_ = stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}
#else
		/// <summary>
		/// Split up a long string into lines. Equivalent to `str.AddLineBreaksToLongString();`.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The split-up string.</returns>
		public static string AddLineBreaksToLongString(string str) => str.AddLineBreaksToLongString();
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		/// <summary>
		/// Compress a long string into one line. Equivalent to `str.RemoveLineBreaks();`.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The compressed string.</returns>
		public static string RemoveLineBreaks(string str) => new StringBuilder(str).Replace("\n", "").Replace("\r", "").ToString();
#else
		/// <summary>
		/// Compress a long string into one line. Equivalent to `str.RemoveLineBreaks();`.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The compressed string.</returns>
		public static string RemoveLineBreaks(string str) => str.RemoveLineBreaks();
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		/// <summary>
		/// Save a Boolean array. Almost equivalent to `DataExposeUtility.LookBoolArray`.
		/// </summary>
		/// <param name="arr">The array to save.</param>
		/// <param name="elements">The number of elements in the array.</param>
		/// <param name="label">The name of the array in the save data.</param>
		public static void LookBoolArray(ref bool[] arr, int elements, string label) {
			if (Scribe.mode == LoadSaveMode.Saving && arr != null) {
				if (arr.Length != elements) {
					Log.ErrorOnce("Bool array length mismatch for " + label, 74135877);
				}

				elements = arr.Length;
			}

			int numBytes = (elements + 7) / 8;

			// Equivalent to `byte[] arr2 = DataExposeUtility.BytesToBits(arr, elements, numBytes);`.
			byte[] arr2 = null;
			if (Scribe.mode == LoadSaveMode.Saving && arr != null) {
				arr2 = new byte[numBytes];
				int num = 0;
				byte b = 1;
				for (int i = 0; i < elements; i++) {
					if (arr[i]) {
						arr2[num] |= b;
					}

					b *= 2;
					if (b == 0) {
						b = 1;
						num++;
					}
				}
			}

			// Equivalent to `DataExposeUtility.LookByteArray(ref arr2, label);`.
			if (Scribe.mode == LoadSaveMode.Saving && arr2 != null) {
				byte[] array = CompressUtility.Compress(arr2);
				if (array.Length < arr2.Length) {
					string value = AddLineBreaksToLongString(Convert.ToBase64String(array));
					Scribe_Values.Look(ref value, label + "Deflate");
				} else {
					string value = AddLineBreaksToLongString(Convert.ToBase64String(arr2));
					Scribe_Values.Look(ref value, label);
				}
			} else if (Scribe.mode == LoadSaveMode.LoadingVars) {
				string value = null;
				Scribe_Values.Look(ref value, label + "Deflate");
				if (value == null) {
					Scribe_Values.Look(ref value, label);
					arr2 = value == null ? null : Convert.FromBase64String(RemoveLineBreaks(value));
				} else {
					arr2 = CompressUtility.Decompress(Convert.FromBase64String(RemoveLineBreaks(value)));
				}
			}

			// Equivalent to `DataExposeUtility.BitsToBytes(ref arr, elements, arr2, numBytes);`.
			if (Scribe.mode != LoadSaveMode.LoadingVars || arr2 == null || arr2.Length == 0) {
				return;
			}
			if (arr == null) {
				arr = new bool[elements];
			}
			if (arr2.Length != numBytes) {
				int num = 0;
				byte b = 1;
				for (int i = 0; i < elements; i++) {
					arr[i] = (arr2[num] & b) != 0;
					b *= 2;
					if (b > 32) {
						b = 1;
						num++;
					}
				}

				return;
			}
			int num2 = 0;
			byte b2 = 1;
			for (int i = 0; i < elements; i++) {
				arr[i] = (arr2[num2] & b2) != 0;
				b2 *= 2;
				if (b2 == 0) {
					b2 = 1;
					num2++;
				}
			}
		}
#else
		/// <summary>
		/// Save a Boolean array. Equivalent to `DataExposeUtility.LookBoolArray`.
		/// </summary>
		/// <param name="arr">The array to save.</param>
		/// <param name="elements">The number of elements in the array.</param>
		/// <param name="label">The name of the array in the save data.</param>
		public static void LookBoolArray(ref bool[] arr, int elements, string label) => DataExposeUtility.LookBoolArray(ref arr, elements, label);
#endif
	}
}
