class_name FactionDatabase extends Node

@export var Faction_data: Dictionary[String, Faction]

func get_faction_colors(faction: String) -> Array[Color]:
	if not Faction_data.has(faction): return [Color(0,0,0,0),Color(0,0,0,0)]
	return [Faction_data[faction].PrimaryColor, Faction_data[faction].SecondaryColor]
