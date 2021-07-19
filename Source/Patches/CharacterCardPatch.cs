using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace WellMet.Patches {
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	public class CharacterCardPatch {
		public static CodeInstruction filterDiscoveredInstruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WellMet), nameof(WellMet.FilterDiscovered)));

		// TODO: Alternatively, it's possible to add a postfix to the Trait.Label getter.
		// TODO: Make sure that works with colors (i.e. trait rarity colors).
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (instruction.opcode == OpCodes.Ldfld && (FieldInfo) instruction.operand == AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits))) {
					// Whenever allTraits is loaded, make a .Where() call to take out traits that haven't been discovered yet.
					yield return filterDiscoveredInstruction;
				}
			}
		}
	}
}
