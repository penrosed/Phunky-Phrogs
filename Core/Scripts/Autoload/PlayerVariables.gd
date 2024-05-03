extends Node


# TODO: Switch from polling to signals. (i.e., instead of updating each frame,
#       send a signal when the variable changes.

## Is the player currently moving?
var isMoving: bool = false

## The current speed of the player.
var currentSpeed: Vector2 = Vector2(1.0, 0.0)

## The distance travelled in the current run.
var runDistance: float = 0.0

## The distance travelled in all runs combined.
var totalDistance: float = 0.0

## The number of hops in the current run.
var runHops: int = 0

## The total number of hops
var totalHops: int = 0
