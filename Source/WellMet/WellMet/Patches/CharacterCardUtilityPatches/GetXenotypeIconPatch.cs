using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetXenotypeIcon")]
	internal static class GetXenotypeIconPatch {
		private static readonly MethodInfo StartingAndOptionalPawnsMethod = AccessTools.PropertyGetter(typeof(StartingPawnUtility), "StartingAndOptionalPawns");

		[HarmonyPostfix]
		public static void Postfix(int startingPawnIndex, ref Texture2D __result) {
			object startingAndOptionalPawnsObject = StartingAndOptionalPawnsMethod.Invoke(null, Array.Empty<object>());
			if (startingAndOptionalPawnsObject is List<Pawn> startingAndOptionalPawns && startingAndOptionalPawns[startingPawnIndex] is Pawn pawn && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn)) {
				return;
			}

			__result = null;
		}
	}
}
