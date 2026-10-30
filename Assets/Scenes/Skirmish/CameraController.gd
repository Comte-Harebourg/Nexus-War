extends Node3D

@onready var camera_rotation = $CameraRotation
@onready var camera_pivot = $CameraRotation/CameraPivot
@onready var camera: Camera3D = $CameraRotation/CameraPivot/Camera3D

#Variables à revoir
var move_speed: float = 0.5
var move_target: Vector3
var rotate_speed: float = 1.
var rotate_target: float
var min_rotation: float = 0
var max_rotation: float = 90
var zoomp_speed: float = 3.
var zoom_target: float
var min_zoom: float = -20.
var max_zoom: float = 20.
var camera_sensitivity: float = 0.2
var edge_size: float = 15.
var scroll_speed: float = 0.6
var is_shaking: bool = false
#Penser à rajouter des limites de map pour éviter de perde la caméra dans l'infini

func _ready() -> void:
	#Attribution des valeurs de départ, si on veut mettre des valeurs par défaut c'est ici
	camera.look_at(position) #Redresse la caméra vers sa position
	move_target = position
	rotate_target = rotation_degrees.y
	zoom_target = camera.position.z

func _process(delta: float) -> void:
	#Gère le déplacement au bord de l'écran
	var mouse_posisiton = get_viewport().get_mouse_position()
	var screen_size = get_viewport().get_visible_rect().size
	var scroll_direction = Vector3.ZERO
	if mouse_posisiton.x <= edge_size and 0 <= mouse_posisiton.x:
		scroll_direction.x = -1
	elif mouse_posisiton.x >= screen_size.x - edge_size and screen_size.x >= mouse_posisiton.x:
		scroll_direction.x = 1
	if mouse_posisiton.y <= edge_size and 0 <= mouse_posisiton.y:
		scroll_direction.z = -1
	elif mouse_posisiton.y >= screen_size.y - edge_size and screen_size.y >= mouse_posisiton.y:
		scroll_direction.z = 1
	move_target += transform.basis * scroll_direction * scroll_speed
	
	#Bloque la souris en rotation libre puis la libère
	if Input.is_action_just_pressed("camera_rotate"):
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	if Input.is_action_just_released("camera_rotate"):
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	
	#Récupération des inputs du joueur
	var input_direction = Input.get_vector("camera_left", "camera_right", "camera_forward", "camera_downward")
	var movement_direction = (transform.basis * Vector3(input_direction.x, 0, input_direction.y)).normalized()
	var rotation_direction = Input.get_axis("camera_rotate_left", "camera_rotate_right")
	var zoom_direction = (int(Input.is_action_just_released("camera_out"))
					   -  int(Input.is_action_just_released("camera_in")))
	
	#Calcul des cibles
	move_target +=  movement_direction * move_speed
	rotate_target += rotation_direction * rotate_speed
	zoom_target += zoom_direction * zoomp_speed
	zoom_target = clamp(zoom_target, min_zoom, max_zoom)
	
	#Déplacement jusqu'aux cibles
	position = lerp(position, move_target, 0.05)
	rotation_degrees.y = lerp(rotation_degrees.y, rotate_target, 0.05)
	camera.position.z = lerp(camera.position.z, zoom_target, 0.05)

func _unhandled_input(event: InputEvent) -> void:
	#Tourne la caméra avec le déplacement de la souris
	if event is InputEventMouseMotion and Input.is_action_pressed("camera_rotate"):
		rotate_target -= event.relative.x * camera_sensitivity
		camera_rotation.rotation_degrees.x -= event.relative.y * camera_sensitivity
		camera_rotation.rotation_degrees.x = clamp(camera_rotation.rotation_degrees.x, min_zoom, max_zoom)

#A remplacer avec le nouveau plugin
func camera_shake(intensity: float = 0.02, duration: float = 0.25) -> void:
	#Secoue la caméra en déplaçant le pivot
	if is_shaking: return
	is_shaking = true
	var time_left := duration
	var start_position = camera_pivot.position
	while time_left > 0:
		var offset := Vector3(randf_range(-intensity, intensity), randf_range(-intensity, intensity), 0.0)
		camera_pivot.position = start_position + offset
		time_left -= get_process_delta_time()
		await get_tree().process_frame
	camera_pivot.position = start_position
	is_shaking = false

func _input(event: InputEvent) -> void:
	#Test de fonction
	if event.is_action("dev_test"):
		pass
