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
	[HarmonyPatch(typeof(Thought), nameof(Thought.ToString))]
	[HarmonyPatch(typeof(Thought_Memory), nameof(Thought_Memory.ToString))]
	public static class ToStringPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref string __result) {
#pragma warning restore CA1707
			if (ThoughtUtilities.ThoughtIsHidden(__instance)) {
				__result = ThoughtUtilities.UnknownThoughtName;
			}
		}
	}
}
