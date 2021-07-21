using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.LabelCap))]
	public class LabelCap {
		[HarmonyPostfix]
		public static void Postfix(Thought_SituationalSocial __instance, ref string __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef) && WellMet.TraitDiscovered(__instance.pawn.story.traits.GetTrait(traitDef)))) {
				__result = WellMet.UnknownThoughtName;
			}

			// TODO: Also check for traits of the other pawn.
		}
	}
}
