using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.Tabs {
	[HarmonyPatch(typeof(ITab_Pawn_Social), nameof(ITab_Pawn_Social.IsVisible), MethodType.Getter)]
	internal static class SocialTabPatch {
		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Social __instance, ref bool __result) {
			// Don't show the tab if it was already hidden.
			if (!__result) {
				return;
			}

			// If there is no selected pawn, do nothing.
			if (!(AccessTools.DeclaredPropertyGetter(typeof(ITab_Pawn_Social).FullName + ":SelPawnForSocialInfo").Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
				return;
			}

			// Show the social tab only if any of the information on the tab is supposed to be shown.
			__result = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, KnowledgeUtility.TypeOf(pawn)) // Relations and social log.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, KnowledgeUtility.TypeOf(pawn)); // Ideology and ideology role.
		}
	}
}
