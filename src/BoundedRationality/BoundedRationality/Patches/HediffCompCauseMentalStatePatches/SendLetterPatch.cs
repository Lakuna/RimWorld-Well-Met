#if !V1_0
using HarmonyLib;

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.HediffCompCauseMentalStatePatches {
	[HarmonyPatch(typeof(HediffComp_CauseMentalState), "SendLetter")]
	internal static class SendLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(HediffComp_CauseMentalState __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.Pawn, ControlCategory.Letter);
	}
}
#endif
