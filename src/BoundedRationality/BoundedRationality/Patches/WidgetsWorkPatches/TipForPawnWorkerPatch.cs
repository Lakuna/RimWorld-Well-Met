#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsWorkPatches {
	[HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.TipForPawnWorker))]
	internal static class TipForPawnWorkerPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn p, WorkTypeDef wDef, ref string __result) {
			if (KnowledgeUtility.AreAllSkillsForWorkTypeKnown(p, wDef)) {
				return;
			}

			__result = MiscellaneousUtility.EndWithPeriod("BR.Unknown".Translate().CapitalizeFirst());
		}
	}
}
