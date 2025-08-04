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
#if V1_0
		private static readonly MethodInfo NameMethod = AccessTools.Method(typeof(Pawn), "get_" + nameof(Pawn.Name));
#else
		private static readonly MethodInfo NameMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Name));
#endif

		private static readonly FieldInfo StoryField = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
		private static readonly MethodInfo IsSubhumanMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSubhuman));
#endif

#if V1_0
		private static readonly MethodInfo LabelPrefixMethod = AccessTools.Method(typeof(Pawn), "get_LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.
#else
		private static readonly MethodInfo LabelPrefixMethod = AccessTools.PropertyGetter(typeof(Pawn), "LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (
#if V1_0
					PatchUtility.Calls(instruction, NameMethod)
#else
					instruction.Calls(NameMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (
#if V1_0
					PatchUtility.LoadsField(instruction, StoryField)
#else
					instruction.LoadsField(StoryField)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Backstory, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
				if (instruction.Calls(IsSubhumanMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif

				if (
#if V1_0
					PatchUtility.Calls(instruction, LabelPrefixMethod)
#else
					instruction.Calls(LabelPrefixMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, "")) {
						yield return i;
					}
				}
			}
		}
	}
}
