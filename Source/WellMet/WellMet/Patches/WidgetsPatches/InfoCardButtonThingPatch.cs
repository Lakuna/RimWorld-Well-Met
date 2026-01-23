#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using System;
using Verse;

namespace Lakuna.WellMet.Patches.WidgetsPatches {
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Thing) })]
	internal static class InfoCardButtonThingPatch {
		[HarmonyPrefix]
		private static bool Prefix(Thing thing) => !(thing is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn, ControlCategory.Control);
	}
}
