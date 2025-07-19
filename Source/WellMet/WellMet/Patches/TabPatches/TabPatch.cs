using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.TabPatches {
	[HarmonyPatch(typeof(InspectTabBase), nameof(InspectTabBase.IsVisible), MethodType.Getter)]
	internal static class TabPatch {
		[HarmonyPostfix]
		private static void Postfix(InspectTabBase __instance, ref bool __result) {
			// Don't show the tab if it was already hidden.
			if (!__result) {
				return;
			}

			// Health tab.
			if (__instance is ITab_Pawn_Health healthTab) {
				// If there is no selected pawn, do nothing.
				if (!(AccessTools.DeclaredPropertyGetter(typeof(ITab_Pawn_Health).FullName + ":PawnForHealth").Invoke(healthTab, Array.Empty<object>()) is Pawn pawn)) {
					return;
				}

				// Never hide the health tab for player-controlled pawns and prisoners because it contains the allow food and medicine dropdowns, self-tend toggle, and operations menu.
				PawnType type = KnowledgeUtility.TypeOf(pawn);
				if (KnowledgeUtility.IsPlayerControlled(type) || type == PawnType.Prisoner) {
					return;
				}

				// Show the health tab only if any of the information on the tab is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, type); // Statistics and hediffs.
				return;
			}

			// Log tab.
			if (__instance is ITab_Pawn_Log logTab) {
				// If there is no selected pawn, do nothing.
				if (!(AccessTools.DeclaredPropertyGetter(typeof(ITab_Pawn_Log).FullName + ":SelPawnForCombatInfo").Invoke(logTab, Array.Empty<object>()) is Pawn pawn)) {
					return;
				}

				// Show the log tab only if any of the information on the tab is supposed to be shown.
				__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn); // Log.
			}
		}
	}
}
