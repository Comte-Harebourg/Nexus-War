extends Node3D

@onready var camera: Camera3D = %Camera
@onready var tile_map: GridMap = %TileMap
@onready var cursor: AnimatedSprite3D = %Cursor
@onready var grid_shader: MeshInstance3D = %GridShader
@onready var minimap: TextureRect = %Minimap
@onready var tile_colors: Dictionary = {"occupied_attack": Color.RED, "attack": Color.DARK_RED, #Détermine les couleurs de surlignage
										"move": Color.BLUE, "occupied_move": Color.DARK_BLUE, #a rajouter dans les paramètres
										"danger": Color.ORANGE, "attack_danger": Color.DARK_ORANGE,
										"interact": Color.GREEN, "occupied_interact": Color.DARK_GREEN}
var grid_size: Vector2i
var _data_image: Image
var _data_texture: ImageTexture
var _shader_material: ShaderMaterial
#var grid_data: Dictionary
#var selected_tile: Tile

func _ready() -> void:
	grid_size = get_gridmap_dimensions(tile_map) #Peut bugger si la GridMap est vide, à tester
	grid_shader.scale = Vector3(grid_size.x,1,grid_size.y)
	_setup()
	minimap.texture = _data_texture

#func update_grid() -> void:
#	#Enregistre toute les tuiles enfants dans un dictionnaire
#	grid_data.clear()
#	for child in get_children():
#		grid_data[child.pos]=child
#		child.grid=self
#	pass

#func select_tile(Tile) -> void:
#	#Actualise la tuile sélectionnée, est appelée depuis la tuile
#	if selected_tile: selected_tile.unselect()
#	selected_tile = Tile
#	print("Tuile sélectionnée", selected_tile.pos)

func _setup() -> void:
	_data_image = Image.create_empty(grid_size.x, grid_size.y, false, Image.FORMAT_RGB8)
	_data_texture = ImageTexture.create_from_image(_data_image)
	_shader_material = grid_shader.get_active_material(0) as ShaderMaterial
	_shader_material.set_shader_parameter("color_data_map", _data_texture)
	_shader_material.set_shader_parameter("map_size", grid_size)

func set_tile_color(address: Vector2i, color: Color) -> void:
	var text_x: int = address.x + int(grid_size.x/2.0)
	var text_y: int = address.y + int(grid_size.y/2.0)
	if text_x>=0 and text_y>=0 and text_x<grid_size.x and text_y<grid_size.y:
		_data_image.set_pixel(text_x, text_y, color)

func _input(event: InputEvent) -> void:
	#Retourne à l'étape de sélection précédente
	if event.is_action("right_mouse_click") and event.is_pressed():
		set_tile_color(get_tile_adress_under_mouse(), tile_colors["move"])
		_data_texture.update(_data_image)
#		if selected_tile:
#			selected_tile.unselect()
#			selected_tile = null
	if event.is_action("left_mouse_click") and event.is_pressed():
		cursor.position = adress_to_position(get_tile_adress_under_mouse()) + Vector3(3.125,0.125,3.125)
		set_tile_color(get_tile_adress_under_mouse(), tile_colors["occupied_attack"])
		_data_texture.update(_data_image)
		
func get_tile_adress_under_mouse() -> Vector2i:
	#Tire un Rayon depuis la caméra pour récupérer la tuile en dessous de la souris
	#renvoie la position si elle existe sinon Vector2i.MIN
	var space_state: PhysicsDirectSpaceState3D = get_world_3d().direct_space_state
	var mouse_pos: Vector2 = get_viewport().get_mouse_position()
	var origin: Vector3 = camera.project_ray_origin(mouse_pos)
	var end: Vector3 = origin + camera.project_ray_normal(mouse_pos) * 1000
	var query: PhysicsRayQueryParameters3D = PhysicsRayQueryParameters3D.create(origin, end)
	query.collision_mask = %Collision.collision_layer #À changer lorsque l'on implémentera d'autres éléments d'interface
	query.collide_with_areas = true
	var result: Dictionary = space_state.intersect_ray(query)
	if result.has("position"):
		var position: Vector3 = result["position"]
		var adress = position_to_adress(position)
		if tile_map.get_cell_item(Vector3(adress.x,0,adress.y)) != GridMap.INVALID_CELL_ITEM:
			#Vérification qu'il y a une tuile sur la gridmap à cette adresse
			return adress
	return Vector2i.MIN

func position_to_adress(position: Vector3) -> Vector2i:
	#Transforme une positon dans l'espace en une adresse de grille
	return Vector2i(floor(position.x / 6.25), floori(position.z / 6.25))

func adress_to_position(adress: Vector2i) -> Vector3:
	#Transforme une adresse de grille en une position dans l'espace
	return Vector3(adress.x, 1.28, adress.y)*6.25

func get_gridmap_dimensions(grid_map: GridMap) -> Vector2i:
	#Renvoie la taille de la gridmap en comparant les extremums de positions
	var cells = grid_map.get_used_cells()
	if cells.is_empty():
		return Vector2i.ZERO
	
	var min_x = cells[0].x
	var max_x = cells[0].x
	var min_z = cells[0].z
	var max_z = cells[0].z
	
	for cell in cells:
		min_x = min(min_x, cell.x)
		max_x = max(max_x, cell.x)
		min_z = min(min_z, cell.z)
		max_z = max(max_z, cell.z)
		
	# Largeur (X) et profondeur (Z) en nombre de cellules
	return Vector2i(max_x - min_x + 1, max_z - min_z + 1)
