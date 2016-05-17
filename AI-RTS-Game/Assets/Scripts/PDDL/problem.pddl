(define(problem prob)
(:domain dom)
(:objects
villager_0 - person
Start_0 - location
building_0 - location
building_1 - location
buildingSite - location
)(:init 
(has-Labourer villager_0)
(at villager_0 Start_0)
(is-BuildingSite buildingSite)
)(:goal 
(and
(is-turfhut buildingSite)
)))