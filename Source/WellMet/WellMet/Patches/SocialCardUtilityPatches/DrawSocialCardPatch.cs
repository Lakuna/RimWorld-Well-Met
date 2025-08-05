#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !V1_0
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
#endif
using Verse;

namespace Lakuna.WellMet.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawSocialCard))]
	internal static class DrawSocialCardPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn, true) // Role selection dropdown.
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn, true); // Romance button.

#if !(V1_0 || V1_1)
		private static readonly MethodInfo IdeoMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(IdeoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, isControl: true)) { // Ideoligion contains role selection dropdown.
						yield return i;
					}
				}
			}
		}
#endif
	}
}
