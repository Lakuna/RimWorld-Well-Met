using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.NeedChemicalAnyPatches {
	[HarmonyPatch(typeof(Need_Chemical_Any), nameof(Need_Chemical_Any.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Need), "pawn");

		[HarmonyPostfix]
		private static void Postfix(Need_Chemical_Any __instance, ref bool __result) => __result = __result
			&& (!(PawnField.GetValue(__instance) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn));
	}
}
