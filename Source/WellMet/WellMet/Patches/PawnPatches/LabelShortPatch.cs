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
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.LabelShort), MethodType.Getter)]
	internal static class LabelShortPatch {
#if V1_0
		private static readonly MethodInfo NameMethod = AccessTools.Method(typeof(Pawn), "get_" + nameof(Pawn.Name));

		private static readonly MethodInfo LabelPrefixMethod = AccessTools.Method(typeof(Pawn), "get_LabelPrefix"); // Only used to indicate whether the pawn is a mutant that has turned.
#else
		private static readonly MethodInfo NameMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Name));

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
