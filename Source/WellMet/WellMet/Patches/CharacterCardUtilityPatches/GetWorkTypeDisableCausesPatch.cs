#if !V1_0
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetWorkTypeDisableCauses")]
	internal static class GetWorkTypeDisableCausesPatch {
#if V1_1 || V1_2 || V1_3
		private static readonly FieldInfo AdulthoodField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.adulthood));

		private static readonly FieldInfo ChildhoodField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.childhood));
#else
		private static readonly MethodInfo ChildhoodMethod = AccessTools.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Childhood));

		private static readonly MethodInfo AdulthoodMethod = AccessTools.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Adulthood));

		private static readonly MethodInfo IdeoMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

#if !(V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		private static readonly FieldInfo HealthField = AccessTools.Field(typeof(Pawn), nameof(Pawn.health));

		private static readonly FieldInfo TraitsField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.traits));

		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

#if !(V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo GenesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if V1_1 || V1_2 || V1_3
				if (instruction.LoadsField(ChildhoodField) || instruction.LoadsField(AdulthoodField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else
				if (instruction.Calls(ChildhoodMethod) || instruction.Calls(AdulthoodMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(IdeoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_1 || V1_2 || V1_3 || V1_4)
				if (instruction.Calls(IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif

				if (instruction.LoadsField(HealthField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(TraitsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Traits, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(RoyaltyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_1 || V1_2 || V1_3)
				if (instruction.LoadsField(GenesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
#endif
