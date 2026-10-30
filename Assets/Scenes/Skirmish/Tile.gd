class_name Tile
extends Node3D

var grid: Node3D
@export var tile_name: String
@export var pos: Vector2i #Position de la grille dans le modèle pas dans le monde 3D
@export var neigbhors: Array[Tile]
@export var type: String
@export var move_type: String #Uniquement pour les tuiles moddées
@export var cover: int
@export var devastation: Tile
#@export var ocupied_unit: Unit

@onready var Collider: Area3D = $Area3D

func _ready() -> void:
	$Highlight.visible = false
	print("Tuile instanciée en" + str(pos))

func _on_area_3d_mouse_entered() -> void:
	if !grid.selected_tile:
		$Highlight.visible = true

func _on_area_3d_mouse_exited() -> void:
	if !grid.selected_tile:
		$Highlight.visible = false

func _on_area_3d_input_event(camera: Node, event: InputEvent, event_position: Vector3, normal: Vector3, shape_idx: int) -> void:
	#Sélectionne la case sous la souris
	if event.is_action_pressed("left_mouse_click"):
		grid.select_tile(self)

func unselect() -> void:
	#Pour éviter que les tuiles déselectionnées restent avec le curseur
	$Highlight.visible = false
	print("fait")
