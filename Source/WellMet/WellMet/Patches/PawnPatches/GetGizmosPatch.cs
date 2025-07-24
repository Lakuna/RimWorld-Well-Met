using HarmonyLib;
using Lakuna.WellMet.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI.Group;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
	internal static class GetGizmosPatch {
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo IsCreepJoinerMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));

		private static readonly MethodInfo IsMechanitorMethod = AccessTools.Method(typeof(MechanitorUtility), nameof(MechanitorUtility.IsMechanitor));

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.abilities)), InformationCategory.Abilities },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.carryTracker)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.needs)), InformationCategory.Needs },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.psychicEntropy)), InformationCategory.Abilities },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.apparel)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.inventory)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.mindState)), InformationCategory.Needs },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.connections)), InformationCategory.Basic },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.genes)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.training)), InformationCategory.Basic }
		};

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.Method(typeof(LordUtility), nameof(LordUtility.GetLord), new Type[] { typeof(Pawn) }), InformationCategory.Advanced }
		};

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
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
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
						yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
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
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `this.IsCreepJoiner` with `this.IsCreepJoiner && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, this)`.
				if (instruction.Calls(IsCreepJoinerMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Basic);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `MechanitorUtility.IsMechanitor(this)` with `MechanitorUtility.IsMechanitor(this) && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, this)`.
				if (instruction.Calls(IsMechanitorMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Abilities);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}
			}
		}
	}
}
