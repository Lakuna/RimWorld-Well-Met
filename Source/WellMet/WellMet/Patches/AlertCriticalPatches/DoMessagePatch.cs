#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

using System.Collections.Generic;
using System.Linq;

using Verse;

namespace Lakuna.WellMet.Patches.AlertCriticalPatches {
	[HarmonyPatch(typeof(Alert_Critical), "DoMessage", MethodType.Getter)]
	internal static class DoMessagePatch {
		[HarmonyPostfix]
		private static void Postfix(Alert_Critical __instance, ref bool __result) {
			// Never enable an already disabled message.
			if (!__result) {
				return;
			}

			List<Pawn> pawns = __instance.GetReport().culpritsPawns;
			if (pawns.Count < 1) {
				return;
			}

			InformationCategory category =
				(__instance is Alert_MajorOrExtremeBreakRisk
				|| __instance is Alert_MeatHunger)
					? InformationCategory.Needs
					: (__instance is Alert_Hypothermia
					|| __instance is Alert_ColonistNeedsRescuing
					|| __instance is Alert_LifeThreateningHediff
					|| __instance is Alert_ImmobileCaravan
					|| __instance is Alert_GhoulHypothermia
					|| __instance is Alert_LowOxygen)
						? InformationCategory.Health
						: InformationCategory.Basic;
			if (pawns.All((pawn) => !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn))) {
				__result = false;
			}
		}
	}
}
