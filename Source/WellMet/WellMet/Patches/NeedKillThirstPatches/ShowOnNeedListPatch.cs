#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.WellMet.Utility;

using RimWorld;

using Verse;

namespace Lakuna.WellMet.Patches.NeedKillThirstPatches {
	[HarmonyPatch(typeof(Need_KillThirst), nameof(Need_KillThirst.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn ___pawn, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, ___pawn);
	}
}
#endif
