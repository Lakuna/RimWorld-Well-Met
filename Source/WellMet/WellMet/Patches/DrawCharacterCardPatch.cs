#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	public static class DrawCharacterCardPatch {
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, ref bool showName) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)) { return; }
			showName = false;
		}

		public readonly static FieldInfo PawnRoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		public static Pawn_RoyaltyTracker RoyaltyObfuscator(Pawn_RoyaltyTracker royalty) =>
			royalty != null && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, royalty.pawn) ? royalty : null;

		public readonly static MethodInfo RoyaltyObfuscatorMethod = SymbolExtensions.GetMethodInfo((Pawn_RoyaltyTracker royalty) => RoyaltyObfuscator(royalty));

		public readonly static FieldInfo PawnGuiltField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt));

		public static Pawn_GuiltTracker GuiltObfuscator(Pawn_GuiltTracker guilt) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, PawnType.Colonist) ? guilt : null;

		public readonly static MethodInfo GuiltObfuscatorMethod = SymbolExtensions.GetMethodInfo((Pawn_GuiltTracker royalty) => GuiltObfuscator(royalty));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions == null) { throw new ArgumentNullException(nameof(instructions)); }

			bool previousLoadedRoyalty = false;
			bool previousLoadedGuilt = false;
			foreach (CodeInstruction instruction in instructions) {
				if (previousLoadedRoyalty && instruction.Branches(out _)) {
					yield return new CodeInstruction(OpCodes.Call, RoyaltyObfuscatorMethod);
				}

				if (previousLoadedGuilt && instruction.Branches(out _)) {
					yield return new CodeInstruction(OpCodes.Call, GuiltObfuscatorMethod);
				}

				yield return instruction;

				previousLoadedRoyalty = instruction.LoadsField(PawnRoyaltyField);
				previousLoadedGuilt = instruction.LoadsField(PawnGuiltField);
			}
		}
	}
}
