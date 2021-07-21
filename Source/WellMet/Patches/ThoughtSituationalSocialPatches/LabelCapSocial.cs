using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.LabelCapSocial))]
	public class LabelCapSocial {
		[HarmonyPostfix]
		public static void Postfix(Thought_SituationalSocial __instance, ref string __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = WellMet.UnknownThoughtName;
			}

			// This is the string which is displayed in a pawn's social opinions list.
		}
	}
}
