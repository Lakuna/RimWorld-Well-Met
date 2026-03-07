#if !V1_0
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompGiveHediffPatches {
	[HarmonyPatch(typeof(HediffComp_GiveHediff), "SendLetter")]
	internal static class SendLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(HediffComp_CauseMentalState __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.Pawn, ControlCategory.Letter);
	}
}
#endif
