#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertIdeoBuildingDisrespectedPatches {
	[HarmonyPatch(typeof(Alert_IdeoBuildingDisrespected), nameof(Alert_IdeoBuildingDisrespected.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) => __result = __result.active && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, PawnType.Colonist, ControlCategory.Alert);
	}
}
