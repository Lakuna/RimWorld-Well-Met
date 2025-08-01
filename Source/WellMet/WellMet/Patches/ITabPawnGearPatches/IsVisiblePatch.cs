using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), nameof(ITab_Pawn_Gear.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Gear __instance, ref bool __result) => __result = __result
			&& (!(SelPawnForGearMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn));
	}
}
