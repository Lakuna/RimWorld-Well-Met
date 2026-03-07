using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertMechDamagedPatches {
	[HarmonyPatch(typeof(Alert_MechDamaged), nameof(Alert_MechDamaged.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
