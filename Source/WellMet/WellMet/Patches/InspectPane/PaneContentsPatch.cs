using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPane {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetInspectString))]
	internal static class PaneContentsPatch {
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
			foreach (CodeInstruction instruction in instructions) {
				// TODO

				yield return instruction;
			}
		}
	}
}
