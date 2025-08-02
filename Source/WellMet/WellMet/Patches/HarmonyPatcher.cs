#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Verse;

namespace Lakuna.WellMet.Patches {
	[StaticConstructorOnStartup]
	internal static class HarmonyPatcher {
#if V1_0
		internal static readonly HarmonyInstance Instance = HarmonyInstance.Create(nameof(WellMet));
#else
		internal static readonly Harmony Instance = new Harmony(nameof(WellMet));
#endif

		static HarmonyPatcher() => Instance.PatchAll();
	}
}
