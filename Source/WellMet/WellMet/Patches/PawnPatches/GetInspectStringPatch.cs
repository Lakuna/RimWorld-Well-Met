using HarmonyLib;
using Lakuna.WellMet.Utility;
#if !(V1_0 || V1_1 || V1_2 || V1_3)
using RimWorld;
#endif
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetInspectString))]
	internal static class GetInspectStringPatch {
#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly FieldInfo HideMainDescField = AccessTools.Field(typeof(ThingDef), nameof(ThingDef.hideMainDesc));
#endif

		private static readonly Dictionary<FieldInfo, InformationCategory> ObfuscatedFields = new Dictionary<FieldInfo, InformationCategory>() {
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.health)), InformationCategory.Health },
#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5)
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.flight)), InformationCategory.Basic },
#endif
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.stances)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment)), InformationCategory.Gear },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.abilities)), InformationCategory.Abilities },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.carryTracker)), InformationCategory.Gear },
#if !(V1_0 || V1_1 || V1_2)
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.roping)), InformationCategory.Basic },
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3)
			{ AccessTools.Field(typeof(Pawn_NeedsTracker), nameof(Pawn_NeedsTracker.energy)), InformationCategory.Needs },
#endif
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.jobs)), InformationCategory.Advanced },
			{ AccessTools.Field(typeof(Pawn), nameof(Pawn.guest)), InformationCategory.Advanced }
		};

		private static readonly MethodInfo GetInspectStringMethod = AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.GetInspectString)); // Used for stuff like egg progress.

		private static readonly MethodInfo TraderKindMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.TraderKind));

		private static readonly MethodInfo InMentalStateMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.InMentalState));

		private static readonly MethodInfo InspiredMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Inspired));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo IsMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				if (instruction.LoadsField(HideMainDescField)) {
					foreach (CodeInstruction i in PatchUtility.OrPawnNotKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif

				bool flag = false;
				foreach (KeyValuePair<FieldInfo, InformationCategory> row in ObfuscatedFields) {
					if (instruction.LoadsField(row.Key)) {
						foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(row.Value, getPawnInstructions, generator)) {
							yield return i;
						}

						flag = true;
						break;
					}
				}
				if (flag) {
					continue;
				}

				if (instruction.Calls(GetInspectStringMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(TraderKindMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(InMentalStateMethod) || instruction.Calls(InspiredMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				if (instruction.Calls(IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}
				}
#endif
			}
		}
	}
}
