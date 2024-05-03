extends CharacterBody3D


const JUMP_VELOCITY: float = 4.5

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity: float = ProjectSettings.get_setting("physics/3d/default_gravity")

# The node for globally storing player variables used by other nodes.
var playerVarsSingleton: Node

var isHolding: bool = false
var tapPos: Vector2
var releasePos: Vector2


# Called when the node enters the scene tree for the first time.
func _ready():
	# Get a reference to the Autloaded PlayerVariables node.
	playerVarsSingleton = get_node("/root/PlayerVariables")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float):
	# Update the global player vars
	playerVarsSingleton.isMoving = not is_on_floor()


# Called at regular intervals, for physics logic.
func _physics_process(delta: float):
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta

	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	move_and_slide()

func _input(event: InputEvent):
	if Input.is_action_just_pressed("Tap"):
		tapPos = event.position
		print("tap:", event.position)
	elif Input.is_action_pressed("Tap"):
		isHolding = true
		releasePos = event.position
		print("HOLD:",event.position)
	elif Input.is_action_just_released("Tap"):
		isHolding = false
		releasePos = event.position
		print("un-tap:",event.position)
