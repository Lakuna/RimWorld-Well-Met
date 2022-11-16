using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.WellMet {
	public class WellMetSettings : ModSettings {
		private float difficultyFactor;

		public float DifficultyFactor {
			get => this.difficultyFactor;
			set => this.difficultyFactor = value;
		}

		private bool allTraitsDiscovered;

		public bool AllTraitsDiscovered {
			get => this.allTraitsDiscovered;
			set => this.allTraitsDiscovered = value;
		}

		private bool showTraitsOnGrowthMoment;

		public bool ShowTraitsOnGrowthMoment {
			get => this.showTraitsOnGrowthMoment;
			set => this.showTraitsOnGrowthMoment = value;
		}

		private bool alwaysShowPhysicalTraits;

		public bool AlwaysShowPhysicalTraits {
			get => this.alwaysShowPhysicalTraits;
			set => this.alwaysShowPhysicalTraits = value;
		}

		private bool showTraitsForStartingColonists;

		public bool ShowTraitsForStartingColonists {
			get => this.showTraitsForStartingColonists;
			set => this.showTraitsForStartingColonists = value;
		}

		public WellMetSettings() {
			this.DifficultyFactor = 15;
			this.allTraitsDiscovered = false;
			this.showTraitsOnGrowthMoment = false;
			this.alwaysShowPhysicalTraits = true;
			this.showTraitsForStartingColonists = false;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.difficultyFactor, nameof(this.difficultyFactor));
			Scribe_Values.Look(ref this.allTraitsDiscovered, nameof(this.allTraitsDiscovered));
			Scribe_Values.Look(ref this.showTraitsOnGrowthMoment, nameof(this.showTraitsOnGrowthMoment));
			Scribe_Values.Look(ref this.alwaysShowPhysicalTraits, nameof(this.alwaysShowPhysicalTraits));
			Scribe_Values.Look(ref this.showTraitsForStartingColonists, nameof(this.showTraitsForStartingColonists));
			base.ExposeData();
		}
	}
}
