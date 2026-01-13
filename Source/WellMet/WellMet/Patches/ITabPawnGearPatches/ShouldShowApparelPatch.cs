#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "ShouldShowApparel")]
	internal static class ShouldShowApparelPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn p, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, p, ControlCategory.Control);
	}
}
