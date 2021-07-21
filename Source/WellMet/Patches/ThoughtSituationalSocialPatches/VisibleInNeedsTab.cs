using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.VisibleInNeedsTab))]
	public class VisibleInNeedsTab {
		[HarmonyPostfix]
		public static void Postfix(Thought_SituationalSocial __instance, ref bool __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = false;
			}
		}
	}
}
