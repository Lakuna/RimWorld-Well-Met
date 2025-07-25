using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.InspectTabBasePatches {
	[HarmonyPatch(typeof(InspectTabBase), nameof(InspectTabBase.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo PawnForHealthMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Health), "PawnForHealth");

		private static readonly MethodInfo SelPawnForCombatInfoMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");

		[HarmonyPostfix]
		private static void Postfix(InspectTabBase __instance, ref bool __result) {
			// Don't show the tab if it was already hidden.
			if (!__result) {
				return;
			}

			// Health tab.
			if (__instance is ITab_Pawn_Health healthTab) {
				// If there is no selected pawn, do nothing.
				if (!(PawnForHealthMethod.Invoke(healthTab, Array.Empty<object>()) is Pawn pawn)) {
					return;
				}

				// Show the health tab only if any of the information on the tab is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn); // Statistics and hediffs.
				return;
			}

			// Log tab.
			if (__instance is ITab_Pawn_Log logTab) {
				// If there is no selected pawn, do nothing.
				if (!(SelPawnForCombatInfoMethod.Invoke(logTab, Array.Empty<object>()) is Pawn pawn)) {
					return;
				}

				// Show the log tab only if any of the information on the tab is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn); // Log.
			}
		}
	}
}
