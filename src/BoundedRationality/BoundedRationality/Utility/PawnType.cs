namespace Lakuna.BoundedRationality.Utility {
	/// <summary>
	/// Categories of pawn (or faction) that can have their visibility configured in the mod settings.
	/// </summary>
	public enum PawnType : int {
		/// <summary>
		/// A member of the player's faction (including tamed animals), a corpse of such a pawn, or the player's faction itself.
		/// </summary>
		Colonist,

		/// <summary>
		/// A pawn that is not a member nor a slave of the player's faction but is controllable by the player, such as temporary workers.
		/// </summary>
		Controlled,

		/// <summary>
		/// A slave of the player's faction.
		/// </summary>
		Slave,

		/// <summary>
		/// A prisoner of the player's faction.
		/// </summary>
		Prisoner,

		/// <summary>
		/// A pawn that is not a member of the player's faction, not controllable by the player, and not a prisoner of the player's faction, but is not hostile to the player's faction, a corpse that belongs to a pawn whose faction is not hostile to the player's faction, or a faction that is not hostile to the player's faction.
		/// </summary>
		Neutral,

		/// <summary>
		/// A pawn that is not a member of the player's faction, not controllable by the player, and not a prisoner of the player's faction, but is hostile to the player's faction, a corpse that belongs to a pawn whose faction is hostile to the player's faction, or a faction that is hostile to the player's faction.
		/// </summary>
		Hostile,

		/// <summary>
		/// A wild (non-colonist) animal.
		/// </summary>
		WildAnimal
	}
}
