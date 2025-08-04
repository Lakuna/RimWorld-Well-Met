#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Lakuna.WellMet.Patches.RelationsUtilityPatches {
	[HarmonyPatch(typeof(RelationsUtility), nameof(RelationsUtility.LabelWithBondInfo))]
	internal static class LabelWithBondInfoPatch {
		private static readonly MethodInfo DirectRelationExistsMethod = AccessTools.Method(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.DirectRelationExists));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (
#if V1_0
					PatchUtility.Calls(instruction, DirectRelationExistsMethod)
#else
					instruction.Calls(DirectRelationExistsMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Social, getPawnInstructions)) {
						yield return i;
					}
				}
			}
		}
	}
}
