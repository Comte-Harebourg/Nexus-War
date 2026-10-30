@tool
extends Node3D

const LEVEL_PATH: String = "res://Assets/Levels/" #Chemin du fichier des levels
@onready var tile_map: GridMap = $"../TileMap"
@onready var grid: Node3D = $"../Grid"
@export var level_index: int = 0
var level: SavedLevel
var tile = preload("res://Assets/Scenes/Skirmish/Tile.tscn")

@export_tool_button("Sauvegarder le niveau", "Save") var save_button: Callable = save_level
@export_tool_button("Charger le niveau", "Load") var load_button: Callable = load_current_level
@export_tool_button("Retirer le niveau", "Clear") var clear_button: Callable = clear_level

func _ready() -> void:
	load_level(level_index)

func _get_level_file(index: int) -> String:
	#Renvoi rapidement le chemin désiré
	return LEVEL_PATH + "Level" + str(index) + ".tres"

func _input(event: InputEvent) -> void:
	#Test de fonction
	if event.is_action("dev_test"):
		pass

func save_level() -> void:
	#Sauvegarde un level à l'index renseigné dans l'éditeur, à ne pas utiliser en dehors de l'éditeur
	level = SavedLevel.new()
	var grid_dict: Array[Dictionary] = []
	for cell: Vector3i in tile_map.get_used_cells():
		grid_dict.append({
			"id": tile_map.get_cell_item(cell),
			"position": cell,
			"rotation": tile_map.get_cell_item_orientation(cell)
		})
	level.grid_dict = grid_dict
	var path: String = _get_level_file(level_index)
	var err: Error = ResourceSaver.save(level, path)
	if err == OK:
		print("Level sauvegardé avec succès : ", path)
	else:
		push_error("Échec de la sauvegarde (code %d) : %s" % [err, path])

func load_level(index) -> void:
	#Charge le level de l'index en paramètre
	var path: String = _get_level_file(index)
	if not ResourceLoader.exists(path):
		push_warning("Level introuvable : " + path)
		return
	level = ResourceLoader.load(path) as SavedLevel
	if level:
		print("Level chargé avec succès")
		load_tilemap()

func load_current_level():
	#Fonction servant uniquement au bouton de l'éditeur
	load_level(level_index)

func load_tilemap() -> void:
	#Génère la Gridmap selon le level chargé
	tile_map.clear()
	for cell_data: Dictionary in level.grid_dict:
		tile_map.set_cell_item(
			cell_data["position"],
			cell_data["id"],
			cell_data["rotation"]
		)
		var instance = tile.instantiate()
		instance.pos = Vector2i(cell_data["position"].x, cell_data["position"].z)
		instance.position = cell_data["position"]*6.25 + Vector3(3.125,6.5,3.125)
		grid.add_child(instance)
	grid.update_grid()

func clear_level() -> void:
	#Retire la Grdimap
	tile_map.clear()
	for tile_child in $"../Grid".get_children():
		tile_child.free()
