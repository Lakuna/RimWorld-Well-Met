using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.ThoughtMemorySocialPatches {
	[HarmonyPatch(typeof(Thought_MemorySocial), nameof(Thought_MemorySocial.VisibleInNeedsTab), MethodType.Getter)]
	internal static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
		private static void Postfix(Thought_MemorySocial __instance, ref bool __result) => __result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance.pawn);
	}
}
