using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.FactionPatches {
	[HarmonyPatch(typeof(Faction), nameof(Faction.Hidden), MethodType.Getter)]
	internal static class HiddenPatch {
		[HarmonyPostfix]
		public static void Postfix(Faction __instance, ref bool __result) => __result = __result || !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
	}
}
