using Lakuna.WellMet.Utility;
using System;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetSettings : ModSettings {
		public WellMetSettings() {
			this.knownInformation = new BoolGrid(Enum.GetValues(typeof(PawnType)).Length, Enum.GetValues(typeof(InformationCategory)).Length);
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Basic] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Health] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Needs] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Gear] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Skills] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Abilities] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Traits] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Backstory] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Social] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Ideoligion] = true;
			this.knownInformation[(int)PawnType.Colonist, (int)InformationCategory.Advanced] = true;
			this.knownInformation[(int)PawnType.Controlled, (int)InformationCategory.Basic] = true;
			this.knownInformation[(int)PawnType.Controlled, (int)InformationCategory.Health] = true;
			this.knownInformation[(int)PawnType.Controlled, (int)InformationCategory.Needs] = true;
			this.knownInformation[(int)PawnType.Controlled, (int)InformationCategory.Gear] = true;
			this.knownInformation[(int)PawnType.Controlled, (int)InformationCategory.Skills] = true;
			this.knownInformation[(int)PawnType.Controlled, (int)InformationCategory.Abilities] = true;
			this.knownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Basic] = true;
			this.knownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Health] = true;
			this.knownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Needs] = true;
			this.knownInformation[(int)PawnType.Prisoner, (int)InformationCategory.Gear] = true;
			this.knownInformation[(int)PawnType.Neutral, (int)InformationCategory.Basic] = true;

			this.colonistTraitDiscoveryDifficulty = 0;

			this.alwaysKnowStartingColonists = true;

			this.alwaysKnowGrowthMoments = true;
		}

		private BoolGrid knownInformation;

		public BoolGrid KnownInformation => this.knownInformation;

		private int colonistTraitDiscoveryDifficulty;

		public int ColonistTraitDiscoveryDifficulty {
			get => this.colonistTraitDiscoveryDifficulty;
			set => this.colonistTraitDiscoveryDifficulty = value;
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

		public override void ExposeData() {
			Scribe_Deep.Look(ref this.knownInformation, nameof(this.knownInformation));
			Scribe_Values.Look(ref this.colonistTraitDiscoveryDifficulty, nameof(this.colonistTraitDiscoveryDifficulty));
			Scribe_Values.Look(ref this.alwaysKnowStartingColonists, nameof(this.alwaysKnowStartingColonists));
			Scribe_Values.Look(ref this.alwaysKnowGrowthMoments, nameof(this.alwaysKnowGrowthMoments));
			base.ExposeData();
		}
	}
}
