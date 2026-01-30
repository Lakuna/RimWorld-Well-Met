#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoPrisonerTab")]
	internal static class DoPrisonerTabPatch {
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		private static readonly MethodInfo InitiatePrisonBreakMtbDaysMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.InitiatePrisonBreakMtbDays));

		private static readonly MethodInfo IsPrisonBreakingMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.IsPrisonBreaking));

		private static readonly MethodInfo GenePreventsPrisonBreakingMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.GenePreventsPrisonBreaking));

		private static readonly MethodInfo RecruitableMethod = AccessTools.PropertyGetter(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.Recruitable));

		private static readonly FieldInfo ResistanceField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.resistance));

		private static readonly FieldInfo MinField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.min));

		private static readonly FieldInfo MaxField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.max));

		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly FieldInfo WillField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.will));

		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly FieldInfo IdeoForConversionField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.ideoForConversion));

		private static readonly FieldInfo FinalResistanceInteractionDataField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.finalResistanceInteractionData));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, InitiatePrisonBreakMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IsPrisonBreakingMethod) || PatchUtility.Calls(instruction, GenePreventsPrisonBreakingMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Meta, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, RecruitableMethod)) {
					foreach (CodeInstruction i in PatchUtility.OrPawnNotKnown(InformationCategory.Meta, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, ResistanceField) || PatchUtility.LoadsField(instruction, MinField) || PatchUtility.LoadsField(instruction, MaxField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, WillField, true)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator, 0f, localAddress: true)) {
						yield return i;
					}
				}

				if (PatchUtility.LoadsField(instruction, IdeoForConversionField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// Royalty is used here only for royal title resistance offset.
				if (PatchUtility.LoadsField(instruction, RoyaltyField) || PatchUtility.Calls(instruction, FactionMethod) || PatchUtility.LoadsField(instruction, FinalResistanceInteractionDataField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Meta, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
