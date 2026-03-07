#if !(V1_0 || V1_1)
using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.AlertCriticalPatches {
	[HarmonyPatch(typeof(Alert_Critical), "DoMessage", MethodType.Getter)]
	internal static class DoMessagePatch {
		[HarmonyPostfix]
		private static void Postfix(Alert_Critical __instance, ref bool __result) {
			// Never enable an already disabled message.
			if (!__result) {
				return;
			}

			List<Pawn> pawns =
#if V1_0
				__instance.GetReport().culprits.Select((target) => target.Thing as Pawn).Where((pawn) => !(pawn is null)).ToList();
#else
				__instance.GetReport().culpritsPawns;
#endif
			if (pawns.NullOrEmpty()) {
				return;
			}

			InformationCategory category =
				(__instance is Alert_MajorOrExtremeBreakRisk
#if !(V1_0 || V1_1)
				|| __instance is Alert_MeatHunger
#endif
				)
					? InformationCategory.Needs
					: (__instance is Alert_Hypothermia
					|| __instance is Alert_ColonistNeedsRescuing
					|| __instance is Alert_LifeThreateningHediff
					|| __instance is Alert_ImmobileCaravan
#if !(V1_0 || V1_1)
					|| __instance is Alert_GhoulHypothermia
					|| __instance is Alert_LowOxygen
#endif
					)
						? InformationCategory.Health
						: InformationCategory.Basic;
			if (pawns.All((pawn) => !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn))) {
				__result = false;
			}
		}
	}
}
#endif
