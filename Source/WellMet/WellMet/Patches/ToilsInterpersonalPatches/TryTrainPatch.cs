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

namespace Lakuna.WellMet.Patches.ToilsInterpersonalPatches {
	[HarmonyPatch(typeof(Toils_Interpersonal), nameof(Toils_Interpersonal.TryTrain))]
	internal static class TryTrainPatch {
		private static readonly FieldInfo ActorField = AccessTools.Field(typeof(Toil), nameof(Toil.actor));

		private static readonly FieldInfo JobsField = AccessTools.Field(typeof(Pawn), nameof(Pawn.jobs));

		private static readonly FieldInfo CurJobField = AccessTools.Field(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.curJob));

		private static readonly MethodInfo GetTargetMethod = AccessTools.Method(typeof(Job), nameof(Job.GetTarget));

		private static readonly MethodInfo ThingMethod = AccessTools.PropertyGetter(typeof(LocalTargetInfo), nameof(LocalTargetInfo.Thing));

		private static readonly MethodInfo ThrowTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });

		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(TryTrainPatch), nameof(ActionDelegateTranspiler));

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
			// If the toil or trainee index field isn't present, return unmodified instructions.
			FieldInfo toilField = original.DeclaringType.GetField("toil");
			FieldInfo traineeIndField = original.DeclaringType.GetField("traineeInd");
			if (toilField == null || traineeIndField == null) {
				foreach (CodeInstruction instruction in instructions) {
					yield return instruction;
				}

				yield break;
			}

			LocalBuilder localTargetInfo = generator.DeclareLocal(typeof(LocalTargetInfo));
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] {
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, toilField),
				new CodeInstruction(OpCodes.Ldfld, ActorField),
				new CodeInstruction(OpCodes.Ldfld, JobsField),
				new CodeInstruction(OpCodes.Ldfld, CurJobField),
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, traineeIndField),
				new CodeInstruction(OpCodes.Callvirt, GetTargetMethod),
				new CodeInstruction(OpCodes.Stloc, localTargetInfo),
				new CodeInstruction(OpCodes.Ldloca_S, localTargetInfo),
				new CodeInstruction(OpCodes.Call, ThingMethod),
				new CodeInstruction(OpCodes.Castclass, typeof(Pawn))
			};

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

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(Toils_Interpersonal)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType.GetDeclaredMethods()) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: ActionDelegateTranspilerMethod);
					}
				}
			}
		}
	}
}
