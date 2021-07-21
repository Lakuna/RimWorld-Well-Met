using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtSituationalPatches {
	[HarmonyPatch(typeof(Thought_Situational), "get_" + nameof(Thought_Situational.LabelCap))]
	public class LabelCap {
		[HarmonyPostfix]
		public static void Postfix(Thought_Situational __instance, ref string __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef) && WellMet.TraitDiscovered(__instance.pawn.story.traits.GetTrait(traitDef)))) {
				__result = WellMet.UnknownThoughtName;
			}

			// This is the string which is displayed in a pawn's needs list.
		}
	}
}
