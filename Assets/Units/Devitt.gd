extends Node3D

@export var UnitName: String = "Devitt"
@export var UnitFaction: String = "Oromound"
@export var speed: float = 5
@export var Vision: float = 6
@export var Camo: float = -1
@export var MaxMember: int = 3
@export var Member: int = 3
@export var MaxHealth: int = 500
@export var Health: int = 500
@export var MaxArmor: int = 50
@export var Armor: int = 50
@export var MaxMorale: int = 5
@export var Morale: int = 5
@export var move_dict: Dictionary[String, float] = {"Grass": 1, "Road": 0.5}
