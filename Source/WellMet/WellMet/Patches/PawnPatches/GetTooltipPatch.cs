using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetTooltip))]
	internal static class GetTooltipPatch {
		private static readonly FieldInfo GenderField = AccessTools.Field(typeof(Pawn), nameof(Pawn.gender));

		private static readonly FieldInfo EquipmentField = AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.LoadsField(GenderField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, (int)Gender.None)) {
						yield return i;
					}

					continue;
				}

				if (instruction.LoadsField(EquipmentField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Gear, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
