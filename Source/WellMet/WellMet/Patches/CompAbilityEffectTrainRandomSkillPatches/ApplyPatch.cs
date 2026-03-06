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

using Verse;

namespace Lakuna.WellMet.Patches.CompAbilityEffectTrainRandomSkillPatches {
	[HarmonyPatch(typeof(CompAbilityEffect_TrainRandomSkill), nameof(CompAbilityEffect_TrainRandomSkill.Apply))]
	internal static class ApplyPatch {
		private static readonly MethodInfo PawnMethod = PatchUtility.PropertyGetter(typeof(LocalTargetInfo), nameof(LocalTargetInfo.Pawn));

		private static readonly MethodInfo SendLetterMethod = PatchUtility.PropertyGetter(typeof(CompAbilityEffect), "SendLetter");

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarga_S, 1), new CodeInstruction(OpCodes.Call, PawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, SendLetterMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Skills, getPawnInstructions, ControlCategory.Letter)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
