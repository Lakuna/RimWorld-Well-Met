#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
using System;
#endif
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	internal static class DrawCharacterCardPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo ToStringFullMethod = PatchUtility.PropertyGetter(typeof(Name), nameof(Name.ToStringFull));

		private static readonly MethodInfo SpawnedMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Spawned));

		private static readonly MethodInfo IsColonistMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsColonist));

		private static readonly MethodInfo GetBackstoryMethod = AccessTools.Method(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));

#if V1_0
		private static readonly MethodInfo CombinedDisabledWorkTagsMethod = PatchUtility.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.CombinedDisabledWorkTags));
#else
		private static readonly MethodInfo CombinedDisabledWorkTagsMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.CombinedDisabledWorkTags));
#endif

		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title));

		private static readonly FieldInfo AllTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo FilterTraitsMethod = AccessTools.Method(typeof(DrawCharacterCardPatch), nameof(FilterTraits));
		private static List<Trait> FilterTraits(List<Trait> traits, Pawn pawn) {
			List<Trait> outValue = new List<Trait>();
			foreach (Trait trait in traits) {
				if (!KnowledgeUtility.IsTraitKnown(pawn, trait.def)) {
					continue;
				}

				outValue.Add(trait);
			}

			return outValue;
		}
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly MethodInfo GetExtraFactionsFromQuestPartsMethod = AccessTools.Method(typeof(QuestUtility), nameof(QuestUtility.GetExtraFactionsFromQuestParts));

		private static readonly FieldInfo AbilitiesField = AccessTools.Field(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.abilities));

		private static readonly ConstructorInfo AbilityListConstructor = AccessTools.Constructor(typeof(List<Ability>));

		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(DrawCharacterCardPatch), nameof(ActionDelegateTranspiler));

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

				if (instruction.LoadsField(TitleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
#endif

#if !V1_0
		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
#if !V1_1
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt)), InformationCategory.Basic }
#endif
		};
#endif

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn,
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			ref Action randomizeCallback
#else
			ref bool showName
#endif
		) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, true); // Name must be shown for renaming, banishing, and starting colonist randomization to work. Guilt must be shown for the "execute colonist" button.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			if (!basic) {
				randomizeCallback = null;
			}
#else
			showName = showName && basic;
#endif
			return basic
#if !V1_0
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn, true) // "Renounce title" button.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
#endif
#if !(V1_0 || V1_1 || V1_2)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
#endif
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn);
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
#if V1_1 || V1_2 || V1_3 || V1_4
				// Don't call `QuestUtility.GetExtraFactionsFromQuestPartsMethod`; just pop its arguments instead.
				if (instruction.Calls(GetExtraFactionsFromQuestPartsMethod)) {
					// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
					yield return PatchUtility.LoadValue(InformationCategory.Basic); // `informationCategory`
					yield return new CodeInstruction(OpCodes.Ldarg_1); // `pawn`
					yield return PatchUtility.LoadValue(false); // `isControl`

					// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
					yield return new CodeInstruction(OpCodes.Call, PatchUtility.IsInformationKnownForPawnMethod); // Remove the arguments from the stack and add the return value.

					// If the value on top of the stack is `true` (the given information is known), don't skip the `QuestUtility.GetExtraFactionsFromQuestPartsMethod` call.
					Label dontSkipLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Brtrue_S, dontSkipLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, go to the call.

					// Don't call `QuestUtility.GetExtraFactionsFromQuestPartsMethod`; just pop its arguments instead.
					yield return new CodeInstruction(OpCodes.Pop); // `forQuest`
					yield return new CodeInstruction(OpCodes.Pop); // `outExtraFactions`
					yield return new CodeInstruction(OpCodes.Pop); // `pawn`

					// If the value on top of the stack was `false` (the given information is not known), skip the `QuestUtility.GetExtraFactionsFromQuestPartsMethod` call.
					Label doSkipLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Br, doSkipLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, go to the call.

					// Jump here when the given information is known, skipping the code that pops the arguments (thus not modifying the stack).
					CodeInstruction dontSkipTarget = new CodeInstruction(OpCodes.Nop);
					dontSkipTarget.labels.Add(dontSkipLabel);
					yield return dontSkipTarget;

					// Call `QuestUtility.GetExtraFactionsFromQuestPartsMethod`.
					yield return instruction;

					// Jump here when the given information is not known, skipping the code that calls the function.
					CodeInstruction doSkipTarget = new CodeInstruction(OpCodes.Nop);
					doSkipTarget.labels.Add(doSkipLabel);
					yield return doSkipTarget;

					// Skip the normal instruction (already returned above).
					continue;
				}
#endif

				yield return instruction;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, ToStringFullMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, "")) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, SpawnedMethod) || PatchUtility.Calls(instruction, IsColonistMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions, true)) { // `Spawned` is used only for the "banish" button. `IsColonist` is used only for the "rename colonist" button.
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, GetBackstoryMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Backstory, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, CombinedDisabledWorkTagsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Skills, getPawnInstructions, generator, (int)WorkTags.None)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, TitleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, AllTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, FilterTraitsMethod);
					continue;
				}
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
				if (instruction.Calls(FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(AbilitiesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Abilities, getPawnInstructions, generator, AbilityListConstructor)) {
						yield return i;
					}

					continue;
				}

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(CharacterCardUtility)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType.GetMethods()) {
						if (!methodInfo.IsDeclaredMember()) {
							continue;
						}

						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: new HarmonyMethod(ActionDelegateTranspilerMethod));
					}

					continue;
				}
#endif

#if !V1_0
				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (PatchUtility.LoadsField(instruction, row.Key)) {
						foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(row.Value, getPawnInstructions, generator, isControl: true)) { // Royalty is used only for the "renounce title" button; guilt is used only for the "execute colonist" button.
							yield return i;
						}

						break;
					}
				}
#endif
			}
		}
	}
}
