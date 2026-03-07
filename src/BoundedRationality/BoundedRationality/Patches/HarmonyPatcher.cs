#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Verse;

namespace Lakuna.BoundedRationality.Patches {
	[StaticConstructorOnStartup]
	internal static class HarmonyPatcher {
#if V1_0
		internal static readonly HarmonyInstance Instance = HarmonyInstance.Create(nameof(BoundedRationality));
#else
		internal static readonly Harmony Instance = new Harmony(nameof(BoundedRationality));
#endif

		static HarmonyPatcher() => Instance.PatchAll();
	}
}
