using HarmonyLib;
using RimWorld;
using System.Collections.Generic;

namespace WellMet.Patches.PawnRelationTrackerPatches {
	[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.OpinionExplanation))]
	public class OpinionExplanation {
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;
			}
		}
	}
}
