(define(problem prob)
(:domain dom)
(:objects
	villager_0 - person
	villager_1 - person
	building_0 - location
	building_1 - location
	Start_0 - location
	buildingSite - location)
(:init 
	(has-Labourer villager_0) (at villager_0 Start_0)(has-Labourer villager_1) (at villager_1 Start_0)
	(is-BuildingSite buildingSite) (is-School building_1))
(:goal (has-Lumberjack villager_0)))