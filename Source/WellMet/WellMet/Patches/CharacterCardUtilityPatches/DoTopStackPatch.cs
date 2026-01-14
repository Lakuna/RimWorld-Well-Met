#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "DoTopStack")]
	internal static class DoTopStackPatch {
		private static readonly FieldInfo GenesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));

		private static readonly FieldInfo FactionField = AccessTools.Field(typeof(ExtraFaction), nameof(ExtraFaction.faction));

		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly FieldInfo StoryField = AccessTools.Field(typeof(Pawn), nameof(Pawn.story));

		private static readonly FieldInfo GuestField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guest));

		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly MethodInfo IdeoMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn, ref bool creationMode) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
			creationMode = creationMode && basic;
			return basic
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// `story` is used only for favorite color in this method.
				// `guest` is used only for unwaveringly loyal status in this method.
				if (instruction.LoadsField(GenesField) || instruction.LoadsField(RoyaltyField) || instruction.LoadsField(StoryField) || instruction.LoadsField(GuestField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// `pawn.Faction == tmpExtraFaction.faction` will always be `true` since both sides will be `null`, causing extra factions to be skipped.
				if (instruction.LoadsField(FactionField) || instruction.Calls(FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(IdeoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
#endif
