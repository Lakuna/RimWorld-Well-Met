#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.MainDesc))]
	internal static class MainDescPatch {
		private static readonly MethodInfo FactionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly FieldInfo AgeTrackerField = AccessTools.Field(typeof(Pawn), nameof(Pawn.ageTracker));

#if !V1_0
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo IsCreepJoinerMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));
#endif

		[HarmonyPrefix]
		private static void Prefix(Pawn __instance,
#if V1_0
			ref bool writeAge
#else
			ref bool writeFaction, ref bool writeGender
#endif
			) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
#if V1_0
			writeAge = writeAge && basic;
#else
			writeFaction = writeFaction && basic;
			writeGender = writeGender && basic;
#endif
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, FactionMethod) || PatchUtility.LoadsField(instruction, AgeTrackerField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !V1_0
				if (instruction.Calls(IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(IsCreepJoinerMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
