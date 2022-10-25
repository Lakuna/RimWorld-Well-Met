#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lakuna.WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.VisibleInNeedsTab))]
	[HarmonyPatch(typeof(Thought_Memory), "get_" + nameof(Thought_Memory.VisibleInNeedsTab))]
	[HarmonyPatch(typeof(Thought_MemorySocial), "get_" + nameof(Thought_MemorySocial.VisibleInNeedsTab))]
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.VisibleInNeedsTab))]
	public static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref bool __result) {
#pragma warning restore CA1707
			if (ThoughtUtilities.ThoughtIsHidden(__instance)) {
				__result = false;
			}
		}
	}
}
