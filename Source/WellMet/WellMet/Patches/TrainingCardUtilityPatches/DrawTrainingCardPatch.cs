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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
		private static readonly FieldInfo TrainabilityField = AccessTools.Field(typeof(RaceProperties), nameof(RaceProperties.trainability));
#else
		private static readonly MethodInfo GetTrainabilityMethod = AccessTools.Method(typeof(TrainableUtility), nameof(TrainableUtility.GetTrainability));
#endif

		private static readonly MethodInfo ToStringPercentMethod = SymbolExtensions.GetMethodInfo((float f) => f.ToStringPercent()); // Used only for creature wildness in this method.

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;


#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
				if (instruction.LoadsField(TrainabilityField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else
				if (instruction.Calls(GetTrainabilityMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

				if (instruction.Calls(ToStringPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, "")) {
						yield return i;
					}
				}
			}
		}
	}
}
