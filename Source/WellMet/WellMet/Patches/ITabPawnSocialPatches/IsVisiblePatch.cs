using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnSocialPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Social), nameof(ITab_Pawn_Social.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnForSocialInfoMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Social), "SelPawnForSocialInfo");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Social __instance, ref bool __result) {
			// Don't modify the tab if it was already hidden or if there is no selected pawn.
			if (!__result || !(SelPawnForSocialInfoMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
				return;
			}

			// Show the social tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn) // Relations and social log.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn); // Ideology and ideology role.
		}
	}
}
