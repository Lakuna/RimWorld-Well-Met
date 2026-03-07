#if !V1_0
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.AlertSlaveRebellionLikelyPatches {
	[HarmonyPatch(typeof(Alert_SlaveRebellionLikely), nameof(Alert_SlaveRebellionLikely.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, __result.culpritTarget?.Pawn, ControlCategory.Alert)) {
				return;
			}

			__result.active = false;
			__result.culpritTarget = default;
		}
	}
}
#endif
