#if V1_0
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
	[HarmonyPatch(typeof(ITab_Pawn_Gear), nameof(ITab_Pawn_Gear.IsVisible), MethodType.Getter)]
	internal static class IsVisiblePatch {
#if V1_0
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.Method(typeof(ITab_Pawn_Gear), "get_SelPawnForGear");

		internal static readonly object[] Parameters = new object[] { };
#else
		private static readonly MethodInfo SelPawnForGearMethod = AccessTools.PropertyGetter(typeof(ITab_Pawn_Gear), "SelPawnForGear");
#endif

		[HarmonyPostfix]
		private static void Postfix(ITab_Pawn_Gear __instance, ref bool __result) => __result = __result
#if V1_0
			&& (!(SelPawnForGearMethod.Invoke(__instance, Parameters) is Pawn pawn)
#else
			&& (!(SelPawnForGearMethod.Invoke(__instance, Array.Empty<object>()) is Pawn pawn)
#endif
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, true));
	}
}
#endif
