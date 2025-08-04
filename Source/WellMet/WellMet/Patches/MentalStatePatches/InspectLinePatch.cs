#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using Verse;
using Verse.AI;

namespace Lakuna.WellMet.Patches.MentalStatePatches {
	[HarmonyPatch(typeof(MentalState), nameof(MentalState.InspectLine), MethodType.Getter)]
	internal static class InspectLinePatch {
		[HarmonyPostfix]
		private static void Postfix(MentalState __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.pawn)) {
				return;
			}

			__result = "Unknown".Translate();
		}
	}
}
