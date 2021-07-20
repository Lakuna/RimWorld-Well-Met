using HarmonyLib;
using RimWorld;

namespace WellMet.Patches {
	[HarmonyPatch(typeof(Trait), "get_LabelCap")]
	public class TraitLabelCapPatch {
		[HarmonyPostfix]
		public static void Postfix(Trait __instance, ref string __result) {
			if (!WellMet.TraitDiscovered(__instance)) {
				__result = WellMet.UnknownTraitLabel;
			}
		}
	}
}
