extends Node3D

@onready var camera: Camera3D = %Camera
@onready var tile_map: GridMap = %TileMap
@onready var cursor: AnimatedSprite3D = %Cursor
@onready var grid_shader: MeshInstance3D = %GridShader
@onready var minimap: TextureRect = %Minimap
@onready var tile_database: TileDatabase = %TileData
@onready var tile_colors: Array[Color] = [
Color.DARK_BLUE, #0: occupied_move
Color.BLUE, #1: move
Color.RED, #2: occupied_attack
Color.DARK_RED, #3: attack
Color.GREEN, #4: interact
Color.DARK_GREEN, #5: occupied_interact
Color.ORANGE, #6: move_danger
Color.DARK_ORANGE #7: attack_danger
]
var grid_size: Vector2i
var grid_origin: Vector2i
var _data_image: Image
var _data_texture: ImageTexture
var _shader_material: ShaderMaterial
var _channels: Array[Array]
var _dirty_cell: Dictionary[int, bool] #Contient les index des tuiles à mettre à jour

func _ready() -> void:
	var rect: Rect2i = get_gridmap_rect(tile_map)
	grid_origin = rect.position
	grid_size = rect.size
	grid_shader.scale = Vector3(grid_size.x, 1, grid_size.y)
	_setup()
	minimap.texture = _data_texture

func _input(event: InputEvent) -> void:
	#Retourne à l'étape de sélection précédente
	if event.is_action("right_mouse_click") and event.is_pressed():
		set_higlight(1, [get_tile_adress_under_mouse()])
		update_highlights()
		
	if event.is_action("left_mouse_click") and event.is_pressed():
		var address = get_tile_adress_under_mouse()
		if is_legal_tile(address):
			cursor.position = adress_to_position(address) + Vector3(0,4.875,0)
			#Décalage nécessaire à cause de l'offset du cursor
			set_higlight(2, [address])
			update_highlights()
			print(tile_database.get_tile(tile_map.mesh_library.get_item_name(tile_map.get_cell_item(Vector3(address.x,0,address.y)))).move_type)

func _setup() -> void:
	_data_image = Image.create_empty(grid_size.x, grid_size.y, false, Image.FORMAT_RGBA8)
	_data_texture = ImageTexture.create_from_image(_data_image)
	_channels = []
	_dirty_cell = {}
	for i: int in range(tile_colors.size()):
		var arr: Array[bool] = []
		arr.resize(grid_size.x * grid_size.y)
		_channels.append(arr)
	_shader_material = grid_shader.get_active_material(0) as ShaderMaterial
	_shader_material.set_shader_parameter("color_data_map", _data_texture)
	_shader_material.set_shader_parameter("map_size", grid_size)
	update_highlights()

func clear_channel(channel_index: int) -> void:
	for index: int in range(grid_size.x * grid_size.y):
		if _channels[channel_index][index]:
			_dirty_cell.set(index, true)
	_channels[channel_index].fill(false)

func set_higlight(channel_index: int, tiles: Array[Vector2i], on: bool = true) -> void:
	#Modifie la valeur d'un index d'un channel, ne mets pas à jour la grille
	for address: Vector2i in tiles:
		if is_legal_tile(address):
			var index: int = address_to_index(address)
			_channels[channel_index][index] = on
			_dirty_cell.set(index, true)

func set_tile_color(address: Vector2i, color: Color) -> void:
	#Pour changer la couleur d'une tuile directement
	var local: Vector2i = address - grid_origin
	if local.x >= 0 and local.y >= 0 and local.x < grid_size.x and local.y < grid_size.y:
		_data_image.set_pixel(local.x, local.y, color)

func update_highlights() -> void:
	#Mets à jour la grille pour s'accorder aux valeurs des channels
	if _dirty_cell.is_empty(): return
	for index: int in _dirty_cell:
		var color: Color = Color.TRANSPARENT
		for channel_index: int in range(_channels.size()):
			if _channels[channel_index][index]:
				color = tile_colors[channel_index]
				break
		set_tile_color(index_to_adress(index), color)
	_data_texture.update(_data_image)
	_dirty_cell.clear()
		
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
		if is_legal_tile(adress): return adress
	return Vector2i.MIN

func position_to_adress(world_pos: Vector3) -> Vector2i:
	#Transforme une positon dans l'espace en une adresse de grille
	var cell: Vector3i = tile_map.local_to_map(tile_map.to_local(world_pos))
	return Vector2i(cell.x, cell.z)

func adress_to_position(adress: Vector2i) -> Vector3:
	#Transforme une adresse de grille en une position dans l'espace
		return tile_map.to_global(tile_map.map_to_local(Vector3i(adress.x, 0, adress.y)))

func address_to_index(address: Vector2i) -> int:
	#Tranforme une adresse en index pour les channels de tuiles
	var local: Vector2i = address - grid_origin
	return local.y * grid_size.x + local.x

func index_to_adress(index: int) -> Vector2i:
	#Donne l'adresse de la tuile depuis l'index d'un channel
	@warning_ignore("integer_division")
	return Vector2i(index % grid_size.x, index / grid_size.x) + grid_origin

func get_gridmap_rect(grid_map: GridMap) -> Rect2i:
	#Renvoie le Rect de la GridMap pour calculer ses dimensions et origine
	var cells: Array[Vector3i] = grid_map.get_used_cells()
	if cells.is_empty():
		return Rect2i()
	var min_c := Vector2i(cells[0].x, cells[0].z)
	var max_c := min_c
	for c: Vector3i in cells:
		min_c.x = mini(min_c.x, c.x)
		min_c.y = mini(min_c.y, c.z)
		max_c.x = maxi(max_c.x, c.x)
		max_c.y = maxi(max_c.y, c.z)
	return Rect2i(min_c, max_c - min_c + Vector2i.ONE)

func is_legal_tile(address: Vector2i) -> bool:
	var local: Vector2i = address - grid_origin
	if local.x >= 0 and local.y >= 0 and local.x < grid_size.x and local.y < grid_size.y:
		return tile_map.get_cell_item(Vector3(address.x,0,address.y)) != GridMap.INVALID_CELL_ITEM
	return false
