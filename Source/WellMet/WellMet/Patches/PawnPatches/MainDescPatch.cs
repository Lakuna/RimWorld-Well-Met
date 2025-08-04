using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.MainDesc))]
	internal static class MainDescPatch {
		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly FieldInfo AgeTrackerField = AccessTools.Field(typeof(Pawn), nameof(Pawn.ageTracker));

		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo IsCreepJoinerMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));

		[HarmonyPrefix]
		private static void Prefix(Pawn __instance, ref bool writeFaction, ref bool writeGender) {
			bool basicIsKnown = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
			writeFaction = writeFaction && basicIsKnown;
			writeGender = writeGender && basicIsKnown;
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.Calls(FactionMethod) || instruction.LoadsField(AgeTrackerField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

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
			}
		}
	}
}
