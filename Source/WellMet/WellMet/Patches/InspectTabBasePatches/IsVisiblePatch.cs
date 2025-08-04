#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !V1_0
using System;
#endif
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.InspectTabBasePatches {
	[HarmonyPatch(typeof(InspectTabBase), nameof(InspectTabBase.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
#if V1_0
		private static readonly MethodInfo PawnForHealthMethod = AccessTools.Method(typeof(ITab_Pawn_Health), "get_PawnForHealth");

		private static readonly MethodInfo SelPawnForCombatInfoMethod = AccessTools.Method(typeof(ITab_Pawn_Log), "get_SelPawnForCombatInfo");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo PawnForHealthMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Health), "PawnForHealth");

		private static readonly MethodInfo SelPawnForCombatInfoMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");
#endif

		[HarmonyPostfix]
		private static void Postfix(InspectTabBase __instance, ref bool __result) {
			if (__instance is ITab_Pawn_Health healthTab) {
				__result = __result
#if V1_0
					&& (!(PawnForHealthMethod.Invoke(healthTab, Parameters) is Pawn pawn)
#else
					&& (!(PawnForHealthMethod.Invoke(healthTab, Array.Empty<object>()) is Pawn pawn)
#endif
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn, true));
				return;
			}

			if (__instance is ITab_Pawn_Log logTab) {
				__result = __result
#if V1_0
					&& (!(SelPawnForCombatInfoMethod.Invoke(logTab, Parameters) is Pawn pawn)
#else
					&& (!(SelPawnForCombatInfoMethod.Invoke(logTab, Array.Empty<object>()) is Pawn pawn)
#endif
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn));
			}
		}
	}
}
