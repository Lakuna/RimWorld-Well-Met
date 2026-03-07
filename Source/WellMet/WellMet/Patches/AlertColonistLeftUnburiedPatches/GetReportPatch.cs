using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertColonistLeftUnburiedPatches {
	[HarmonyPatch(typeof(Alert_ColonistLeftUnburied), nameof(Alert_ColonistLeftUnburied.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			__result.culpritsThings = __result.culpritsThings?.Where((thing) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, thing, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
