using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using UnityEngine;

namespace Lakuna.WellMet.Patches.SkillUiPatches {
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkill), new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) })]
	internal static class DrawSkillPatch {
		[HarmonyPrefix]
		private static bool Prefix(SkillRecord skill) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, skill.Pawn);
	}
}
