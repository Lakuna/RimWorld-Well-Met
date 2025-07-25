using HarmonyLib;
using System;
using UnityEngine;

namespace Lakuna.WellMet.Patches.GuiPatches {
	[HarmonyPatch(typeof(GUI), nameof(GUI.DrawTexture), new Type[] { typeof(Rect), typeof(Texture), typeof(ScaleMode), typeof(bool), typeof(float), typeof(Color), typeof(Vector4), typeof(Vector4) })]
	internal static class DrawTexturePatch {
		[HarmonyPrefix]
		private static bool Prefix(Texture image) => image != null; // Don't log an error when attempting to draw a `null` texture because of another patch.
	}
}
