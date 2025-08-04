using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.LabelShort), MethodType.Getter)]
	internal static class LabelShortPatch {
		private static readonly MethodInfo NameMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Name));

		private static readonly MethodInfo LabelPrefixMethod = AccessTools.PropertyGetter(typeof(Pawn), "LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(NameMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(LabelPrefixMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, "")) {
						yield return i;
					}
				}
			}
		}
	}
}
