(define (domain dom)
  (:requirements :strips :typing)
  
   (:types         
	person - object
	skill - object
	location - object
	item - object
	turfhut - location
	house - location
	School - location
    Barracks - location
	Storage - location
	Smelter - location
	Sawmill - location
	Blacksmith - location
	Market_Stall - location
	Stone - item
	Wood - item
	Iron  - item
	Timber  - item
	Ore - item
	Coal - item
	Money - item
	Goods - item
	tool - item
	Axe - tool
	Cart - tool
	Rifle - tool
	Labourer - skill
	Rifleman - skill
	Trader - skill
	BlacksmithS - skill
	Miner - skill
	Lumberjack - skill
    Carpenter - skill
	
	tree - location
	Mine - location
	Quarry - location
	BuildingSite - location
	)
	
   (:predicates
		(at ?p - person ?pl - location)
		(has-item ?p - person)
		(has-item ?l - location)
		(has-skill ?p - person)
		(isParent ?p - person)

;;		(my-new-pred ?a ?b ?c ?d - location ?e - person ?f - location)
   )
   
	(:action Walk
           :parameters (?p - person ?locfrom ?locto - location)
           :precondition ((at ?p ?locfrom))
           :effect (at ?p ?locto))
		   
	(:action Pickup
           :parameters (?p - person ?l - location)
           :precondition (and (has-item ?l ) (at ?p ?l))
           :effect (and (has-item ?p)(not (has-item ?l)))
		   
	(:action Putdown
           :parameters (?p - person ?l - location)
           :precondition (and (has-item ?p)(at ?p ?l))
           :effect (and (has-item ?l) (not (has-item ?p))))
		   
	(:action Family
           :parameters (?p1 ?p2 - person ?h - turfhut)
           :precondition (and (at ?p1 ?h)(at ?p2 ?h))
           :effect (and(isParent ?p1) (isParent ?p2))
		   
	(:action Family_House
           :parameters (?p1 ?p2 - person ?h - house)
           :precondition (and (at ?p1 ?h)(at ?p2 ?h))
           :effect (and(isParent ?p1) (isParent ?p2))
	(:action Educate_Barracks
           :parameters (?p1 - person ?p2 - person ?l - Barracks)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Rifleman ? p2) (not(has-Labourer ? p2))))

	(:action Cut_Tree
           :parameters (?p - person ?t - tree)
           :precondition (and (at ?p ?t) (has-Lumberjack ?p) (not(has-item ?p)))
           :effect ((has-Timber ?p)))	   
	(:action Smelt
           :parameters (?p - person ?s - Smelter )
           :precondition (and (has-Ore ?s)(has-Coal ?s)(at ?p ?s)(has-Labourer ?p))
           :effect (and (has-Iron ?s) (not(has-Ore ?s)(not(has-Coal ?s)))))
	(:action Quarry
           :parameters (?p - person ?l - Quarry)
           :precondition (and (at ?p ?l) (has-Labourer ?p) (not(has-item ?p) ))
           :effect (has-Stone ?p))
		   
	(:action Saw_Wood
           :parameters (?p - person ?l - Sawmill)
           :precondition (and (has-Timber ?l) (at ?p ?l))
           :effect (and (has-Wood ?l) (not(has-Timber ?l))))
	(:action Mine_Coal
           :parameters (?p - person ?l - mine)
           :precondition ((at ?p ?l))
           :effect (has-Coal ?p))
	(:action Mine_Ore
           :parameters (?p - person ?l - mine)
           :precondition ((at ?p ?l))
           :effect (has-Ore ?p))
	(:action Make_Tool_Axe
           :parameters (?p - person ?l - Blacksmith)
           :precondition (and (has-BlacksmithS ?p) (at ?p ?l))
           :effect (has-Axe ?l))
	(:action Make_Tool_Cart
           :parameters (?p - person ?l - Blacksmith)
           :precondition (and (has-BlacksmithS ?p) (at ?p ?l))
           :effect (has-Cart ?l))
	(:action Make_Tool_Rifle
           :parameters (?p - person ?l - Blacksmith)
           :precondition (and (has-BlacksmithS ?p) (at ?p ?l))
           :effect (has-Rifle ?l))
   (:action Build_Turf
           :parameters (?p1 - person ?l - BuildingSite)
           :precondition ( (at ?p1 ?l))
           :effect (?l - turfhut))
	(:action Build_House
           :parameters (?p1 - person ?p2 - person ?l - BuildingSite)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2))
           :effect (?l - House))
	(:action Build_School
           :parameters (?p1 - person ?p2 - person ?l - BuildingSite)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Iron ?l)(has-Carpenter ?p1) (has-Labourer ?p2))
           :effect (?l - School))
	(:action Build_Barracks
           :parameters (?p1 - person ?p2 - person ?l - BuildingSite)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2))
           :effect (?l - Barracks))
	(:action Build_Storage
           :parameters (?p1 - person ?p2 - person ?l - BuildingSite)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2))
           :effect (?l - Storage))
	(:action Build_Mine
           :parameters (?p1 - person ?p2 - person ?p3 - person ?l - BuildingSite)
           :precondition (and (has-Iron ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2)(has-BlacksmithS ?p3))
           :effect (?l - Mine))
	(:action Build_Smelter
           :parameters (?p1 - person ?l - BuildingSite)
           :precondition (and (has-Stone ?l)(has-Labourer ?p1))
           :effect (?l - Smelter))
	(:action Build_Quarry
           :parameters (?p1 - person ?l - BuildingSite)
           :precondition ((has-Labourer ?p1))
           :effect (?l - Quarry))
	(:action Build_Sawmill
           :parameters (?p1 - person ?l - BuildingSite)
           :precondition (and (has-Iron ?l)(has-Stone ?l)(has-Timber ?l) (has-Labourer ?p2))
           :effect (?l - Sawmill))
	(:action Build_Blacksmith
           :parameters (?p1 - person ?l - BuildingSite)
           :precondition (and (has-Iron ?l)(has-Stone ?l)(has-Timber ?l) (has-Labourer ?p2)(at ?p1 ?l))
           :effect (?l - Blacksmith))
	(:action Build_Market
           :parameters (?p1 - person ?l - BuildingSite)
           :precondition (and (has-Wood ?l)(has-Carpenter ?p2))
           :effect (?l - Market_Stall))
		   
		   ;; ALL BELOW NEED COMPLETION
		   
		   
		   
		   

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
			
;; special cases build building train people
   (:action Train_Rifleman
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Rifleman ? p2) (not(has-Labourer ? p2))))
   (:action Educate_Rifleman
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-Rifleman ? p2) (not(has-Labourer ? p2))))
		   
   (:action Train_Lumberjack
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Lumberjack ? p2) (not(has-Labourer ? p2))))
   (:action Educate_Lumberjack
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-Lumberjack ? p2) (not(has-Labourer ? p2))))
		   
   (:action Train_Trader
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Trader ? p2) (not(has-Labourer ? p2))))
   (:action Educate_Trader
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-Trader ? p2) (not(has-Labourer ? p2))))
		   
   (:action Train_BlacksmithS
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-BlacksmithS ? p2) (not(has-Labourer ? p2))))
   (:action Educate_BlacksmithS
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-BlacksmithS ? p2) (not(has-Labourer ? p2))))
		   
   (:action Train_Carpenter
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Carpenter ? p2) (not(has-Labourer ? p2))))
   (:action Educate_Carpenter
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-Carpenter ? p2) (not(has-Labourer ? p2))))
		   
   (:action Train_Miner
           :parameters (?p1 - person ?p2 - person ?l - School)
           :precondition (and (has-skill ?Labourer ? p2) (at ?p1 ?l)(at ?p2 ?l))
           :effect (and (has-Miner ? p2) (not(has-Labourer ? p2))))
   (:action Educate_Miner
           :parameters (?p1 - person ?p2 - person)
           :precondition ((has-skill ?Labourer ? p2))
           :effect (and (has-Miner ? p2) (not(has-Labourer ? p2))))
		   
		   
		   
   (:action Store
           :parameters (?i - item ?m - money)
           :precondition ((has-item ?))
           :effect ()
)
  