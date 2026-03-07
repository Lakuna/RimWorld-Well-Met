#if !V1_0
using System.Linq;

using HarmonyLib;

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertHitchedAnimalHungryNoFoodPatches {
	[HarmonyPatch(typeof(Alert_HitchedAnimalHungryNoFood), nameof(Alert_HitchedAnimalHungryNoFood.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
#endif
