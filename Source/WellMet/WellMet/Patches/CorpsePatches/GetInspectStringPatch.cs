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

namespace Lakuna.WellMet.Patches.CorpsePatches {
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.GetInspectString))]
	internal static class GetInspectStringPatch {
		private static readonly MethodInfo InnerPawnMethod = PatchUtility.PropertyGetter(typeof(Corpse), nameof(Corpse.InnerPawn));

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction)), InformationCategory.Basic },
#if !(V1_0 || V1_1)
			{ AccessTools.Method(typeof(HediffSet), nameof(HediffSet.GetFirstHediff)), InformationCategory.Health }
#endif
		};

		private static readonly MethodInfo ToStringTicksToPeriodVagueMethod = AccessTools.Method(typeof(GenDate), nameof(GenDate.ToStringTicksToPeriodVague));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Call, InnerPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, ToStringTicksToPeriodVagueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, "")) {
						yield return i;
					}

					continue;
				}

				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (PatchUtility.Calls(instruction, row.Key)) {
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
