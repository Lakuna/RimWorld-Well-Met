#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using Verse;
using Verse.AI;

namespace Lakuna.WellMet.Patches.MentalBreakWorkerPatches {
	[HarmonyPatch(typeof(MentalBreakWorker), "TrySendLetter")]
	internal static class TrySendLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn, ControlCategory.Letter);
	}
}
