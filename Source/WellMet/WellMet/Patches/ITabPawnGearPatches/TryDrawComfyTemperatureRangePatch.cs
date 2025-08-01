using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "TryDrawComfyTemperatureRange")]
	internal static class TryDrawComfyTemperatureRangePatch {
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPostfix]
		private static bool Prefix(ITab_Pawn_Gear __instance) => !(SelPawnForGearMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
