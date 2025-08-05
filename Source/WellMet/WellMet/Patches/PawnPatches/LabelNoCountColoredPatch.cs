#if !V1_0
using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.LabelNoCountColored), MethodType.Getter)]
	internal static class LabelNoCountColoredPatch {
		private static readonly MethodInfo NameMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Name));

		private static readonly FieldInfo StoryField = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));

#if !(V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo IsSubhumanMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSubhuman));

		private static readonly MethodInfo LabelPrefixMethod = AccessTools.PropertyGetter(typeof(Pawn), "LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.
#endif

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

				if (instruction.LoadsField(StoryField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Backstory, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_1 || V1_2 || V1_3 || V1_4)
				if (instruction.Calls(IsSubhumanMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(LabelPrefixMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, "")) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
#endif
