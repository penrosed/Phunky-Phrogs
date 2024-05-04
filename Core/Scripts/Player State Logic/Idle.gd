# The state the player is in when they're in the on the ground.
extends PlayerState

var tapPos: Vector2
var releasePos: Vector2

const JUMP_VELOCITY: float = 5


# Recieves any unhandled input.
func handle_input(_event: InputEvent) -> void:
	if Input.is_action_just_pressed("Tap"):
		tapPos = _event.position
	elif Input.is_action_pressed("Tap"):
		releasePos = _event.position
	elif Input.is_action_just_released("Tap"):
		var jumpVector = Vector3(0.0, JUMP_VELOCITY, 0.0)
		state_machine.transition_to("Air", {jump_vector = jumpVector})
