using HarmonyLib;
using Lakuna.WellMet.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.MainDesc))]
	internal static class MainDescPatch {
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo IsCreepJoinerMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.ageTracker)), InformationCategory.Basic }
		};

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction)), InformationCategory.Basic }
		};

		[HarmonyPrefix]
		private static void Prefix(Pawn __instance, ref bool writeFaction, ref bool writeGender) {
			bool basicIsKnown = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
			writeFaction = writeFaction && basicIsKnown;
			writeGender = writeGender && basicIsKnown;
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Replace fields with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (instruction.LoadsField(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}

				// Replace method results with `null` if they are locked behind an information category that the user has disabled.
				foreach (KeyValuePair<MethodInfo, InformationCategory> row in ObfuscatedMethods) {
					if (instruction.Calls(row.Key)) {
						Label dontNullifyLabel = generator.DefineLabel();
						yield return new CodeInstruction(OpCodes.Ldc_I4, (int)row.Value);
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForMethod);
						yield return new CodeInstruction(OpCodes.Brtrue_S, dontNullifyLabel);
						yield return new CodeInstruction(OpCodes.Pop);
						yield return new CodeInstruction(OpCodes.Ldnull);
						CodeInstruction dontNullifyTarget = new CodeInstruction(OpCodes.Nop);
						dontNullifyTarget.labels.Add(dontNullifyLabel);
						yield return dontNullifyTarget;
					}
				}

				// Replace `this.IsMutant` with `this.IsMutant && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, this)`.
				if (instruction.Calls(IsMutantMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Health);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `this.IsCreepJoiner` with `this.IsCreepJoiner && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, this)`.
				if (instruction.Calls(IsCreepJoinerMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Advanced);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForMethod);
					yield return new CodeInstruction(OpCodes.And);
				}
			}
		}
	}
}
