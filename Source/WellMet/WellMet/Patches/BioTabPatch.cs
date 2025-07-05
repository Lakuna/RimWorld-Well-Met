using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches {
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

			// Never hide the biography tab for colonists or slaves because it contains the renounce title, rename colonist, and banish buttons.
			PawnType type = KnowledgeUtility.TypeOf(pawn);
			if (type == PawnType.Colonist || type == PawnType.Slave) {
				return;
			}

			// Show the biography tab if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, type) // Name (first, nickname, last), gender, age (biological, chronological), xenotype, faction, home faction, and title.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, type) // Ideology.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, type) // Royal title (honor) and favorite color.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, type) // Unwaveringly loyal.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, type) // Childhood and adulthood.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, type) // Traits.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, type) // Incapabilities, skills, and passions.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, type); // Abilities.
		}
	}
}
