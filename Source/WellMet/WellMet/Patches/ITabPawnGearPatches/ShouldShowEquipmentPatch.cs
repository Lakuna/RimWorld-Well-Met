using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "ShouldShowEquipment")]
	internal static class ShouldShowEquipmentPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn p, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, p, true);
	}
}
