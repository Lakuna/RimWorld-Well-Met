using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.Description))]
	public class Description {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref string __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = WellMet.UnknownThoughtDescription;
			}
		}
	}
}
