using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.VisibleInNeedsTab))]
	public class VisibleInNeedsTab {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref bool __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef) && WellMet.TraitDiscovered(__instance.pawn.story.traits.GetTrait(traitDef)))) {
				__result = false;
			}
		}
	}
}
