using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Lakuna.WellMet.Patches.ColonistBarColonistDrawerPatches {
	[HarmonyPatch(typeof(ColonistBarColonistDrawer), nameof(ColonistBarColonistDrawer.DrawColonist))]
	internal static class DrawColonistPatch {
		private static readonly FieldInfo MoodField = AccessTools.Field(typeof(Pawn_NeedsTracker), nameof(Pawn_NeedsTracker.mood));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.LoadsField(MoodField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Needs, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
