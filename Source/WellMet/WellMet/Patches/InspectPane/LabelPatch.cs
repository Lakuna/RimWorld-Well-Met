using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPane {
	[HarmonyPatch(typeof(InspectPaneUtility), nameof(InspectPaneUtility.AdjustedLabelFor))]
	internal static class LabelPatch {
		[HarmonyPostfix]
		private static void Postfix(List<object> selected, ref string __result) {
			if (selected.Count != 1 || !(selected[0] is Pawn pawn)) {
				return;
			}

			PawnType type = KnowledgeUtility.TypeOf(pawn);
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, type)) {
				return;
			}

			__result = (type.ToString() + "Pawn").Translate().CapitalizeFirst();
		}
	}
}
