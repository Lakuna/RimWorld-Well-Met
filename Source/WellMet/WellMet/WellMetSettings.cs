using Lakuna.WellMet.Utility;
using System;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetSettings : ModSettings {
		private BooleanMatrix visibleInformation;

		public BooleanMatrix KnownInformation => this.visibleInformation;

		private int discoverSpeedFactor;

		public int TraitDiscoverSpeedFactor {
			get => this.discoverSpeedFactor;
			set => this.discoverSpeedFactor = value;
		}

		private bool alwaysKnowStartingColonists;

		public bool AlwaysKnowStartingColonists {
			get => this.alwaysKnowStartingColonists;
			set => this.alwaysKnowStartingColonists = value;
		}

		private bool alwaysKnowGrowthMoments;

		public bool AlwaysKnowGrowthMoments {
			get => this.alwaysKnowGrowthMoments;
			set => this.alwaysKnowGrowthMoments = value;
		}

		public WellMetSettings() {
			this.visibleInformation = new BooleanMatrix(Enum.GetValues(typeof(PawnType)).Length, Enum.GetValues(typeof(InformationCategory)).Length);
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Gear] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Ideoligion] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Social] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Basic] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Advanced] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Backstory] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Traits] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Abilities] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Skills] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Needs] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Mood] = true;
			this.visibleInformation[(int)PawnType.Colonist, (int)InformationCategory.Health] = true;
			this.visibleInformation[(int)PawnType.Prisoner, (int)InformationCategory.Gear] = true;
			this.visibleInformation[(int)PawnType.Prisoner, (int)InformationCategory.Basic] = true;
			this.visibleInformation[(int)PawnType.Prisoner, (int)InformationCategory.Needs] = true;
			this.visibleInformation[(int)PawnType.Prisoner, (int)InformationCategory.Health] = true;
			this.visibleInformation[(int)PawnType.Slave, (int)InformationCategory.Gear] = true;
			this.visibleInformation[(int)PawnType.Slave, (int)InformationCategory.Basic] = true;
			this.visibleInformation[(int)PawnType.Slave, (int)InformationCategory.Skills] = true;
			this.visibleInformation[(int)PawnType.Slave, (int)InformationCategory.Needs] = true;
			this.visibleInformation[(int)PawnType.Slave, (int)InformationCategory.Health] = true;
			this.visibleInformation[(int)PawnType.Other, (int)InformationCategory.Basic] = true;

			this.discoverSpeedFactor = 0;

			this.alwaysKnowStartingColonists = true;

			this.alwaysKnowGrowthMoments = true;
		}

		public override void ExposeData() {
			Scribe_Deep.Look(ref this.visibleInformation, nameof(this.visibleInformation));
			Scribe_Values.Look(ref this.discoverSpeedFactor, nameof(this.discoverSpeedFactor));
			Scribe_Values.Look(ref this.alwaysKnowStartingColonists, nameof(this.alwaysKnowStartingColonists));
			Scribe_Values.Look(ref this.alwaysKnowGrowthMoments, nameof(this.alwaysKnowGrowthMoments));
			base.ExposeData();
		}
	}
}
