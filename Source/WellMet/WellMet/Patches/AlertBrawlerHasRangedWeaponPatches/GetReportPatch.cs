using System.Linq;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertBrawlerHasRangedWeaponPatches {
	[HarmonyPatch(typeof(Alert_BrawlerHasRangedWeapon), nameof(Alert_BrawlerHasRangedWeapon.GetReport))]
	internal static class GetReportPatch {
		[HarmonyPostfix]
		private static void Postfix(ref AlertReport __result) {
			if (WellMetMod.Settings.NeverHideAlerts) {
				return;
			}

			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) =>
				KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, ControlCategory.Alert)
				&& KnowledgeUtility.IsTraitKnown(pawn.story.traits.GetTrait(TraitDefOf.Brawler))).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
