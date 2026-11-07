class_name TileDatabase extends Node

@export var tile_data: Dictionary[String, Tile]

func get_tile(type: String) -> Tile:
	if not tile_data.has(type): return null
	return tile_data[type]
