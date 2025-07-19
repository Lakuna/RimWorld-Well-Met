using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.TabPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), nameof(ITab_Pawn_Gear.IsVisible), MethodType.Getter)]
	internal static class GearTabPatch {
		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Gear __instance, ref bool __result) {
			// Don't show the tab if it was already hidden.
			if (!__result) {
				return;
			}

			// If there is no selected pawn, do nothing.
			if (!(AccessTools.DeclaredPropertyGetter(typeof(ITab_Pawn_Gear).FullName + ":SelPawnForGear").Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
				return;
			}

			// Never hide the gear tab for player-controlled pawns because it contains the drop item buttons.
			PawnType type = KnowledgeUtility.TypeOf(pawn);
			if (KnowledgeUtility.IsPlayerControlled(type)) {
				return;
			}

			// Show the gear tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, type); // Mass carried, comfortable temperature range, overall armor (sharp, blunt, and heat), equipment, apparel, and inventory.
		}
	}
}
