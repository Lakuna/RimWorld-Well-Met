using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellMet.Patches {
	[HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
	public class CharacterCardPatch {
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			// TUTORIAL: https://harmony.pardeike.net/articles/patching-transpiler.html
			return instructions; // TODO
		}
	}
}
