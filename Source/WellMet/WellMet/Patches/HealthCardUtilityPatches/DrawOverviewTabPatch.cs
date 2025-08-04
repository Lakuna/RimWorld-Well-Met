#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Lakuna.WellMet.Patches.HealthCardUtilityPatches {
	[HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
	internal static class DrawOverviewTabPatch {
		private static readonly MethodInfo DrawLeftRowMethod = AccessTools.Method(typeof(HealthCardUtility), "DrawLeftRow");

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				// Don't call `HealthCardUtility.DrawLeftRow`; just pop its arguments instead.
				if (PatchUtility.Calls(instruction, DrawLeftRowMethod)) {
					// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
					yield return PatchUtility.LoadValue(InformationCategory.Health); // `informationCategory`
					yield return new CodeInstruction(OpCodes.Ldarg_1); // `pawn`
					yield return PatchUtility.LoadValue(false); // `isControl`

					// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
					yield return new CodeInstruction(OpCodes.Call, PatchUtility.IsInformationKnownForPawnMethod); // Remove the arguments from the stack and add the return value.

					// If the value on top of the stack is `true` (the given information is known), don't skip the `HealthCardUtility.DrawLeftRow` call.
					Label dontSkipLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Brtrue_S, dontSkipLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, go to the call.

					// Don't call `HealthCardUtility.DrawLeftRow`; just pop its arguments instead.
					yield return new CodeInstruction(OpCodes.Pop); // `tipSignal`
					yield return new CodeInstruction(OpCodes.Pop); // `rightLabelColor`
					yield return new CodeInstruction(OpCodes.Pop); // `rightLabel`
					yield return new CodeInstruction(OpCodes.Pop); // `leftLabel`
					yield return new CodeInstruction(OpCodes.Pop); // `curY`
					yield return new CodeInstruction(OpCodes.Pop); // `rect`

					// If the value on top of the stack was `false` (the given information is not known), skip the `HealthCardUtility.DrawLeftRow` call.
					Label doSkipLabel = generator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Br, doSkipLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, go to the call.

					// Jump here when the given information is known, skipping the code that pops the arguments (thus not modifying the stack).
					CodeInstruction dontSkipTarget = new CodeInstruction(OpCodes.Nop);
					dontSkipTarget.labels.Add(dontSkipLabel);
					yield return dontSkipTarget;

					// Call `HealthCardUtility.DrawLeftRow`.
					yield return instruction;

					// Jump here when the given information is not known, skipping the code that calls the function.
					CodeInstruction doSkipTarget = new CodeInstruction(OpCodes.Nop);
					doSkipTarget.labels.Add(doSkipLabel);
					yield return doSkipTarget;

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}
	}
}
