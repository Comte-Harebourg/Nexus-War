class_name UnitManager extends Node3D

@onready var grid_manager: GridManager = %Grid
@export var unit_dict: Dictionary[Vector2i, Node3D]

func _ready() -> void:
	update_units()
	for unit in unit_dict.values():
		update_animation(unit, "idle", 0.33)

func update_animation(unit: Node3D, animation: String, delay: float = 0, membres: Array = []) -> void:
	#Mets à jour les animations des membres de l'unité avec l'animation renseignée et un écarte de delay en secondes
	#Met à jour tous les membres si un array n'est pas renseigné
	if membres==[]: for i in range(unit.animators.size()): membres.append(i)
	for i in membres:
		if unit.animators[i].has_animation(animation):
			unit.animators[i].play(animation)
			unit.animators[i].seek(fmod(i*delay,unit.animators[i].current_animation_length))
		else: print("Animation {anim} manquante sur le membre {int} de {unit}".format({"anim": animation, "int": str(i+1), "unit": unit.unitName}))

func update_units() -> void:
	#Récupère toutes les unités en jeu et les renseigne avec leures positions dans unit_dict
	unit_dict.clear()
	for unit in get_children():
		unit_dict[grid_manager.position_to_adress(unit.position)] = unit

func get_attack_range(unit: Unit) -> Array[Vector2i]:
	var result: Array[Vector2i] = []
	for capacity in unit.capacitie:
		if capacity.isAttack:
			result.append(Vector2i(capacity.minRange, capacity.maxRange))
	return result
