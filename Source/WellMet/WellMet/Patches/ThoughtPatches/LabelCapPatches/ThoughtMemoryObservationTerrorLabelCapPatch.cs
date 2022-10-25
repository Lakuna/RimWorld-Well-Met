﻿#if V1_0
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
using Verse;

namespace Lakuna.WellMet.Patches.ThoughtPatches.LabelCapPatches {
#if !(V1_0 || V1_1 || V1_2)
	[HarmonyPatch(typeof(Thought_MemoryObservationTerror), "get_" + nameof(Thought_MemoryObservationTerror.LabelCap))]
	public static class ThoughtMemoryObservationTerrorLabelCapPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref string __result) {
#pragma warning restore CA1707
			if (!ThoughtUtilities.ThoughtIsDiscovered(__instance)) {
				__result = "UnknownThought".Translate().CapitalizeFirst();
			}
		}
	}
#endif
}
