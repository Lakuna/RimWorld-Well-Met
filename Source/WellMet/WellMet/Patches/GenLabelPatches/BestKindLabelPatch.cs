using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.GenLabelPatches {
	[HarmonyPatch(typeof(GenLabel), nameof(GenLabel.BestKindLabel), new Type[] { typeof(Pawn), typeof(bool), typeof(bool), typeof(bool), typeof(int) })]
	internal static class BestKindLabelPatch {
		[HarmonyPrefix]
		private static void Prefix(Pawn pawn, ref bool mustNoteGender, ref bool mustNoteLifeStage) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
			mustNoteGender = mustNoteGender && basic;
			mustNoteLifeStage = mustNoteLifeStage && basic;
		}

		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				|| pawn.RaceProps.Animal
#else
				|| pawn.IsAnimal
#endif
			) {
				return;
			}

			// If this isn't done, NPC humans with basic information not known will show their kind (i.e. "mercenary gunner").
			__result = "Unknown".Translate();
		}
	}
}
