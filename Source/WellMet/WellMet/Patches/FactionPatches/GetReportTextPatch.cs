using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.FactionPatches {
	[HarmonyPatch(typeof(Faction), nameof(Faction.GetReportText), MethodType.Getter)]
	internal static class GetReportTextPatch {
		[HarmonyPostfix]
		public static void Postfix(Faction __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst();
		}
	}
}
