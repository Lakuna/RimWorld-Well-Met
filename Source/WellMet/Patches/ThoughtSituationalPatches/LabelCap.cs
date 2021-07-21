using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtSituationalPatches {
	[HarmonyPatch(typeof(Thought_Situational), "get_" + nameof(Thought_Situational.LabelCap))]
	public class LabelCap {
		[HarmonyPostfix]
		public static void Postfix(Thought_Situational __instance, ref string __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = WellMet.UnknownThoughtName;
			}

			// This is the string which is displayed in a pawn's needs list.
		}
	}
}
