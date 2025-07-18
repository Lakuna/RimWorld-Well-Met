﻿using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI.Group;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetInspectString))]
	internal static class GetInspectStringPatch {
		private static readonly FieldInfo HideMainDescField = AccessTools.Field(typeof(ThingDef), nameof(ThingDef.hideMainDesc));

		private static readonly MethodInfo InMentalStateMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.InMentalState));

		private static readonly MethodInfo InspiredMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Inspired));

		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo IsCreepJoinerMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));

		private static readonly MethodInfo ShouldShowRestraintsInfoMethod = SymbolExtensions.GetMethodInfo(() => RestraintsUtility.ShouldShowRestraintsInfo(null));

		private static readonly MethodInfo IsSubhumanMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSubhuman));

		private static readonly MethodInfo IsSlaveOfColonyMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSlaveOfColony));

		private static readonly MethodInfo IsPrisonerOfColonyMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsPrisonerOfColony));

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.health)), InformationCategory.Health },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.flight)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.stances)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.abilities)), InformationCategory.Abilities },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.carryTracker)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.roping)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn_NeedsTracker), nameof(Pawn_NeedsTracker.energy)), InformationCategory.Needs },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.jobs)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.guest)), InformationCategory.Advanced }
		};

		private static readonly Dictionary<MethodInfo, InformationCategory> ObfuscatedMethods = new Dictionary<MethodInfo, InformationCategory>() {
			{ AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.GetInspectString)), InformationCategory.Basic },
			{ AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.TraderKind)), InformationCategory.Basic },
			{ AccessTools.Method(typeof(LordUtility), nameof(LordUtility.GetLord), new Type[] { typeof(Pawn) }), InformationCategory.Advanced },
			{ AccessTools.Method(typeof(Pawn), nameof(Pawn.GetJobReport)), InformationCategory.Advanced },
			{ AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction)), InformationCategory.Basic }
		};

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Replace `this.def.hideMainDesc` with `this.def.hideMainDesc || !KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, this)`.
				if (instruction.LoadsField(HideMainDescField)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Basic);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.Ldc_I4_0);
					yield return new CodeInstruction(OpCodes.Ceq);
					yield return new CodeInstruction(OpCodes.Or);
				}

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

				// Replace `this.InMentalState` with `this.InMentalState && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, this)`.
				// Replace `this.Inspired` with `this.Inspired && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, this)`.
				if (instruction.Calls(InMentalStateMethod) || instruction.Calls(InspiredMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Needs);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `this.IsMutant` with `this.IsMutant && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, this)`.
				// Replace `this.IsSubhuman` with `this.IsSubhuman && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, this)`.
				if (instruction.Calls(IsMutantMethod) || instruction.Calls(IsSubhumanMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Health);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `RestraintsUtility.ShouldShowRestraintsInfo(this)` with `RestraintsUtility.ShouldShowRestraintsInfo(this) && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, this)`.
				// Replace `this.IsSlave` with `this.IsSlave && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, this)`.
				if (instruction.Calls(ShouldShowRestraintsInfoMethod) || instruction.Calls(IsSlaveOfColonyMethod) || instruction.Calls(IsPrisonerOfColonyMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Basic);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}

				// Replace `this.IsCreepJoiner` with `this.IsCreepJoiner && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, this)`.
				if (instruction.Calls(IsCreepJoinerMethod)) {
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)InformationCategory.Advanced);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.IsInformationKnownForPawnMethod);
					yield return new CodeInstruction(OpCodes.And);
				}
			}
		}
	}
}
