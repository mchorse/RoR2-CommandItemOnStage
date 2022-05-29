# CommandItemOnStage Risk of Rain 2 mod

**CommandItemOnStage** is a Risk of Rain 2 mod which adds an ability to spawn command item bubbles on every stage. The purpose of this mod is to make runs easier, and to lessen random's impact on the run. And yes, this is certainly not a QoL but rather a *cheat mod*.

I didn't test it in multiplayer, but it should drop command item(s) for every player. When enabled, command items are also dropped in Bazaar Between Time and, theoretically, after Dio respawn due to the way it's hooked.

Made and tested for RoR2 `1.2.4`.

## Changelog

**1.0.0**

* Added main mechanic (of spawning [command items](https://riskofrain2.fandom.com/wiki/Artifacts#Command) upon)
* Added `reload_cios_config` [console command](https://riskofrain2.fandom.com/wiki/Developer_Console)
* Added basic config options:
	* Enabled - allows to toggle this mechanic
	* Void chance - what is the chance of getting a void variant of an item (0% - 100%)
	* Item count - allows to pick how many items per stage a player gets
	* Randomize item count - allows to enable randomization of item count (basically for every stage it would drop between 1 and *Item count* command items)
	* Equipment - whether command equipment should be dropped as well