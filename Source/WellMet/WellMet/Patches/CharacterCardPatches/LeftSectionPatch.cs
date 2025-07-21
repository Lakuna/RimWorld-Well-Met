using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "DoLeftSection")]
	internal static class LeftSectionPatch {
		private static readonly MethodInfo AllAbilitiesForReadingMethod = AccessTools.PropertyGetter(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.AllAbilitiesForReading));

		private static readonly ConstructorInfo AbilityListConstructor = AccessTools.Constructor(typeof(List<Ability>));

		private static readonly MethodInfo CombinedDisabledWorkTagsMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.CombinedDisabledWorkTags));

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title)), InformationCategory.Basic },
			{ AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits)), InformationCategory.Traits }
		};

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.Method(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory)), InformationCategory.Backstory }
		};

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn);

		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(LeftSectionPatch), nameof(ActionDelegateTranspiler));

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
			FieldInfo pawnField = original.DeclaringType.GetField("pawn");

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// If the pawn field isn't present, return unmodified instructions.
				if (pawnField == null) {
					continue;
				}

				// Replace method results with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (instruction.Calls(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}
			}
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(CharacterCardUtility)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType.GetDeclaredMethods()) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, null, null, ActionDelegateTranspilerMethod);
					}
				}

				// Replace fields with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (instruction.LoadsField(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_2);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}

				// Replace `pawn.abilities.AllAbilitiesForReading` with `KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn) ? pawn.abilities.AllAbilitiesForReading : new List<Ability>()`.
				if (instruction.Calls(AllAbilitiesForReadingMethod)) {
					Label dontNullifyLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Abilities);
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
					yield return new CodeInstruction(OpCodes.Pop);
					yield return new CodeInstruction(OpCodes.Newobj, AbilityListConstructor);
					CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
					dontNullifyTarget.labels.Add(dontNullifyLabel);
					yield return dontNullifyTarget;
				}

				// Replace `pawn.CombinedDisabledWorkTags` with `KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn) ? pawn.CombinedDisabledWorkTags : WorkTags.None`.
				if (instruction.Calls(CombinedDisabledWorkTagsMethod)) {
					Label dontNullifyLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Skills);
					yield return new CodeInstruction(OpCodes.Ldarg_2);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
					yield return new CodeInstruction(OpCodes.Pop);
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)WorkTags.None);
					CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
					dontNullifyTarget.labels.Add(dontNullifyLabel);
					yield return dontNullifyTarget;
				}
			}
		}
	}
}
