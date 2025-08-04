#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace Lakuna.WellMet.Patches.GenMapUiPatches {
	[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.DrawPawnLabel), new Type[] { typeof(Pawn), typeof(Rect), typeof(float), typeof(float), typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool) })]
	internal static class DrawPawnLabelPatch {
		private static readonly MethodInfo SummaryHealthPercentMethod = PatchUtility.PropertyGetter(typeof(SummaryHealthHandler), nameof(SummaryHealthHandler.SummaryHealthPercent));

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn);

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, SummaryHealthPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, 1f)) {
						yield return i;
					}
				}
			}
		}
	}
}
