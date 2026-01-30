#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawPortraitArea))]
	internal static class DrawPortraitAreaPatch {
		private static readonly MethodInfo AnyMethod = SymbolExtensions.GetMethodInfo((List<ThingDefCount> list) => list.Any());

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldloc_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Used only to check if the pawn has any possessions in this method.
				if (PatchUtility.Calls(instruction, AnyMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Gear, getPawnInstructions)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
