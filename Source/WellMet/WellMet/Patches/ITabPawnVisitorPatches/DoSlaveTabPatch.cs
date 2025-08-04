#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnVisitorPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Visitor), "DoSlaveTab")]
	internal static class DoSlaveTabPatch {
#if V1_0
		private static readonly MethodInfo SelPawnMethod = AccessTools.Method(typeof(ITab), "get_SelPawn");

		private static readonly MethodInfo CurLevelMethod = AccessTools.Method(typeof(Need), "get_" + nameof(Need.CurLevel));
#else
		private static readonly MethodInfo SelPawnMethod = AccessTools.PropertyGetter(typeof(ITab), "SelPawn");

		private static readonly MethodInfo CurLevelMethod = AccessTools.PropertyGetter(typeof(Need), nameof(Need.CurLevel));
#endif

		private static readonly MethodInfo GetStatValueMethod = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValue));

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo GetTerrorThoughtsMethod = AccessTools.Method(typeof(TerrorUtility), nameof(TerrorUtility.GetTerrorThoughts));

		private static readonly ConstructorInfo ThoughtMemoryObservationTerrorListConstructor = AccessTools.Constructor(typeof(List<Thought_MemoryObservationTerror>));

		private static readonly MethodInfo InitiateSlaveRebellionMtbDaysMethod = AccessTools.Method(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.InitiateSlaveRebellionMtbDays));
#endif

#if V1_0
		private static readonly MethodInfo FactionMethod = AccessTools.Method(typeof(Pawn), "get_" + nameof(Pawn.Faction));
#else
		private static readonly MethodInfo FactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Faction));
#endif

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo SlaveFactionMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.SlaveFaction));
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Callvirt, SelPawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (
#if V1_0
					PatchUtility.Calls(instruction, CurLevelMethod) || PatchUtility.Calls(instruction, GetStatValueMethod)
#else
					instruction.Calls(CurLevelMethod) || instruction.Calls(GetStatValueMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2)
				if (instruction.Calls(GetTerrorThoughtsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, ThoughtMemoryObservationTerrorListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (instruction.Calls(InitiateSlaveRebellionMtbDaysMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator, -1f)) {
						yield return i;
					}

					continue;
				}
#endif

				if (
#if V1_0
					PatchUtility.Calls(instruction, FactionMethod)
#else
					instruction.Calls(FactionMethod)
#endif
#if !(V1_0 || V1_1 || V1_2)
					|| instruction.Calls(SlaveFactionMethod)
#endif
					) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Advanced, getPawnInstructions, generator)) {
						yield return i;
					}
				}
			}
		}
	}
}
