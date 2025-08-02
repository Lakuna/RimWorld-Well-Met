using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	internal static class DrawCharacterCardPatch {
		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt)), InformationCategory.Basic }
		};

		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn, ref bool showName) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, true); // Name must be shown for renaming, banishing, and starting colonist randomization to work. Guilt must be shown for the "execute colonist" button.
			showName = showName && basic;
			return basic
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn, true) // "Renounce title" button.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn);
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (instruction.LoadsField(row.Key)) {
						foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(row.Value, getPawnInstructions, generator, isControl: true)) { // Royalty is used only for the "renounce title" button; guilt is used only for the "execute colonist" button.
							yield return i;
						}
					}
				}
			}
		}
	}
}
