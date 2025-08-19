#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.LabelNoCount), MethodType.Getter)]
	internal static class LabelNoCountPatch {
		private static readonly MethodInfo NameMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Name));

		private static readonly FieldInfo StoryField = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo LabelPrefixMethod = AccessTools.PropertyGetter(typeof(Pawn), "LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private static readonly MethodInfo IsSubhumanMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSubhuman));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, NameMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, StoryField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Backstory, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				if (instruction.Calls(LabelPrefixMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, "")) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
				if (instruction.Calls(IsSubhumanMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
