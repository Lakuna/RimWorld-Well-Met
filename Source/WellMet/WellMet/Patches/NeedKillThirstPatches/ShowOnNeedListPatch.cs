using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.NeedKillThirstPatches {
	[HarmonyPatch(typeof(Need_KillThirst), nameof(Need_KillThirst.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Need), "pawn");

		[HarmonyPostfix]
		private static void Postfix(Need_KillThirst __instance, ref bool __result) => __result = __result
			&& (!(PawnField.GetValue(__instance) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn));
	}
}
