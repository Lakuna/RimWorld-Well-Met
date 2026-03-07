using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.WellMet.Utility;

using RimWorld;

namespace Lakuna.WellMet.Patches.AlertThoughtPatches {
	[HarmonyPatch(typeof(Alert_Thought), nameof(Alert_Thought.GetReport))]
	internal static class GetReportPatch {
		private static readonly MethodInfo ThoughtMethod = PatchUtility.PropertyGetter(typeof(Alert_Thought), "Thought");

		[HarmonyPostfix]
		private static void Postfix(Alert_Thought __instance, ref AlertReport __result) {
			if (WellMetMod.Settings.NeverHideAlerts || !(ThoughtMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is ThoughtDef thoughtDef)) {
				return;
			}

			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => {
				List<Thought> thoughts = new List<Thought>();
				pawn.needs.mood.thoughts.GetMoodThoughtsFor(thoughtDef, thoughts);
				return thoughts.Any((thought) => KnowledgeUtility.IsThoughtKnown(thought));
			}).ToList();
			__result.active = __result.AnyCulpritValid;
		}
	}
}
