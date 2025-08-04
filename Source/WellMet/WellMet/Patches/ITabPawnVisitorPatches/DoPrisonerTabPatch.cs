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
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoPrisonerTab")]
	internal static class DoPrisonerTabPatch {
#if V1_0
		private static readonly MethodInfo SelPawnMethod = AccessTools.Method(typeof(ITab), "get_SelPawn");
#else
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");
#endif

		private static readonly MethodInfo InitiatePrisonBreakMtbDaysMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.InitiatePrisonBreakMtbDays));

		private static readonly MethodInfo IsPrisonBreakingMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.IsPrisonBreaking));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly MethodInfo GenePreventsPrisonBreakingMethod = AccessTools.Method(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.GenePreventsPrisonBreaking));
#endif

		private static readonly FieldInfo ResistanceField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.resistance));

		private static readonly FieldInfo MinField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.min));

		private static readonly FieldInfo MaxField = AccessTools.Field(typeof(FloatRange), nameof(FloatRange.max));

#if !V1_0
		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));
#endif

#if !(V1_0 || V1_1 || V1_2)
		private static readonly FieldInfo WillField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.will));
#endif

#if V1_0
		private static readonly MethodInfo FactionMethod = AccessTools.Method(typeof(Pawn), "get_" + nameof(Pawn.Faction));
#else
		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Faction));
#endif

#if !(V1_0 || V1_1 || V1_2)
		private static readonly FieldInfo IdeoForConversionField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.ideoForConversion));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo FinalResistanceInteractionDataField = AccessTools.Field(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.finalResistanceInteractionData));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (
#if V1_0
					PatchUtility.Calls(instruction, InitiatePrisonBreakMtbDaysMethod)
#else
					instruction.Calls(InitiatePrisonBreakMtbDaysMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (
#if V1_0
					PatchUtility.Calls(instruction, IsPrisonBreakingMethod)
#else
					instruction.Calls(IsPrisonBreakingMethod)
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3)
					|| instruction.Calls(GenePreventsPrisonBreakingMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, false)) {
						yield return i;
					}

					continue;
				}

				if (
#if V1_0
					PatchUtility.LoadsField(instruction, ResistanceField) || PatchUtility.LoadsField(instruction, MinField) || PatchUtility.LoadsField(instruction, MaxField)
#else
					instruction.LoadsField(ResistanceField) || instruction.LoadsField(MinField) || instruction.LoadsField(MaxField)
#endif
#if !(V1_0 || V1_1 || V1_2)
					|| instruction.LoadsField(WillField)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				if (
#if V1_0
					PatchUtility.Calls(instruction, FactionMethod)
#else
					instruction.LoadsField(RoyaltyField) || instruction.Calls(FactionMethod)
#endif
#if !(V1_0 || V1_1 || V1_2)
					|| instruction.LoadsField(IdeoForConversionField)
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3)
					|| instruction.LoadsField(FinalResistanceInteractionDataField)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
