using HarmonyLib;
using Verse;

namespace WellMet.Patches {
	[StaticConstructorOnStartup]
	public class HarmonyPatcher {
		static HarmonyPatcher() => new Harmony("Lakuna.WellMet").PatchAll();
	}
}
