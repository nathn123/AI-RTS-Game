(define (domain coconut)
  (:requirements :strips :typing)
  
   (:types         
	person - object
	location - object
	item - object
	building - location
	turfhut - building
	house - building
	School - building
    Barracks - building
	Storage - building
	Smelter - building
	Sawmill - building
	Blacksmith - building
	Market_Stall - building
	Stone - item
	Wood - item
	Iron  - item
	Timber  - item
	Ore - item
	Coal - item
	Axe - item
	;;Cart - item
	Rifle - item
	Money - item
	Goods - item
	Labourer - person
	Rifleman - person
	Trader - person
	Blacksmith - person
	Miner - person
	Lumberjack - person
    Carpenter - person
	
	tree - location
	Mine - location
	Quarry - location
	)
	
   (:predicates
		(at ?p - person ?pl - location)
		(at-balloon ?place - location)
		(in-balloon ?person - person)
		(has-coconuts ?pl - location)
		(got-coconuts ?p - person ?pl - location)

;;		(my-new-pred ?a ?b ?c ?d - location ?e - person ?f - location)
   )
   
   (:action walk
           :parameters (?person - person ?pl-from ?pl-to - location)
           :precondition (and (at ?person ?pl-from))
           :effect (and (at ?person ?pl-to) (not (at ?person ?pl-from))))
	
	(:action board
			:parameters (?person - person ?place - location)
			:precondition (and (at ?person ?place) (at-balloon ?place))
			:effect (and (in-balloon ?person) (not (at ?person ?place))))
			
	(:action depart
			:parameters (?person - person ?place - location)
			:precondition (and (in-balloon ?person) (at-balloon ?place))
			:effect (and (not (in-balloon ?person)) (at ?person ?place)))

	(:action fly
			:parameters (?person - person ?pl-from ?pl-to - location)
			:precondition (and (in-balloon ?person) (at-balloon ?pl-from))
			:effect (and (not (at-balloon ?pl-from)) (at-balloon ?pl-to)))
			
	(:action pick
			:parameters (?person - person ?place - location)
			:precondition (and (in-balloon ?person) (at-balloon ?place) (has-coconuts ?place))
			:effect (and (got-coconuts ?person ?place) (not (has-coconuts ?place))))

)
  