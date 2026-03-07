#if !V1_0
using HarmonyLib;

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.HediffCompGiveHediffPatches {
	[HarmonyPatch(typeof(HediffComp_GiveHediff), "SendLetter")]
	internal static class SendLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(HediffComp_CauseMentalState __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.Pawn, ControlCategory.Letter);
	}
}
#endif
