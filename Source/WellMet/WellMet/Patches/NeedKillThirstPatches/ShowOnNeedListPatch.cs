#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.NeedKillThirstPatches {
	[HarmonyPatch(typeof(Need_KillThirst), nameof(Need_KillThirst.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn ___pawn, ref bool __result) => __result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, ___pawn);
	}
}
#endif
