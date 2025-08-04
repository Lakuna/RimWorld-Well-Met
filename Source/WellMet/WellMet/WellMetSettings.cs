using Lakuna.WellMet.Utility;
using System;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetSettings : ModSettings {
		public WellMetSettings() {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5
			Map map = new Map();
			map.info.Size = new IntVec3(Enum.GetValues(typeof(PawnType)).Length, 0, Enum.GetValues(typeof(InformationCategory)).Length);
			this.knownInformation = new BoolGrid(map);
#if V1_5
			map.Dispose();
#endif
#else
			this.knownInformation = new BoolGrid(Enum.GetValues(typeof(PawnType)).Length, Enum.GetValues(typeof(InformationCategory)).Length);
#endif
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
			this.alwaysKnowGrowthMomentTraits = true;
			this.hideFactionInformation = false;
			this.neverHideControls = true;
			this.hideAncientCorpses = true;
		}

		private BoolGrid knownInformation;

		internal BoolGrid KnownInformation => this.knownInformation;

		private int colonistTraitDiscoveryDifficulty;

		internal int ColonistTraitDiscoveryDifficulty {
			get => this.colonistTraitDiscoveryDifficulty;
			set => this.colonistTraitDiscoveryDifficulty = value;
		}

		private bool alwaysKnowStartingColonists;

		internal bool AlwaysKnowStartingColonists {
			get => this.alwaysKnowStartingColonists;
			set => this.alwaysKnowStartingColonists = value;
		}

		private bool alwaysKnowGrowthMomentTraits;

		internal bool AlwaysKnowGrowthMomentTraits {
			get => this.alwaysKnowGrowthMomentTraits;
			set => this.alwaysKnowGrowthMomentTraits = value;
		}

		private bool hideFactionInformation;

		internal bool HideFactionInformation {
			get => this.hideFactionInformation;
			set => this.hideFactionInformation = value;
		}

		private bool neverHideControls;

		internal bool NeverHideControls {
			get => this.neverHideControls;
			set => this.neverHideControls = value;
		}

		private bool hideAncientCorpses;

		internal bool HideAncientCorpses {
			get => this.hideAncientCorpses;
			set => this.hideAncientCorpses = value;
		}

		public override void ExposeData() {
			base.ExposeData();
			Scribe_Deep.Look(ref this.knownInformation, nameof(this.knownInformation));
			Scribe_Values.Look(ref this.colonistTraitDiscoveryDifficulty, nameof(this.colonistTraitDiscoveryDifficulty));
			Scribe_Values.Look(ref this.alwaysKnowStartingColonists, nameof(this.alwaysKnowStartingColonists));
			Scribe_Values.Look(ref this.alwaysKnowGrowthMomentTraits, nameof(this.alwaysKnowGrowthMomentTraits));
			Scribe_Values.Look(ref this.hideFactionInformation, nameof(this.hideFactionInformation));
			Scribe_Values.Look(ref this.neverHideControls, nameof(this.neverHideControls));
			Scribe_Values.Look(ref this.hideAncientCorpses, nameof(this.hideAncientCorpses));
		}
	}
}
