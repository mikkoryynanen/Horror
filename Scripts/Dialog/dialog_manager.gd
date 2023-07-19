extends Control


func _ready():
	#DialogueManager.dialog_finished.connect(end_dialog)
	pass


func start_dialog(dialog, start_point):
	var line = await DialogueManager.get_next_dialogue_line(dialog)
