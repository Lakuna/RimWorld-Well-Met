﻿using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnFormingCaravanPatches {
	[HarmonyPatch(typeof(ITab_Pawn_FormingCaravan), nameof(ITab_Pawn_FormingCaravan.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_FormingCaravan __instance, ref bool __result) => __result = __result
			&& (!(SelPawnMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn));
	}
}
