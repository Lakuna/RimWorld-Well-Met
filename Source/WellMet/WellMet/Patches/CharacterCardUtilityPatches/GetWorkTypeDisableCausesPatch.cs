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
		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			{ AccessTools.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Childhood)), InformationCategory.Backstory },
			{ AccessTools.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Adulthood)), InformationCategory.Backstory },
#endif
			{ AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo)), InformationCategory.Ideoligion }
		};

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
#if V1_0 || V1_1 || V1_2 || V1_3
			{ AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.childhood)), InformationCategory.Backstory },
			{ AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.adulthood)), InformationCategory.Backstory },
#endif
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.health)), InformationCategory.Health },
			{ AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.traits)), InformationCategory.Traits },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.genes)), InformationCategory.Advanced }
#endif
		};

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				bool flag = false;
				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (instruction.Calls(row.Key)) {
						foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(row.Value, getPawnInstructions, generator)) {
							yield return i;
						}

						flag = true;
						break;
					}
				}
				if (flag) {
					continue;
				}

				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (instruction.LoadsField(row.Key)) {
						foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(row.Value, getPawnInstructions, generator)) {
							yield return i;
						}

						flag = true;
						break;
					}
				}
				if (flag) {
					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				if (instruction.Calls(IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
