using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.Tabs {
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

			// Never hide the biography tab for player-controlled pawns because it contains the renounce title, rename colonist, and banish buttons.
			PawnType type = KnowledgeUtility.TypeOf(pawn);
			if (KnowledgeUtility.IsPlayerControlled(type)) {
				return;
			}

			// Show the biography tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, type) // Name (first, nickname, and last), gender, age (biological and chronological), faction, home faction, and title.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, type) // Ideology.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, type) // Xenotype, royal title (honor), favorite color, and unwaveringly loyal.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, type) // Childhood and adulthood.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, type) // Traits.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, type) // Incapabilities, skills, and passions.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, type); // Abilities.
		}
	}
}
