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
		public static readonly HarmonyInstance Instance = HarmonyInstance.Create(nameof(WellMet));
#else
		public static readonly Harmony Instance = new Harmony(nameof(WellMet));
#endif

		static HarmonyPatcher() => Instance.PatchAll();

	}
}
