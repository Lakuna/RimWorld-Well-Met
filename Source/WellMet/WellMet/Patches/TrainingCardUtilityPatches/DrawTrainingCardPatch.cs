using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.TrainingCardUtilityPatches {
	[HarmonyPatch(typeof(TrainingCardUtility), nameof(TrainingCardUtility.DrawTrainingCard))]
	internal static class DrawTrainingCardPatch {
		private static readonly MethodInfo GetTrainabilityMethod = AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.GetTrainability));

		private static readonly MethodInfo ToStringPercentMethod = SymbolExtensions.GetMethodInfo((float f) => f.ToStringPercent()); // Used only for creature wildness in this method.

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(GetTrainabilityMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}

				if (instruction.Calls(ToStringPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, "")) {
						yield return i;
					}
				}
			}
		}
	}
}
