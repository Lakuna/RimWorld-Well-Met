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
		private static void Postfix(ITab_Pawn_Social __instance, ref bool __result) => __result = __result
			&& (!(SelPawnForSocialInfoMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn));
	}
}
