using HarmonyLib;
using RimWorld;
using System.Linq;

namespace WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.VisibleInNeedsTab))]
	public class VisibleInNeedsTab {
		[HarmonyPostfix]
		public static void Postfix(Thought __instance, ref bool __result) {
			if (__instance.def.requiredTraits != null && __instance.def.requiredTraits.All((traitDef) => __instance.pawn.story.traits.HasTrait(traitDef))) {
				__result = false;
			}

			foreach (PawnRelationDef relationDef in __instance.pawn.GetRelations(__instance.pawn /* OTHER PAWN HERE */)) {

			}
		}
	}
}
