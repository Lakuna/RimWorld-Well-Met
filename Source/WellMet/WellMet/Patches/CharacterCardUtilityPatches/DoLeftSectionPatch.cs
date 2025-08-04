using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
#if V1_0 || V1_1 || V1_2 || V1_3
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
#else
	[HarmonyPatch(typeof(CharacterCardUtility), "DoLeftSection")]
#endif
	internal static class DoLeftSectionPatch {
#if V1_0 || V1_1 || V1_2
		private static readonly FieldInfo AbilitiesField = AccessTools.Field(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.abilities));
#else
		private static readonly MethodInfo AllAbilitiesForReadingMethod = AccessTools.PropertyGetter(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.AllAbilitiesForReading));
#endif

		private static readonly ConstructorInfo AbilityListConstructor = AccessTools.Constructor(typeof(List<Ability>));

		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title));

		private static readonly MethodInfo GetBackstoryMethod = AccessTools.Method(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo SourceGeneField = AccessTools.Field(typeof(Trait), nameof(Trait.sourceGene));
#endif

		private static readonly FieldInfo AllTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo CombinedDisabledWorkTagsMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.CombinedDisabledWorkTags));

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn);

		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(DoLeftSectionPatch), nameof(ActionDelegateTranspiler));

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
			FieldInfo pawnField = original.DeclaringType.GetField("pawn");
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, pawnField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// If the pawn field isn't present, return unmodified instructions.
				if (pawnField == null) {
					continue;
				}

				if (instruction.Calls(GetBackstoryMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Backstory, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (instruction.LoadsField(TitleField) || instruction.LoadsField(SourceGeneField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}
#endif
			}
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (
#if V1_0 || V1_1 || V1_2
					instruction.LoadsField(AbilitiesField)
#else
					instruction.Calls(AllAbilitiesForReadingMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Abilities, getPawnInstructions, generator, AbilityListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(TitleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(CharacterCardUtility)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
						.GetMethods()
#else
						.GetDeclaredMethods()
#endif
						) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
						HarmonyPatcher.Instance.Patch(methodInfo, transpiler: new HarmonyMethod(ActionDelegateTranspilerMethod));
#else
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: ActionDelegateTranspilerMethod);
#endif
					}

					continue;
				}

				if (instruction.LoadsField(AllTraitsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Traits, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(CombinedDisabledWorkTagsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Skills, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
