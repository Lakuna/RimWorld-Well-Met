using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertHunterHasShieldAndRangedWeaponPatches {
	[HarmonyPatch(typeof(Alert_HunterHasShieldAndRangedWeapon), nameof(Alert_HunterHasShieldAndRangedWeapon.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, ControlCategory.Alert)).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
