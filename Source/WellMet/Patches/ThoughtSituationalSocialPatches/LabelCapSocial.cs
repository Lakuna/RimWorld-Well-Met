using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtSituationalSocialPatches {
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.LabelCapSocial))]
	public class LabelCapSocial {
		[HarmonyPostfix]
		public static void Postfix(Thought_SituationalSocial __instance, ref string __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef) && WellMet.TraitDiscovered(__instance.pawn.story.traits.GetTrait(traitDef)))) {
				__result = WellMet.UnknownThoughtName;
			}

			// This is the string which is displayed in a pawn's social opinions list.

			// TODO: Also check for traits of the other pawn.
		}
	}
}
