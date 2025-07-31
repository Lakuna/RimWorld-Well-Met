using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.FactionPatches {
	[HarmonyPatch(typeof(Faction), nameof(Faction.NameColored), MethodType.Getter)]
	internal static class NameColoredPatch {
		[HarmonyPostfix]
		public static void Postfix(Faction __instance, ref TaggedString __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst();
		}
	}
}
