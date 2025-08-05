#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawPortraitArea))]
	internal static class DrawPortraitAreaPatch {
		private static readonly MethodInfo AnyMethod = SymbolExtensions.GetMethodInfo((List<ThingDefCount> list) => list.Any()); // Used only to check if the pawn has any possessions in this method.

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldloc_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(AnyMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Gear, getPawnInstructions)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
