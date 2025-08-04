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
using Verse;

namespace Lakuna.WellMet.Patches.HealthCardUtilityPatches {
	[HarmonyPatch(typeof(HealthCardUtility), nameof(HealthCardUtility.DrawHediffListing))]
	internal static class DrawHediffListingPatch {
#if V1_0
		private static readonly MethodInfo BleedRateTotalMethod = AccessTools.Method(typeof(HediffSet), "get_" + nameof(HediffSet.BleedRateTotal));
#else
		private static readonly MethodInfo BleedRateTotalMethod = AccessTools.PropertyGetter(typeof(HediffSet), nameof(HediffSet.BleedRateTotal));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo GenesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (
#if V1_0
					PatchUtility.Calls(instruction, BleedRateTotalMethod)
#else
					instruction.Calls(BleedRateTotalMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (instruction.LoadsField(GenesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
