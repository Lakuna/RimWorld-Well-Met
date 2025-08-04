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

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlavePriceListing")]
	internal static class DoSlavePriceListingPatch {
#if V1_0
		private static readonly MethodInfo SelPawnMethod = AccessTools.Method(typeof(ITab), "get_SelPawn");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");
#endif

		[HarmonyPrefix]
		private static bool Prefix(ITab_Pawn_Visitor __instance) =>
#if V1_0
			!(SelPawnMethod.Invoke(__instance, Parameters) is Pawn pawn)
#else
			!(SelPawnMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
