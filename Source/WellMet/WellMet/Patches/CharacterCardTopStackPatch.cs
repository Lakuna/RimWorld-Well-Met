#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(CharacterCardUtility), "DoTopStack")]
	public static class CharacterCardTopStackPatch {
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, ref bool creationMode) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)) { return; }
			creationMode = false; // `creationMode` is used only to determine whether the gender icon and initial faction should be shown.
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions == null) { throw new ArgumentNullException(nameof(instructions)); }

			// TODO: Remove main description label and corresponding age tooltip.

			// TODO: Clear `tmpExtraFactions` after calling `QuestUtility.GetExtraFactionsFromQuestParts` and `GuestUtility.GetExtraFactionsFromGuestStatus` if factions are unknown.

			// TODO: Hide `pawn.Ideo` if ideologies are unknown.

			// TODO: Hide `pawn.Ideo.GetRole` if ideologies are unknown.

			// TODO: Hide `pawn.royalty` if titles are unknown.

			// TODO: Hide `pawn.story` if backstories are unknown.

			// TODO: Hide `pawn.guest` if guest information is unknown.

			Queue<CodeInstruction> loadPawnStack = new Queue<CodeInstruction>();
			bool previousLoadedGenes = false;
			bool previousCalledFactionGetter = false;
			foreach (CodeInstruction instruction in instructions) {
				if (previousLoadedGenes && instruction.Branches(out _)) {
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.GeneObfuscatorInfo);
				}

				if (previousCalledFactionGetter && instruction.Branches(out _)) {
					while (loadPawnStack.Count > 0) {
						yield return loadPawnStack.Dequeue();
					}

					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.FactionObfuscatorInfo);
				}

				yield return instruction;

				if (instruction.IsLdloc()) {
					loadPawnStack.Clear();
					loadPawnStack.Enqueue(instruction);
				} else if (loadPawnStack.Count == 1 && instruction.opcode == OpCodes.Ldfld) {
					loadPawnStack.Enqueue(instruction);
				} else {
					loadPawnStack.Clear();
				}

				previousLoadedGenes = instruction.LoadsField(KnowledgeUtility.PawnGenesField);
				previousCalledFactionGetter = instruction.Calls(KnowledgeUtility.PawnFactionGetterInfo);
			}
		}
	}
}
