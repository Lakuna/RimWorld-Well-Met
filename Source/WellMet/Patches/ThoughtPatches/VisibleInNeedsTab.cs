using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.VisibleInNeedsTab))]
	public class VisibleInNeedsTab {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref bool __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = false;
			}
		}
	}
}
