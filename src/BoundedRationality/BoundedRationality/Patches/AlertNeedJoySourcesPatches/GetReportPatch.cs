#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertNeedJoySourcesPatches {
	[HarmonyPatch(typeof(Alert_NeedJoySources), nameof(Alert_NeedJoySources.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) => __result = __result.active && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, PawnType.Colonist, ControlCategory.Alert);
	}
}
