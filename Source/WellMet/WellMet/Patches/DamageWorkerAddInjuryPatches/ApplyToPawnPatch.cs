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

namespace Lakuna.WellMet.Patches.DamageWorkerAddInjuryPatches {
	[HarmonyPatch(typeof(DamageWorker_AddInjury), "ApplyToPawn")]
	internal static class ApplyToPawnPatch {
		private static readonly MethodInfo ThrowTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(Color), typeof(float) });

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_3) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, ThrowTextMethod)) {
					Log.Message("Replaced `MoteMaker.ThrowText` in `DamageWorker_AddInjury.ApplyToPawn`."); // TODO
				}
			}
		}
	}
}
