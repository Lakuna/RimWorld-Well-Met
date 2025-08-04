#if !V1_0
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.NeedChemicalAnyPatches {
	[HarmonyPatch(typeof(Need_Chemical_Any), nameof(Need_Chemical_Any.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn ___pawn, ref bool __result) => __result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, ___pawn);
	}
}
#endif
