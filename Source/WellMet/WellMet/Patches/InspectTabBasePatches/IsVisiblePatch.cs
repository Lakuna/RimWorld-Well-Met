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
			if (__instance is ITab_Pawn_Health healthTab) {
				__result = __result
					&& (!(PawnForHealthMethod.Invoke(healthTab, Array.Empty<object>()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, true));
				return;
			}

			if (__instance is ITab_Pawn_Log logTab) {
				__result = __result
					&& (!(SelPawnForCombatInfoMethod.Invoke(logTab, Array.Empty<object>()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn));
			}
		}
	}
}
