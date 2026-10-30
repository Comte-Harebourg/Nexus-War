extends Node3D

var grid_data: Dictionary
var selected_tile: Tile

func update_grid() -> void:
	#Enregistre toute les tuiles enfants dans un dictionnaire
	grid_data.clear()
	for child in get_children():
		grid_data[child.pos]=child
		child.grid=self

func select_tile(Tile) -> void:
	#Actualise la tuile sélectionnée, est appelée depuis la tuile
	if selected_tile: selected_tile.unselect()
	selected_tile = Tile
	print("Tuile sélectionnée", selected_tile.pos)

func _input(event: InputEvent) -> void:
	#Retourne à l'étape de sélection précédente
	if event.is_action("right_mouse_click"):
		if selected_tile:
			selected_tile.unselect()
			selected_tile = null
