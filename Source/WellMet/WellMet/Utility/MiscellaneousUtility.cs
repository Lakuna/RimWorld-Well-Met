#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using RimWorld;
#if !V1_0
using System;
#endif
#if V1_0 || V1_1 || V1_2 || V1_3
using System.Reflection;
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
	}
}
