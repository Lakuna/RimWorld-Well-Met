#if !V1_0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsWorkPatches {
	[HarmonyPatch(typeof(WidgetsWork), nameof(WidgetsWork.DrawWorkBoxFor))]
	internal static class DrawWorkBoxForPatch {
		private static readonly MethodInfo WorkTypeIsDisabledMethod = AccessTools.Method(typeof(Pawn), nameof(Pawn.WorkTypeIsDisabled));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_3) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, WorkTypeIsDisabledMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfAnySkillsForWorkTypeNotKnown(getPawnInstructions, getWorkTypeInstructions, generator, false)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
