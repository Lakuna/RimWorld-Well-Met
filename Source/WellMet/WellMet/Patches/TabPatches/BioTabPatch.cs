using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.TabPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Character), nameof(ITab_Pawn_Character.IsVisible), MethodType.Getter)]
	internal static class BioTabPatch {
		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Character __instance, ref bool __result) {
			// Don't show the tab if it was already hidden.
			if (!__result) {
				return;
			}

			// If there is no selected pawn, do nothing.
			if (!(AccessTools.DeclaredPropertyGetter(typeof(ITab_Pawn_Character).FullName + ":PawnToShowInfoAbout").Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
				return;
			}

			// Show the biography tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn) // Name (first, nickname, and last), gender, age (biological and chronological), faction, home faction, and title.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn) // Ideology.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn) // Xenotype, royal title (honor), favorite color, and unwaveringly loyal.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn) // Childhood and adulthood.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn) // Traits.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn) // Incapabilities, skills, and passions.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn); // Abilities.
		}
	}
}
