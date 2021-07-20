using HarmonyLib;
using RimWorld;

namespace WellMet.Patches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.ToString))]
	public class TraitToStringPatch {
		[HarmonyPostfix]
		public static void Postfix(Trait __instance, ref string __result) {
			if (!WellMet.TraitDiscovered(__instance)) {
				__result = WellMet.UnknownTraitLabel;
			}
		}
	}
}
