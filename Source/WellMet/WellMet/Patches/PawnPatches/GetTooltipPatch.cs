#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
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

				if (
#if V1_0
					PatchUtility.LoadsField(instruction, GenderField)
#else
					instruction.LoadsField(GenderField)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, (int)Gender.None)) {
						yield return i;
					}

					continue;
				}

				if (
#if V1_0
					PatchUtility.LoadsField(instruction, EquipmentField)
#else
					instruction.LoadsField(EquipmentField)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Gear, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
