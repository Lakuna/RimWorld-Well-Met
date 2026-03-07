#if !V1_0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.LabelNoCountColored), MethodType.Getter)]
	internal static class LabelNoCountColoredPatch {
		private static readonly MethodInfo NameMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Name));

#if !(V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo LabelPrefixMethod = PatchUtility.PropertyGetter(typeof(Pawn), "LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.
#endif

#if !(V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private static readonly MethodInfo IsSubhumanMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSubhuman));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, NameMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_1 || V1_2 || V1_3 || V1_4)
				if (PatchUtility.Calls(instruction, LabelPrefixMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
				if (PatchUtility.Calls(instruction, IsSubhumanMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
#endif
