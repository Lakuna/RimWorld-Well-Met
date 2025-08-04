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

namespace Lakuna.WellMet.Patches.ITabPawnLogPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Log), "FillTab")]
	internal static class FillTabPatch {
#if V1_0
		private static readonly MethodInfo SelPawnForCombatInfoMethod = AccessTools.Method(typeof(ITab_Pawn_Log), "get_SelPawnForCombatInfo");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo SelPawnForCombatInfoMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Log), "SelPawnForCombatInfo");
#endif

		[HarmonyPrefix]
		private static void Prefix(ITab_Pawn_Log __instance, ref bool ___showCombat, ref bool ___showSocial) {
#if V1_0
			if (!(SelPawnForCombatInfoMethod.Invoke(__instance, Parameters) is Pawn pawn)) {
#else
			if (!(SelPawnForCombatInfoMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)) {
#endif
				return;
			}

			___showCombat = ___showCombat && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
			___showSocial = ___showSocial && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
		}
	}
}
