using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), nameof(Thought.ToString))]
	public class ToString {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref string __result) {
			if (WellMet.ThoughtIsHiddenForPawn(__instance.pawn, __instance.def)) {
				__result = WellMet.UnknownThoughtName;
			}
		}
	}
}
