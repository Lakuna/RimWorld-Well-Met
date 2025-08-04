using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.CorpsePatches {
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.GetInspectString))]
	internal static class GetInspectStringPatch {
		private static readonly MethodInfo InnerPawnMethod = AccessTools.PropertyGetter(typeof(Corpse), nameof(Corpse.InnerPawn));

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction)), InformationCategory.Basic },
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			{ AccessTools.Method(typeof(HediffSet), nameof(HediffSet.GetFirstHediff)), InformationCategory.Health }
#endif
		};

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Call, InnerPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

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
