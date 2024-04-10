#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utilities;
using RimWorld;

namespace Lakuna.WellMet.Patches.ThoughtPatches.VisibleInNeedsTabPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.VisibleInNeedsTab))]
	public static class ThoughtSituationalSocialVisibleInNeedsTabPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref bool __result) {
#pragma warning restore CA1707
			if (!ThoughtUtilities.ThoughtIsDiscovered(__instance)) {
				__result = false;
			}
		}
	}
}
