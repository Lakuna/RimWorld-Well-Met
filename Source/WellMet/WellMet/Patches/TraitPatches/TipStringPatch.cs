#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !(V1_0 || V1_1)
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
#endif
using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	internal static class TipStringPatch {
#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo GetAffectedIssuesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedIssues));

		private static readonly ConstructorInfo IssueDefListConstructor = AccessTools.Constructor(typeof(List<IssueDef>));

		private static readonly MethodInfo GetAffectedMemesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedMemes));

		private static readonly ConstructorInfo MemeDefListConstructor = AccessTools.Constructor(typeof(List<MemeDef>));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo SourceGeneField = AccessTools.Field(typeof(Trait), nameof(Trait.sourceGene));

		private static readonly MethodInfo SuppressedMethod = AccessTools.PropertyGetter(typeof(Trait), nameof(Trait.Suppressed));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(GetAffectedIssuesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, IssueDefListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(GetAffectedMemesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, MemeDefListConstructor)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (instruction.LoadsField(SourceGeneField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(SuppressedMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Advanced, getPawnInstructions)) {
						yield return i;
					}
				}
#endif
			}
		}
#endif

		[HarmonyPostfix]
		private static void Postfix(Trait __instance,
#if V1_0 || V1_1
			Pawn pawn,
#endif
	ref string __result) {
#if V1_0 || V1_1
			if (KnowledgeUtility.IsTraitKnown(pawn, __instance.def)) {
#else
			if (KnowledgeUtility.IsTraitKnown(__instance)) {
#endif
				return;
			}

			__result = MiscellaneousUtility.EndWithPeriod("Unknown".Translate().CapitalizeFirst());
		}
	}
}
