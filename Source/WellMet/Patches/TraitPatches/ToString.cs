using HarmonyLib;
using RimWorld;

namespace WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.ToString))]
	public class ToString {
		[HarmonyPostfix]
		public static void Postfix(Trait __instance, ref string __result) {
			if (!WellMet.TraitDiscovered(__instance)) {
				__result = WellMet.UnknownTraitName;
			}
		}
	}
}
