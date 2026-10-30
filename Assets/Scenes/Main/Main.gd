extends Control

func _on_load_skirmish_pressed() -> void:
	SceneManager.change_scene("res://Assets/Scenes/Skirmish/Skirmish.tscn", {})

func _on_quit_pressed() -> void:
	get_tree().quit()
