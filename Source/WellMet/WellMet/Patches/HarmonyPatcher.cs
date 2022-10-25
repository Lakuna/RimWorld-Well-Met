#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Verse;

namespace Lakuna.WellMet.Patches {
	[StaticConstructorOnStartup]
	public static class HarmonyPatcher {
		static HarmonyPatcher() =>
#if V1_0
			HarmonyInstance.Create(nameof(WellMet)).PatchAll();
#else
			new Harmony(nameof(WellMet)).PatchAll();
#endif
	}
}
