#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !V1_0
using System;
#endif
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnCharacterPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Character), nameof(ITab_Pawn_Character.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
#if V1_0
		private static readonly MethodInfo PawnToShowInfoAboutMethod = AccessTools.Method(typeof(ITab_Pawn_Character), "get_PawnToShowInfoAbout");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo PawnToShowInfoAboutMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Character), "PawnToShowInfoAbout");
#endif

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Character __instance, ref bool __result) => __result = __result
#if V1_0
			&& (!(PawnToShowInfoAboutMethod.Invoke(__instance, Parameters) is Pawn pawn)
#else
			&& (!(PawnToShowInfoAboutMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, true) // Contains "rename," "banish," and "execute" controls.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn, true) // Contains "renounce title" control.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn));
	}
}
