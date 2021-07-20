using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.Description))]
	public class Description {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref string __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef))) {
				__result = WellMet.UnknownThoughtDescription;
			}
		}
	}
}
