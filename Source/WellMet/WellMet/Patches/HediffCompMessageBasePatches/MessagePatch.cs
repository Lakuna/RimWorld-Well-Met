#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using Verse;

namespace Lakuna.WellMet.Patches.HediffCompMessageBasePatches {
	[HarmonyPatch(typeof(HediffComp_MessageBase), "Message")]
	internal static class MessagePatch {
		[HarmonyPrefix]
		private static bool Prefix(HediffComp_MessageBase __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.Pawn, ControlCategory.Message);
	}
}
