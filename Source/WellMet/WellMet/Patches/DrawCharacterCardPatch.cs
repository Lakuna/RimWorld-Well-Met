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
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	public static class DrawCharacterCardPatch {
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, ref bool showName) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)) {
				return;
			}

			showName = false;
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions == null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			Queue<CodeInstruction> loadPawnStack = new Queue<CodeInstruction>();
			bool previousLoadedRoyalty = false;
			bool previousLoadedGuilt = false;
			foreach (CodeInstruction instruction in instructions) {
				if (previousLoadedRoyalty && instruction.Branches(out _)) {
					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.RoyaltyObfuscatorInfo);
				}

				if (previousLoadedGuilt && instruction.Branches(out _)) {
					while (loadPawnStack.Count > 0) {
						yield return loadPawnStack.Dequeue();
					}

					yield return new CodeInstruction(OpCodes.Call, KnowledgeUtility.GuiltObfuscatorInfo);
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

				previousLoadedRoyalty = instruction.LoadsField(KnowledgeUtility.PawnRoyaltyField);
				previousLoadedGuilt = instruction.LoadsField(KnowledgeUtility.PawnGuiltField);
			}
		}
	}
}
