﻿#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
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
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "FillTab")]
	internal static class FillTabPatch {
		private static readonly MethodInfo SelPawnMethod = PatchUtility.PropertyGetter(typeof(ITab), "SelPawn");

#if !V1_0
		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Faction));
#endif

#if V1_0 || V1_1 || V1_2
		private static readonly MethodInfo RecruitDifficultyMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.RecruitDifficulty));
#else
		private static readonly FieldInfo WillField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.will));

		private static readonly FieldInfo IdeoForConversionField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.ideoForConversion));

		private static readonly MethodInfo CurLevelMethod = AccessTools.PropertyGetter(typeof(Need), nameof(Need.CurLevel));

		private static readonly MethodInfo GetStatValueMethod = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValue));

		private static readonly MethodInfo GetTerrorThoughtsMethod = AccessTools.Method(typeof(TerrorUtility), nameof(TerrorUtility.GetTerrorThoughts));

		private static readonly ConstructorInfo ThoughtMemoryObservationTerrorListConstructor = AccessTools.Constructor(typeof(List<Thought_MemoryObservationTerror>));

		private static readonly MethodInfo InitiateSlaveRebellionMtbDaysMethod = AccessTools.Method(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.InitiateSlaveRebellionMtbDays));

		private static readonly MethodInfo SlaveFactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.SlaveFaction));
#endif

		private static readonly MethodInfo InitiatePrisonBreakMtbDaysMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.InitiatePrisonBreakMtbDays));

		private static readonly FieldInfo ResistanceField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.resistance));

		private static readonly FieldInfo MinField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.min));

		private static readonly FieldInfo MaxField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.max));

		private static readonly MethodInfo IsGuiltyMethod = PatchUtility.PropertyGetter(typeof(Pawn_GuiltTracker), nameof(Pawn_GuiltTracker.IsGuilty));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if !V1_0
				if (instruction.LoadsField(RoyaltyField) || instruction.Calls(FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if V1_0 || V1_1 || V1_2
				if (PatchUtility.Calls(instruction, RecruitDifficultyMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}
#else
				if (instruction.LoadsField(WillField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(IdeoForConversionField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(SlaveFactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

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
#endif

				if (PatchUtility.Calls(instruction, InitiatePrisonBreakMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, ResistanceField) || PatchUtility.LoadsField(instruction, MinField) || PatchUtility.LoadsField(instruction, MaxField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IsGuiltyMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
