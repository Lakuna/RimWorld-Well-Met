#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !V1_0
using System;
#endif
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnGearPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "TryDrawMassInfo")]
	internal static class TryDrawMassInfoPatch {
#if V1_0
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.Method(typeof(ITab_Pawn_Gear), "get_SelPawnForGear");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");
#endif

		[HarmonyPostfix]
		private static bool Prefix(ITab_Pawn_Gear __instance) =>
#if V1_0
			!(SelPawnForGearMethod.Invoke(__instance, Parameters) is Pawn pawn)
#else
			!(SelPawnForGearMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
