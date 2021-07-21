using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), "get_" + nameof(Trait.LabelCap))]
	public class LabelCap {
		[HarmonyPostfix]
		public static void Postfix(Trait __instance, ref string __result) {
			if (!WellMet.TraitDiscovered(__instance)) {
				__result = WellMet.UnknownTraitName;
			}

			// This is the string which is displayed in a pawn's trait list.
		}
	}
}
