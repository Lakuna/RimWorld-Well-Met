﻿#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utilities;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.ThoughtPatches.LabelCapPatches {
	[HarmonyPatch(typeof(Thought_Situational), "get_" + nameof(Thought_Situational.LabelCap))]
	public static class ThoughtSituationalLabelCapPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref string __result) {
#pragma warning restore CA1707
			if (!ThoughtUtilities.ThoughtIsDiscovered(__instance)) {
				__result = "UnknownThought".Translate().CapitalizeFirst();
			}
		}
	}
}
