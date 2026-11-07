class_name UnitDatabase extends Node

@export var unit_data: Dictionary[String, Unit]

func get_unit(id: String) -> Unit:
	if not unit_data.has(id): return null
	return unit_data[id]
