using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlavePriceListing")]
	internal static class DoSlavePriceListingPatch {
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPrefix]
		private static bool Prefix(ITab_Pawn_Visitor __instance) => !(SelPawnMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
