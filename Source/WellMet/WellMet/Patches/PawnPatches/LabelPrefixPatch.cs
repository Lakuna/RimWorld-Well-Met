using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), "LabelPrefix", MethodType.Getter)]
	internal static class LabelPrefixPatch {
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Replace `this.IsMutant` with `this.IsMutant && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, this)`.
				if (instruction.Calls(IsMutantMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Health);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForMethod);
					yield return new CodeInstruction(OpCodes.And);
				}
			}
		}
	}
}
