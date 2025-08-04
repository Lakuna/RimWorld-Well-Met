#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "TryDrawMassInfo")]
	internal static class TryDrawMassInfoPatch {
		private static readonly MethodInfo SelPawnForGearMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPrefix]
		private static bool Prefix(ITab_Pawn_Gear __instance) => !(SelPawnForGearMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
