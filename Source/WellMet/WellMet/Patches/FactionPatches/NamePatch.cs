using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.FactionPatches {
	[HarmonyPatch(typeof(Faction), nameof(Faction.Name), MethodType.Getter)]
	internal static class NamePatch {
		[HarmonyPostfix]
		public static void Postfix(Faction __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance)) {
				return;
			}

			__result = "";
		}
	}
}
