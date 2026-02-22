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
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "TryDrawComfyTemperatureRange")]
	internal static class TryDrawComfyTemperatureRangePatch {
		private static readonly MethodInfo SelPawnForGearMethod = PatchUtility.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");

		[HarmonyPrefix]
#pragma warning disable CA1707
		private static bool Prefix(ITab_Pawn_Gear __instance) =>
#pragma warning restore CA1707
			!(SelPawnForGearMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
	}
}
