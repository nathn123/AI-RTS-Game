(define(problem prob)
(:domain dom)
(:objects
villager_0 - person
Start_0 - location
villager_1 - person
Start_1 - location
building_0 - location
building_1 - location
building_2 - location
building_3 - location
)(:init 
(has-Labourer  villager_0)
(at villager_0  Start_0)
(has-Labourer  villager_1)
(at villager_1  Start_1)
(is-turfhut  building_0)
(is-turfhut  building_1)
(is-Quarry  building_2)
(is-Quarry  building_3)
)(:goal 
(and
(isParent villager_0)
(isParent villager_1)
)))