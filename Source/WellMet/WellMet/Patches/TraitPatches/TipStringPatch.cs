using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	internal static class TipStringPatch {
		private static readonly MethodInfo GetAffectedIssuesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedIssues));

		private static readonly ConstructorInfo IssueDefListConstructor = AccessTools.Constructor(typeof(List<IssueDef>));

		private static readonly MethodInfo GetAffectedMemesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedMemes));

		private static readonly ConstructorInfo MemeDefListConstructor = AccessTools.Constructor(typeof(List<MemeDef>));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(GetAffectedIssuesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, IssueDefListConstructor)) {
						yield return i;
					}
				}

				if (instruction.Calls(GetAffectedMemesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, MemeDefListConstructor)) {
						yield return i;
					}
				}
			}
		}

		[HarmonyPostfix]
		private static void Postfix(Trait __instance, ref string __result) {
			if (KnowledgeUtility.IsTraitKnown(__instance)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst().EndWithPeriod();
		}
	}
}
