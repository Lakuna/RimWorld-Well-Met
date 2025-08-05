﻿#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlaveTab")]
	internal static class DoSlaveTabPatch {
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		private static readonly MethodInfo CurLevelMethod = AccessTools.PropertyGetter(typeof(Need), nameof(Need.CurLevel));

		private static readonly MethodInfo GetStatValueMethod = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValue));

		private static readonly MethodInfo GetTerrorThoughtsMethod = AccessTools.Method(typeof(TerrorUtility), nameof(TerrorUtility.GetTerrorThoughts));

		private static readonly ConstructorInfo ThoughtMemoryObservationTerrorListConstructor = AccessTools.Constructor(typeof(List<Thought_MemoryObservationTerror>));

		private static readonly MethodInfo InitiateSlaveRebellionMtbDaysMethod = AccessTools.Method(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.InitiateSlaveRebellionMtbDays));

		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Faction));

		private static readonly MethodInfo SlaveFactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.SlaveFaction));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(CurLevelMethod) || instruction.Calls(GetStatValueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(GetTerrorThoughtsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, ThoughtMemoryObservationTerrorListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(InitiateSlaveRebellionMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(FactionMethod) || instruction.Calls(SlaveFactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
