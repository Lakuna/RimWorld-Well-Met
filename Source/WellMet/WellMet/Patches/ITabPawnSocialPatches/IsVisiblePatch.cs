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

namespace Lakuna.WellMet.Patches.ITabPawnSocialPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Social), nameof(ITab_Pawn_Social.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
#if V1_0
		private static readonly MethodInfo SelPawnForSocialInfoMethod = AccessTools.Method(typeof(ITab_Pawn_Social), "get_SelPawnForSocialInfo");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo SelPawnForSocialInfoMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Social), "SelPawnForSocialInfo");
#endif

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Social __instance, ref bool __result) => __result = __result
#if V1_0
			&& (!(SelPawnForSocialInfoMethod.Invoke(__instance, Parameters) is Pawn pawn)
#else
			&& (!(SelPawnForSocialInfoMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn, true) // "Romance" button.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn, true)); // "Assign role" button.
	}
}
