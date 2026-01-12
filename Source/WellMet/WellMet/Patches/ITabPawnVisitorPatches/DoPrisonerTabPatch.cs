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

#pragma warning disable CS0649 // Need an address (non-constant) with a guaranteed value of `0` to replace `will`.
		private static readonly float Zero;
#pragma warning restore CS0649

		private static readonly FieldInfo ZeroField = AccessTools.Field(typeof(DoPrisonerTabPatch), nameof(Zero));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(InitiatePrisonBreakMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(IsPrisonBreakingMethod) || instruction.Calls(GenePreventsPrisonBreakingMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Advanced, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(RecruitableMethod)) {
					foreach (CodeInstruction i in PatchUtility.OrPawnNotKnown(InformationCategory.Advanced, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(ResistanceField) || instruction.LoadsField(MinField) || instruction.LoadsField(MaxField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

				// Will needs to be handled separately because it is loaded by address.
				if (instruction.LoadsField(WillField, true)) {
					// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
					yield return PatchUtility.LoadValue(InformationCategory.Advanced); // `category`.
					foreach (CodeInstruction instruction2 in getPawnInstructions) {
						yield return new CodeInstruction(instruction2); // `pawn` or `faction`.
					}
					yield return PatchUtility.LoadValue(InformationTypeCategory.Default); // `typeCategory`.

					// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
					yield return new CodeInstruction(OpCodes.Call, PatchUtility.IsInformationKnownForPawnMethod); // Remove the arguments from the stack and add the return value.

					// If the value on top of the stack is `true` (the given information is known), don't replace the value.
					Label dontReplaceLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, leaving the value that might be replaced on top.

					// This section is skipped unless the given information isn't known.
					yield return new CodeInstruction(OpCodes.Pop); // Remove the value that is being replaced from the stack.
					yield return new CodeInstruction(OpCodes.Ldsflda, ZeroField);

					// Jump here when the given information is known, skipping the code that replaces the original value (thus not modifying the stack).
					CodeInstruction dontReplaceTarget = new CodeInstruction(OpCodes.Nop);
					dontReplaceTarget.labels.Add(dontReplaceLabel);
					yield return dontReplaceTarget;

					continue;
				}

				if (instruction.LoadsField(RoyaltyField) || instruction.Calls(FactionMethod) || instruction.LoadsField(IdeoForConversionField) || instruction.LoadsField(FinalResistanceInteractionDataField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
