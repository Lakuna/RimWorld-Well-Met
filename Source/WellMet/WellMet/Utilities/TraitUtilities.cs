using RimWorld;
#if !(V1_0 || V1_1)
using System;
#endif
using Verse;

namespace Lakuna.WellMet.Utilities {
	public static class TraitUtilities {
		private const int TicksPerDay = 60000;

#if !(V1_0 || V1_1)
		public static bool TraitIsDiscovered(Trait trait) => trait == null
			? throw new ArgumentNullException(nameof(trait))
			: TraitIsDiscovered(trait.pawn, trait.def);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		public static bool TraitIsDiscovered(Pawn pawn, TraitDef def) => WellMetMod.Settings.AllTraitsDiscovered
			|| (pawn == null
			? WellMetMod.Settings.ShowTraitsOnGrowthMoment // Pawn is only null on growth moments in vanilla RimWorld.
			: def == null // Causes some issues with pawns that cannot gain traits otherwise.
			|| WellMetMod.Settings.ShowTraitsForStartingColonists
			&& pawn.IsColonist
			&& pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) == 0
			|| (def == TraitDefOf.Bloodlust
			? pawn.records.GetValue(RecordDefOf.Kills) > 0
			: def == TraitDefOf.BodyPurist
			|| def == TraitDefOf.Transhumanist
			? pawn.records.GetValue(RecordDefOf.OperationsReceived) > 0
			: def == TraitDefOf.Brawler
			|| def == TraitDefOf.ShootingAccuracy
			? pawn.records.GetValue(RecordDefOf.ShotsFired) > 0
			: def == TraitDefOf.Tough
#if !(V1_0 || V1_1 || V1_2)
			|| def == TraitDefOf.Masochist
			|| def == TraitDefOf.Wimp
#endif
			? pawn.records.GetValue(RecordDefOf.DamageTaken) > 0
			: def == TraitDefOf.NaturalMood
			|| def == TraitDefOf.Nerves
			|| def == TraitDefOf.TooSmart
			|| def == TraitDefOf.Pyromaniac
			? pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 0
			: def == TraitDefOf.Nudist
			? pawn.records.GetValue(RecordDefOf.BodiesStripped) > 0
			: WellMetMod.Settings.AlwaysShowPhysicalTraits
			&& (def == TraitDefOf.AnnoyingVoice
			|| def == TraitDefOf.Beauty
			|| def == TraitDefOf.CreepyBreathing)
			|| pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal)
			> def.GetGenderSpecificCommonality(pawn.gender) * TicksPerDay * WellMetMod.Settings.DifficultyFactor));
#else
		public static bool TraitIsDiscovered(Pawn pawn, TraitDef def) => WellMetMod.Settings.AllTraitsDiscovered
			|| (pawn == null // In vanilla RimWorld, the pawn is only null during a growth moment.
			? WellMetMod.Settings.ShowTraitsOnGrowthMoment
			: def == null // Fixes some issues with pawns that cannot gain traits otherwise.
			|| WellMetMod.Settings.ShowTraitsForStartingColonists
			&& pawn.IsColonist
			&& pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) <= 0
			|| (def == TraitDefOf.Bloodlust
			? pawn.records.GetValue(RecordDefOf.Kills) > 0
			: def == TraitDefOf.BodyPurist
			|| def == TraitDefOf.Transhumanist
			? pawn.records.GetValue(RecordDefOf.OperationsReceived) > 0
			: def == TraitDefOf.Brawler
			? pawn.records.GetValue(RecordDefOf.ShotsFired) > 0
			: def == TraitDefOf.Wimp
			? pawn.records.GetValue(RecordDefOf.DamageTaken) > 0
			: def == TraitDefOf.Pyromaniac
			? pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 0
			: def == TraitDefOf.Nudist
			? pawn.records.GetValue(RecordDefOf.BodiesStripped) > 0
			: WellMetMod.Settings.AlwaysShowPhysicalTraits
			&& (def == TraitDefOf.AnnoyingVoice
			|| def == TraitDefOf.CreepyBreathing)
			|| pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal)
			> def.GetGenderSpecificCommonality(pawn.gender) * TicksPerDay * WellMetMod.Settings.DifficultyFactor));
#endif
	}
}
