#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;

using Lakuna.WellMet.Utility;

using RimWorld;

using System;
using System.Reflection;

using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlavePriceListing")]
	internal static class DoSlavePriceListingPatch {
		private static readonly MethodInfo SelPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPrefix]
#pragma warning disable CA1707
		private static bool Prefix(ITab_Pawn_Visitor __instance) =>
#pragma warning restore CA1707
			!(SelPawnMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
	}
}
#endif
