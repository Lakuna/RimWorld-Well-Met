using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnCharacterPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Character), nameof(ITab_Pawn_Character.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo PawnToShowInfoAboutMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Character), "PawnToShowInfoAbout");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Character __instance, ref bool __result) => __result = __result
			&& (!(PawnToShowInfoAboutMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, true) // Contains "rename," "banish," and "execute" controls.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn, true) // Contains "renounce title" control.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn));
	}
}
