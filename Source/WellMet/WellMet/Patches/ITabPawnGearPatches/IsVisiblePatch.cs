using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), nameof(ITab_Pawn_Gear.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Gear __instance, ref bool __result) {
			// Don't modify the tab if it was already hidden or if there is no selected pawn.
			if (!__result || !(SelPawnForGearMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
				return;
			}

			// Show the gear tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn); // Mass carried, comfortable temperature range, overall armor (sharp, blunt, and heat), equipment, apparel, and inventory.
		}
	}
}
