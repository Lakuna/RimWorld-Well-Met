using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), "get_" + nameof(Trait.Label))]
	public class Label {
		[HarmonyPostfix]
		public static void Postfix(Trait __instance, ref string __result) {
			if (!WellMet.TraitDiscovered(__instance)) {
				__result = WellMet.UnknownTraitName;
			}
		}
	}
}
