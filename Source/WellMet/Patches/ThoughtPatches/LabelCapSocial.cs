using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.LabelCapSocial))]
	public class LabelCapSocial {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref string __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = WellMet.UnknownThoughtName;
			}

			// This is the string which is displayed in a pawn's social opinions list.
		}
	}
}
