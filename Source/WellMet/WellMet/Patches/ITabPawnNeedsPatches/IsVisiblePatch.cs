﻿using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnNeedsPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Needs), nameof(ITab_Pawn_Needs.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Needs __instance, ref bool __result) => __result = __result
			&& (!(SelPawnMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn));
	}
}
