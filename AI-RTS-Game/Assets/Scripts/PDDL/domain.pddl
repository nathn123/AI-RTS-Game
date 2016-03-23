(define (domain dom)
  (:requirements :strips :typing)
  
   (:types         
	person - object
	skill - object
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
	Labourer - skill
	Rifleman - skill
	Trader - skill
	Blacksmith - skill
	Miner - skill
	Lumberjack - skill
    Carpenter - skill
	
	tree - location
	Mine - location
	Quarry - location
	)
	
   (:predicates
		;;(at ?i - item ?l - location)
		(at ?p - person ?pl - location)
		(has-item ?p - person)
		(has-item ?l - location)
		(has-skill ?p - person)
		

;;		(my-new-pred ?a ?b ?c ?d - location ?e - person ?f - location)
   )
   
	(:action Walk
           :parameters (?p - person ?locfrom ?locto - location)
           :precondition ((at ?p ?locfrom))
           :effect (at ?p ?locto))
		   
	(:action Pickup
           :parameters (?p - person ?l - location)
           :precondition (and (has-item ?l ) (at ?p ?l))
           :effect (has-item ?p))
		   
	(:action Putdown
           :parameters (?p - person ?l - location)
           :precondition (and (has-item ?p)(at ?p ?l))
           :effect (and (has-item ?l) not (has-item ?p)))
		   
	(:action Family
           :parameters (?p1 ?p2 - person ?h - turfhut)
           :precondition (and (at ?p1 ?h)(at ?p2 ?h))
           :effect ())
		   
	(:action Family_House
           :parameters (?p1 ?p2 - person ?h - house)
           :precondition (and (at ?p1 ?h)(at ?p2 ?h))
           :effect ())
	(:action Educate_Barracks
           :parameters (?p1 - person ?p2 - person ?l - Barracks)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Rifleman ? p2) not(has-Labourer ? p2)))

	(:action Cut_Tree
           :parameters (?p - person ?t - tree)
           :precondition (and (at ?p ?t) (has-Lumberjack ?p) not(has-item ?p))
           :effect ((has-Timber ?p)))	   
	(:action Smelt
           :parameters (?p - person ?s - Smelter )
           :precondition (and (has-Ore ?s)(has-Coal ?s)(at ?p ?s)(has-Labourer ?p))
           :effect (and (has-Iron ?s) not(has-Ore ?s)not(has-Coal ?s)))
	(:action Quarry
           :parameters (?p - person ?l - Quarry)
           :precondition (and (at ?p ?l) (has-Labourer ?p) not(has-item ?p) )
           :effect (has-Stone ?p))
		   
	(:action Saw_Wood
           :parameters (?p - person ?l - Sawmill)
           :precondition (and (has-Timber ?l) (at ?p ?l))
           :effect (and (has-Wood ?l) not(has-Timber ?l)))
		   ;; ALL BELOW NEED COMPLETION
		   
		   
		   
		   
	(:action Make_Tool_TOOLTYPE_
           :parameters (?p - person ?l - Blacksmith)
           :precondition (and (has-Blacksmith ?p) (at ?p ?l))
           :effect ()
	(:action Buy
           :parameters ()
           :precondition ()
           :effect ())
	(:action Sell
           :parameters ()
           :precondition ()
           :effect ())
		   
   (:action Combat
           :parameters ()
           :precondition ()
           :effect ())

	(:action pick
			:parameters (?person - person ?place - location)
			:precondition (and (in-balloon ?person) (at-balloon ?place) (has-coconuts ?place))
			:effect (and (got-coconuts ?person ?place) (not (has-coconuts ?place))))
			
;; special cases build building train people
   (:action Train
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-skill ?_____InsertSkill____ ? p2) not(has-skill ?Labourer ? p2)))
   (:action Educate
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-skill ?_____InsertSkill____ ? p2) not(has-skill ?Labourer ? p2)))
		   
   (:action Build
           :parameters ()
           :precondition ()
           :effect ())
   (:action Mine
           :parameters (?p - person ?l - mine)
           :precondition ()
           :effect ())
   (:action Store
           :parameters (?i - item ?m - money)
           :precondition ((has-item ?))
           :effect ()
)
  