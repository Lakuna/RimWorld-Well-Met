#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Lakuna.WellMet.Patches.IdeoUiUtilityPatches {
	[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DrawIdeoPlate))]
	internal static class DrawIdeoPlatePatch {
		private static readonly MethodInfo CertaintyMethod = PatchUtility.PropertyGetter(typeof(Pawn_IdeoTracker), nameof(Pawn_IdeoTracker.Certainty));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, CertaintyMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
