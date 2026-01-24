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
using UnityEngine;
using Verse;
using Verse.AI;

namespace Lakuna.WellMet.Patches.JobDriverCarryGenepackToContainerPatches {
	[HarmonyPatch(typeof(JobDriver_CarryGenepackToContainer), "MakeNewToils")]
	internal static class MakeNewToilsPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(JobDriver), nameof(JobDriver.pawn));

		private static readonly MethodInfo ThrowTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });

		private static readonly MethodInfo InnerActionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler));

		private static IEnumerable<CodeInstruction> InnerActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, ThrowTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Meta, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}

		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler));

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to functions referenced via pointer.
				if (instruction.opcode == OpCodes.Ldftn && instruction.operand is MethodInfo methodInfo && methodInfo.DeclaringType == typeof(JobDriver_CarryGenepackToContainer)) {
					_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: InnerActionDelegateTranspilerMethod);
				}
			}
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(JobDriver_CarryGenepackToContainer)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType.GetDeclaredMethods()) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: ActionDelegateTranspilerMethod);
					}
				}
			}
		}
	}
}
