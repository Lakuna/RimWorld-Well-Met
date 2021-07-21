using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.LabelCap))]
	public class LabelCap {
		[HarmonyPostfix]
		public static void Postfix(Thought_SituationalSocial __instance, ref string __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def) || WellMet.ThoughtIsHiddenForPawn(__instance.otherPawn, __instance.def)) {
				__result = WellMet.UnknownThoughtName;
			}
		}
	}
}
