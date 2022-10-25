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

		public WellMetSettings() {
			this.DifficultyFactor = 15;
			this.allTraitsDiscovered = false;
		}

		public override void ExposeData() {
			Scribe_Values.Look(ref this.difficultyFactor, nameof(this.difficultyFactor));
			Scribe_Values.Look(ref this.allTraitsDiscovered, nameof(this.allTraitsDiscovered));
			base.ExposeData();
		}
	}
}
