using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.GenLabelPatches {
	[HarmonyPatch(typeof(GenLabel), nameof(GenLabel.ThingLabel), new Type[] { typeof(Thing), typeof(int), typeof(bool), typeof(bool) })]
	internal static class ThingLabelPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Thing t, ref string __result) {
#pragma warning restore CA1707
			if (!(t is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate();
		}
	}
}
