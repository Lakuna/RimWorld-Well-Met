using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetWorkTypeDisableCauses")]
	internal static class GetWorkTypeDisableCausesPatch {
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo GetWorkDisabledQuestPartMethod = AccessTools.Method(typeof(QuestUtility), nameof(QuestUtility.GetWorkDisabledQuestPart));

		private static readonly ConstructorInfo QuestPartWorkDisabledListConstructor = AccessTools.Constructor(typeof(List<QuestPart_WorkDisabled>));

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.health)), InformationCategory.Health },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.genes)), InformationCategory.Basic },
			{ AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.traits)), InformationCategory.Traits }
		};

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Childhood)), InformationCategory.Backstory },
			{ AccessTools.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Adulthood)), InformationCategory.Backstory },
			{ AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo)), InformationCategory.Ideoligion }
		};

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Replace fields with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (instruction.LoadsField(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}

				// Replace method results with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (instruction.Calls(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}

				// Replace `this.IsMutant` with `this.IsMutant && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, this)`.
				if (instruction.Calls(IsMutantMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Health);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `QuestUtility.GetWorkDisabledQuestPart(pawn)` with `KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn) ? QuestUtility.GetWorkDisabledQuestPart(pawn) : new List<QuestPart_WorkDisabled>()`.
				if (instruction.Calls(GetWorkDisabledQuestPartMethod)) {
					Label dontNullifyLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Basic);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
					yield return new CodeInstruction(OpCodes.Pop);
					yield return new CodeInstruction(OpCodes.Newobj, QuestPartWorkDisabledListConstructor);
					CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
					dontNullifyTarget.labels.Add(dontNullifyLabel);
					yield return dontNullifyTarget;
				}
			}
		}
	}
}
