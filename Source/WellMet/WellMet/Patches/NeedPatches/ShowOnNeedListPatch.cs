using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.NeedPatches {
	[HarmonyPatch(typeof(Need), nameof(Need.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Need), "pawn");

		[HarmonyPostfix]
		private static void Postfix(Need __instance, ref bool __result) {
			if (__instance is Need_Chemical needChemical) {
				__result = __result && (!(PawnField.GetValue(needChemical) is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn));
				return;
			}

			if (__instance is Need_Deathrest needDeathrest) {
				__result = __result && (!(PawnField.GetValue(needDeathrest) is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn));
			}
		}
	}
}
