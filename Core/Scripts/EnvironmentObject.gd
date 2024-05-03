extends Node3D

# The node containing the player's current speed, and other variables.
var playerVarsSingleton: Node


# Called when the node enters the scene tree for the first time.
func _ready():
	# Get a reference to the Autoloaded PlayerVariables node.
	playerVarsSingleton = get_node("/root/PlayerVariables")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	# When the player moves, it actually stays still, and all platforms move
	# instead. The speed is gotten from the PlayerVariables, and applied here.
	if playerVarsSingleton.isMoving:
		position.x -= playerVarsSingleton.currentSpeed.x * delta
