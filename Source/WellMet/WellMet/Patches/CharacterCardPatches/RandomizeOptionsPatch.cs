using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "LifestageAndXenotypeOptions")]
	internal static class RandomizeOptionsPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn); // Hide the lifestage and xenotype options when the pawn's name is hidden since it will be unusable anyway.
	}
}
