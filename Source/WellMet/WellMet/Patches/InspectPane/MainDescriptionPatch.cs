using HarmonyLib;
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPane {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.MainDesc))]
	internal static class MainDescriptionPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance)) {
				return true;
			}

			__result = "Unknown".Translate().CapitalizeFirst(); // TODO: Instead just don't even add this line. Must be done from `Pawn.GetInspectString` or `InspectPaneFiller.DrawInspectStringFor` to avoid the "inspect string for foo contains empty lines" error.
			return false;
		}
	}
}
