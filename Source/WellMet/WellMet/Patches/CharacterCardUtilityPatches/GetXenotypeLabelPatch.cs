using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetXenotypeLabel")]
	internal static class GetXenotypeLabelPatch {
		private static readonly MethodInfo StartingAndOptionalPawnsMethod = AccessTools.PropertyGetter(typeof(StartingPawnUtility), "StartingAndOptionalPawns");

		[HarmonyPostfix]
		public static void Postfix(int startingPawnIndex, ref string __result) {
			object startingAndOptionalPawnsObject = StartingAndOptionalPawnsMethod.Invoke(null, Array.Empty<object>());
			if (startingAndOptionalPawnsObject is List<Pawn> startingAndOptionalPawns && startingAndOptionalPawns[startingPawnIndex] is Pawn pawn && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn)) {
				return;
			}

			__result = "";
		}
	}
}
