using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPane {
	[HarmonyPatch(typeof(RoyalTitleDef), nameof(RoyalTitleDef.GetLabelFor), new Type[] { typeof(Pawn) })]
	internal static class RoyalTitlePatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn p, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, p)) {
				return true;
			}

			__result = "Unknown".Translate();
			return false;
		}
	}
}
