using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPane {
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.GetInspectString))]
	internal static class CorpseInspectStringPatch {
		private static readonly MethodInfo InnerPawnMethod = AccessTools.PropertyGetter(typeof(Corpse), nameof(Corpse.InnerPawn));

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction)), InformationCategory.Basic },
			{ AccessTools.Method(typeof(HediffSet), nameof(HediffSet.GetFirstHediff)), InformationCategory.Health },
			{ AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.GetInspectString)), InformationCategory.Basic }
		};

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Replace method results with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (instruction.Calls(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, InnerPawnMethod);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}
			}
		}
	}
}
