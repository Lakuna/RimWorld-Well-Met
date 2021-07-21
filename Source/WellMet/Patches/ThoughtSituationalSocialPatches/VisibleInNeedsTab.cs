using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.VisibleInNeedsTab))]
	public class VisibleInNeedsTab {
		[HarmonyPostfix]
		public static void Postfix(Thought_SituationalSocial __instance, ref bool __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef) && WellMet.TraitDiscovered(__instance.pawn.story.traits.GetTrait(traitDef)))) {
				__result = false;
			}

			// TODO: Also check for traits of the other pawn.
		}
	}
}
