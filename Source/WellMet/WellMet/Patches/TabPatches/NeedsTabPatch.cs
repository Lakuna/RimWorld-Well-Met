using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.TabPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Needs), nameof(ITab_Pawn_Needs.IsVisible), MethodType.Getter)]
	internal static class NeedsTabPatch {
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Needs __instance, ref bool __result) {
			// Don't show the tab if it was already hidden.
			if (!__result) {
				return;
			}

			// If there is no selected pawn, do nothing.
			if (!(SelPawnMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
				return;
			}

			// Show the needs tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn); // Food, sleep, beauty, and outdoors needs, psychite, go-juice, luciferium, wake-up, alcohol, smokeleaf, and ambrosia dependencies, mood, and moodlets.
		}
	}
}
