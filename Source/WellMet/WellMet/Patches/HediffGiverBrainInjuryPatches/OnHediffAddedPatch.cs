#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Verse;

namespace Lakuna.WellMet.Patches.HediffGiverBrainInjuryPatches {
	[HarmonyPatch(typeof(HediffGiver_BrainInjury), nameof(HediffGiver_BrainInjury.OnHediffAdded))]
	internal static class OnHediffAddedPatch {
		private static readonly MethodInfo ReceiveLetterMethod = AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), new Type[] { typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(LookTargets), typeof(Faction), typeof(Quest), typeof(List<ThingDef>), typeof(string), typeof(int), typeof(bool) });

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, ReceiveLetterMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Health, getPawnInstructions, generator, controlCategory: ControlCategory.Letter)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}
	}
}
