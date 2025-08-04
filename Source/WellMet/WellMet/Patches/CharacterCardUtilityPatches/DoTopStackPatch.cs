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
		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.genes)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(ExtraFaction), nameof(ExtraFaction.faction)), InformationCategory.Basic }, // `pawn.Faction == tmpExtraFaction.faction` will always be `true` since both sides will be `null`, causing extra factions to be skipped.
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.story)), InformationCategory.Advanced }, // `story` is used only for favorite color in this method.
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.guest)), InformationCategory.Advanced } // `guest` is used only for unwaveringly loyal status in this method.
		};

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction)), InformationCategory.Basic },
			{ AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo)), InformationCategory.Ideoligion }
		};

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

				bool flag = false;
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

				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (instruction.Calls(row.Key)) {
						foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(row.Value, getPawnInstructions, generator)) {
							yield return i;
						}

						break;
					}
				}
			}
		}
	}
}
