using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	public class TipString {
		[HarmonyPostfix]
		public static void Postfix(Trait __instance, ref string __result) {
			if (!WellMet.TraitIsDiscoveredForPawn(__instance)) {
				__result = WellMet.UnknownTraitDescription;
			}

			// This is the string which is displayed in tooltips on a pawn's trait list.
		}
	}
}
