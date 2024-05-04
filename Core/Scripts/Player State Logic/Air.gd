# The state the player is in when they're in the air, i.e., jumping.
extends PlayerState

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity: float = ProjectSettings.get_setting("physics/3d/default_gravity")

# The node for globally storing player variables used by other nodes.
@onready var playerVarsSingleton: Node = get_node("/root/PlayerVariables")


# Called when the state is entered
func enter(_msg := {}) -> void:
	if _msg.has("jump_vector"):
		player.velocity = _msg["jump_vector"]
		player.move_and_slide()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func update(_delta) -> void:
	playerVarsSingleton.isMoving = not player.is_on_floor()
	if player.is_on_floor():
		state_machine.transition_to("Idle")


# Called at regular intervals, for physics logic.
func physics_update(_delta) -> void:
	# Add the gravity.
	if not player.is_on_floor():
		player.velocity.y -= gravity * _delta
	
	# Apply all physics movement to the player.
	player.move_and_slide()
