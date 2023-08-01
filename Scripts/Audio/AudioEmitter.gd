extends Node3D

@export var music_event: EventAsset
@export var footsteps_event: EventAsset
@export var breathe_event: EventAsset

@export var melee_event: EventAsset
@export var melee_hit_event: EventAsset

@export var ui_click_event: EventAsset
@export var ui_confirm_event: EventAsset

@export var melee_pickup_event: EventAsset

@export var pistol_shoot_event: EventAsset
@export var pistol_reload_event: EventAsset

var callable: Callable = Callable(self, "on_melee_end")

signal on_audio_played

var instance: EventInstance
var breathe_instance: EventInstance

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

func on_breathe():
	breathe_instance = RuntimeManager.create_instance(breathe_event)
	breathe_instance.start()
	
func on_set_breathe_end(exhaustion_level: String):
	if breathe_instance == null:
		printerr("Failed to set exhaustion_level. No breathe playing.")
		return;
	print("setting exhaustion_level to " + exhaustion_level)
	breathe_instance.set_parameter_by_name_with_label("ExhaustionLevel", exhaustion_level, false)
	
func on_melee():
	var meleeInstance = RuntimeManager.create_instance(melee_event)
	meleeInstance.start()
	
	# set_callback is not working, This is hacky solution to fix that
	# Not really scaleable if we want to change the duration of the melee sound
	await get_tree().create_timer(0.5).timeout
	on_audio_played.emit()
	#meleeInstance.set_callback(func(): print("melee ended"), FMODStudioModule.FMOD_STUDIO_EVENT_CALLBACK_SOUND_PLAYED)

func on_melee_hit():
	RuntimeManager.play_one_shot(melee_hit_event)

func on_ui_click():
	RuntimeManager.play_one_shot(ui_click_event)
		
func on_ui_confirm():
	RuntimeManager.play_one_shot(ui_confirm_event)
	
func on_melee_pickup():
	RuntimeManager.play_one_shot(melee_pickup_event)
	
func on_pistol_shoot():
	RuntimeManager.play_one_shot(pistol_shoot_event)
	
func on_pistol_reload():
	RuntimeManager.play_one_shot(pistol_reload_event)
