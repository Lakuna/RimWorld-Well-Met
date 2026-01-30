using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.BloodRainUtilityPatches {
	[HarmonyPatch(typeof(BloodRainUtility), nameof(BloodRainUtility.TryTriggerBerserkShort))]
	internal static class TryTriggerBerserkShortPatch {
		private static readonly MethodInfo MessageShowAllowedMethod = AccessTools.Method(typeof(MessagesRepeatAvoider), nameof(MessagesRepeatAvoider.MessageShowAllowed));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, MessageShowAllowedMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions, ControlCategory.Message)) {
						yield return i;
					}
				}
			}
		}
	}
}
