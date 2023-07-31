extends Node3D

@export var music_event: EventAsset
@export var footsteps_event: EventAsset

@export var melee_event: EventAsset
@export var melee_hit_event: EventAsset

var instance: EventInstance

func play_music():
	instance = RuntimeManager.create_instance(music_event)
	instance.start()
	
func on_set_intensity(intensity: String):
	if instance == null:
		printerr("Failed to set music intensity. No music playing.")
		return;
	print("setting music intensity to " + intensity)
	instance.set_parameter_by_name_with_label("Intensity", intensity, false)

func on_footstep():
	RuntimeManager.play_one_shot(footsteps_event)
	
func on_melee():
	RuntimeManager.play_one_shot(melee_event)

func on_melee_hit():
	RuntimeManager.play_one_shot(melee_hit_event)
	
