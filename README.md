![Steam Subscriptions](https://img.shields.io/steam/subscriptions/2553173153?style=for-the-badge)

# Well Met

A RimWorld mod that aims to increase mechanical difficulty via information hiding. Different categories of information can be configured to be hidden by default for different categories of pawn. Optionally, this information can be learned over time by imprisoning, enslaving, or recruiting the pawn.

## Documentation

### Pawn/Faction Types

Pawns and factions are divided into a few broad categories, which allows the user more granular control over which information is shown and hidden.

- A pawn is considered to be a **colonist** if it is a free, non-slave colonist or if it is an animal whose faction is the player's faction. Additionally, the player's faction itself is considered to be this type.
- A pawn is considered to be **player-controlled** if it isn't considered to be a **colonist** and it is controlled by the player.
- A pawn is considered to be a **prisoner** if it isn't considered to be **player-controlled** and it is a prisoner of the player's faction.
- A pawn is considered to be a **wild animal** if it isn't considered to be a **prisoner** and it is an animal.
- A pawn is considered to be **hostile** if it isn't considered to be a **wild animal** and it is hostile to the player's faction or it's dead or downed and its faction is hostile to the player's faction. Additionally, factions that are hostile to the player's faction are considered to be this type.
- A pawn is considered to be **neutral** if it isn't considered to be **hostile**. Additionally, factions that aren't considered to be **hostile** are considered to be this type. In the case of an error or unexpected circumstance, pawns and factions default to being considered this type.

### Information Categories

Information is divided into several categories which can each be enabled or disabled per pawn/faction type. If an information category is disabled for a given pawn/faction type, that information won't be shown on the UI when viewing a pawn or faction of that type. If a piece of information is hidden, controls related to that piece of information will also be hidden[^1]. If all of the information on a given tab is hidden, that entire tab will be hidden. If the information relating to a specific gizmo is hidden, the gizmo itself will also be hidden[^2].

- **Basic** information roughly corresponds to the information that a friendly person might tell you upon introducing themselves to you, such as their name, age, gender, faction, kind (i.e. "mercenary gunner"), and title. Additionally, a lot of miscellaneous information that is necessary to properly interact with the pawn or faction belongs to this category[^3].
- **Health** information is information relating to a pawn's physical health. This includes their physical statistics (i.e. consciousness and pain), hediffs (i.e. missing body parts, scars, and other injuries), general condition (i.e. "unconscious"), whether or not the pawn is a mutant, addictions, and thoughts related to any of that information.
- **Needs** information is information relating to a pawn's mental health and natural needs. This includes their mood, hunger, energy (for mechs), thoughts, mental state, and inspiration.
- **Gear** information is information relating to a pawn's physical equipment. This includes their weapons[^4], equipment, apparel, inventory, held items, and possessions.
- **Skills** information is information relating to a pawn's ability to do work. This includes their skills, passions, and what types of work they're incapable of.
- **Traits** information is information relating to a pawn's unique traits[^5].
- **Abilities** information is information relating to a pawn's psychic, genetic, and role-based (royal title and ideological role) abilities. This includes psychic entropy, neural heat, and mechanitor bandwidth and control groups.
- **Backstory** information is information relating to a pawn's childhood and adulthood.
- **Social** information is information relating to a pawn's relationships. This includes the pawn's interactions log, family, friends, enemies, bonds, acquaintances, and social thoughts.
- **Ideoligion** information is information relating to a pawn's beliefs. This includes the pawn's ideoligion and ideological role, thoughts, and certainty.
- **Advanced** information represents information that a pawn would only tell to close friends or family and "gamified" information that exists only to make the game more playable for the player. This includes a pawn's genes, xenotype, royal title (and related thoughts), favorite color, growth tier, genetic resources (i.e. deathrest and hemogen), allowed area, timetable setting, combat log, overall armor (heat, blunt, and sharp), comfortable temperature range, mass carried, average time between prison breaks and slave rebellions, resistance, will, terror, suppression, slave price, kill thirst, stance (i.e. "stunned"), job (the task that the pawn is currently carrying out), trainability, traits that are given or suppressed by genes, and the information button.

[^1]: For example, you can't operate on a pawn for which you don't know health information, and you can't rename a pawn that you don't know the name of. Controls won't be hidden if "always show controls" is enabled in the settings and the pawn is considered to be a **colonist** or **player-controlled**
[^2]: Gizmos will not be hidden for **colonist** or **player-controlled** pawns if "always show controls" is enabled in the settings.
[^3]: Other information that belongs to the **basic** category includes whether or not the player's faction considers the pawn to be guilty, the feeding, caravan, guest, prisoner, slave, and training tabs, whether or not the pawn is flying, being roped, a creep joiner, or in restraints, the kind of trader that the pawn is, the pawn's lord report (i.e. "attending party"), and the amount of time that the pawn has been dead. Factions for which basic information isn't known will be hidden on the list of factions if "apply information hiding to factions" is enabled.
[^4]: This also includes thoughts related to weapon traits and apparel.
[^5]: In addition to the traits themselves, this also hides thoughts and relationships related to those traits, and prevents the traits from being listed among the causes for a pawn being incapable of any work types.

### Settings

In addition to the ability to enable or disable information categories for pawn/faction types, there are several other options available to configure how the mod behaves. Some settings will be hidden based on whether or not they have any effect given your other settings.

- "Learning enabled" adds a new layer to information hiding by allowing information to be learned over time rather than simply "known" or "not known." If an information category is known for a pawn/faction type, learning is enabled for that pawn/faction type, and the learning difficulty for that information category is greater than zero, that information will be learned after certain conditions have been met rather than right away.
- "Trait learning difficulty" is used to set the difficulty of learning traits. This can be set to zero to disable learning for traits (which will cause traits to either be always known or never known). A trait is learned when $t>rqd$, where $t$ is the number of ticks that the pawn has been either a colonist or a prisoner, $r$ is the trait's rarity (calculated as $\frac{1}{c}$, where $c$ is the trait's gender-specific commonality for the pawn's gender), $q$ is the number of ticks in a quadrum, and $d$ is the trait learning difficulty.
- "Backstory learning difficulty" is used to set the difficulty of learning backstories. This can be set to zero to disable learning for backstories (which will cause backstories to either be always known or never known). A backstory is learned when $t>qd$, where $t$ is the number of ticks that the pawn has been either a colonist or a prisoner, $q$ is the number of ticks in a quadrum, and $d$ is the backstory learning difficulty.
- "Skills learning difficulty" is used to set the difficulty of learning skills. This can be set to zero to disable learning for skills (which will cause skills to either be always known or never known). A skill is learned when $t>oqd$, where $t$ is the number of ticks that the pawn has been either a colonist or a prisoner, $o$ is the one-based index of the skill when the pawn's skills are sorted in order of best to worst, $q$ is the number of ticks in five in-game days, and $d$ is the skills learning difficulty.
- "Enable unique trait unlock conditions" can be used to enable unique unlock conditions for certain supported traits, which override the default trait unlock condition.
- "Always know more about starting colonists" will cause all information to be shown when selecting starting colonists, regardless of any other setting. This will not apply to starting colonists after starting the game - it only applies to the "choose starting colonists" window.
- "Always know more about colonist relatives" will cause certain categories of information (basic, traits, and backstory) to be known about pawns who are related to at least one colonist, regardless of what information would otherwise be known about them.
- "Always know growth moment traits" will cause traits to always be visible in the "growth moment" window, regardless of whether or not they would otherwise be visible.
- "Always show controls" will cause UI elements that are used to control pawns to always be shown for **colonist** and **player-controlled** pawns, regardless of any other setting.
- "Always know less about ancient corpses" will cause ancient corpses (i.e. those that start on the map) to have all of their information hidden, regardless of what information would otherwise be known about them.
- "Apply information hiding to factions" will cause factions to be hidden in the list of factions if basic information isn't known for the relevant pawn/faction type.
- "Legacy mode" reverts this mod's functionality to its pre-version 3 functionality: all information categories will be known for all pawn/faction types, learning will be enabled for all pawn/faction types, learning will be disabled for all information categories except for traits, and unique trait unlock conditions will be disabled.

### Unique Trait Unlock Conditions

Some traits have optional unique unlock conditions.

- Bloodlust is unlocked if the pawn has killed at least $d$ pawns, where $d$ is the trait learning difficulty.
- Pyromaniac is unlocked if the pawn has had at least $d$ mental breaks, where $d$ is the trait learning difficulty.
- Brawler, trigger happy, and careful shooter are unlocked if the pawn has fired at least $10d$ shots, where $d$ is the trait learning difficulty.
- Wimp, tough, and masochist are unlocked if the pawn has taken at least $10d$ damage, where $d$ is the trait learning difficulty.
- Body purist and transhumanist are unlocked if the pawn has received at least $d$ operations, where $d$ is the trait learning difficulty (adult human pawns have $100$ health by default).
- Gourmand is unlocked if the pawn has eaten at least $16d$ nutrition, where $d$ is the trait learning difficulty (adult human pawns eat $1.6$ nutrition per day by default).
