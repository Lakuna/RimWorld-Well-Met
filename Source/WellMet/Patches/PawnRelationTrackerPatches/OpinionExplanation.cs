using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace WellMet.Patches.PawnRelationTrackerPatches {
	[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.OpinionExplanation))]
	public class OpinionExplanation {
		private static readonly CodeInstruction GetGenderSpecificLabelCapInstruction = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PawnRelationDef), nameof(PawnRelationDef.GetGenderSpecificLabelCap)));
		private static readonly CodeInstruction LoadArgumentThisInstruction = new CodeInstruction(OpCodes.Ldarg_0);
		private static readonly CodeInstruction GetRelationLabelInstruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OpinionExplanation), nameof(OpinionExplanation.GetRelationLabel)));

		private static string GetRelationLabel(PawnRelationDef relation, Pawn other, Pawn_RelationsTracker instance) => "TEST STRING"; // TODO

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (CodeInstruction instruction in instructions) {
				if (instruction.opcode == OpCodes.Callvirt && instruction.operand == GetGenderSpecificLabelCapInstruction.operand) {
					yield return LoadArgumentThisInstruction; // Load this onto stack.
															  // Stack: StringBuilder stringBuilder, PawnRelationDef relation, Pawn other, Pawn_RelationsTracker this.
					yield return GetRelationLabelInstruction; // Call OpinionExplanation.GetRelationLabel(relation, other, this) to replace the call to relation.GetGenderSpecificLabelCap(other).
															  // Stack: StringBuilder stringBuilder

					// THIS CURRENTLY PATCHES THE WRONG SECTION (relationships i.e. "son"), BUT IT WORKS OTHERWISE.
				} else {
					yield return instruction;
				}
			}
		}
	}
}
