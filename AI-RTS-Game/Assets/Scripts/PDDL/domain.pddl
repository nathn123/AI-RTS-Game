(define (domain dom)
  (:requirements :strips :typing)
  
   (:types         
	person - object
	skill - object
	location - object


	

	)
	
   (:predicates
		
		(at ?p - person ?pl - location)
		
		(has-Labourer ?p - person)
		(has-Rifleman ?p - person)
		(has-Trader ?p - person)
		(has-BlacksmithS ?p - person)
		(has-Miner ?p - person)
		(has-Lumberjack ?p - person)
		(has-Carpenter ?p - person)
		
		(has-Stone ?p - object)
		(has-Wood ?p - object)
		(has-Iron ?p - object)
		(has-Timber ?p - object)
		(has-Ore ?p - object)
		(has-Coal ?p - object)
		(has-Money ?p - object)
		(has-Goods ?p - object)
		(has-tool ?p - object)
		(has-Axe ?p - object)
		(has-Cart ?p - object)
		(has-Rifle ?p - object)
		
		
		(isParent ?p - person)

		(is-turfhut ?l - location)
		(is-house ?l - location)
		(is-School ?l - location)
		(is-Barracks ?l - location)
		(is-Storage ?l - location)
		(is-Smelter ?l - location)
		(is-Sawmill ?l - location)
		(is-Blacksmith ?l - location)
		(is-Market_Stall ?l - location)
		(is-BuildingSite ?l - location)
		(is-Quarry ?l - location)
		(is-Mine ?l - location)
		(is-Tree ?l - location)

;;		(my-new-pred ?a ?b ?c ?d - location ?e - person ?f - location)
   )
   
	(:action Walk
           :parameters (?p - person ?locfrom ?locto - location)
           :precondition (at ?p ?locfrom)
           :effect (and(at ?p ?locto)(not (at ?p ?locfrom))))
		   
	(:action Pickup_Stone
           :parameters (?p - person ?l - location )
           :precondition (and (has-Stone ?l ) (at ?p ?l))
           :effect (and (has-Stone ?p)(not (has-Stone ?l))))
	(:action Pickup_Wood
           :parameters (?p - person ?l - location )
           :precondition (and (has-Wood ?l ) (at ?p ?l))
           :effect (and (has-Wood ?p)(not (has-Wood ?l))))		   
	(:action Pickup_Iron
           :parameters (?p - person ?l - location )
           :precondition (and (has-Iron ?l ) (at ?p ?l))
           :effect (and (has-Iron ?p)(not (has-Iron ?l))))
	(:action Pickup_Timber
           :parameters (?p - person ?l - location )
           :precondition (and (has-Timber ?l ) (at ?p ?l))
           :effect (and (has-Timber ?p)(not (has-Timber ?l))))
	(:action Pickup_Ore
           :parameters (?p - person ?l - location )
           :precondition (and (has-Ore ?l ) (at ?p ?l))
           :effect (and (has-Ore ?p)(not (has-Ore ?l))))
	(:action Pickup_Coal
           :parameters (?p - person ?l - location )
           :precondition (and (has-Coal ?l ) (at ?p ?l))
           :effect (and (has-Coal ?p)(not (has-Coal ?l))))
	(:action Pickup_Money
           :parameters (?p - person ?l - location )
           :precondition (and (has-Money ?l ) (at ?p ?l))
           :effect (and (has-Money ?p)(not (has-Money ?l))))
	(:action Pickup_Goods
           :parameters (?p - person ?l - location )
           :precondition (and (has-Goods ?l ) (at ?p ?l))
           :effect (and (has-Goods ?p)(not (has-Goods ?l))))
	(:action Pickup_tool
           :parameters (?p - person ?l - location )
           :precondition (and (has-tool ?l ) (at ?p ?l))
           :effect (and (has-tool ?p)(not (has-tool ?l))))
	(:action Pickup_Axe
           :parameters (?p - person ?l - location )
           :precondition (and (has-Axe ?l ) (at ?p ?l))
           :effect (and (has-Axe ?p)(not (has-Axe ?l))))
	(:action Pickup_Cart
           :parameters (?p - person ?l - location )
           :precondition (and (has-Cart ?l ) (at ?p ?l))
           :effect (and (has-Cart ?p)(not (has-Cart ?l))))
	(:action Pickup_Rifle
           :parameters (?p - person ?l - location )
           :precondition (and (has-Rifle ?l ) (at ?p ?l))
           :effect (and (has-Rifle ?p)(not (has-Rifle ?l))))
		   
		   
	(:action Putdown_Stone
           :parameters (?p - person ?l - location )
           :precondition (and (has-Stone ?l ) (at ?p ?l))
           :effect (and (has-Stone ?l)(not (has-Stone ?p))))
	(:action Putdown_Wood
           :parameters (?p - person ?l - location )
           :precondition (and (has-Wood ?l ) (at ?p ?l))
           :effect (and (has-Wood ?l)(not (has-Wood ?p))))		   
	(:action Putdown_Iron
           :parameters (?p - person ?l - location )
           :precondition (and (has-Iron ?l ) (at ?p ?l))
           :effect (and (has-Iron ?l)(not (has-Iron ?p))))
	(:action Putdown_Timber
           :parameters (?p - person ?l - location )
           :precondition (and (has-Timber ?l ) (at ?p ?l))
           :effect (and (has-Timber ?l)(not (has-Timber ?p))))
	(:action Putdown_Ore
           :parameters (?p - person ?l - location )
           :precondition (and (has-Ore ?l ) (at ?p ?l))
           :effect (and (has-Ore ?l)(not (has-Ore ?p))))
	(:action Putdown_Coal
           :parameters (?p - person ?l - location )
           :precondition (and (has-Coal ?l ) (at ?p ?l))
           :effect (and (has-Coal ?l)(not (has-Coal ?p))))
	(:action Putdown_Money
           :parameters (?p - person ?l - location )
           :precondition (and (has-Money ?l ) (at ?p ?l))
           :effect (and (has-Money ?l)(not (has-Money ?p))))
	(:action Putdown_Goods
           :parameters (?p - person ?l - location )
           :precondition (and (has-Goods ?l ) (at ?p ?l))
           :effect (and (has-Goods ?l)(not (has-Goods ?p))))
	(:action Putdown_tool
           :parameters (?p - person ?l - location )
           :precondition (and (has-tool ?l ) (at ?p ?l))
           :effect (and (has-tool ?l)(not (has-tool ?p))))
	(:action Putdown_Axe
           :parameters (?p - person ?l - location )
           :precondition (and (has-Axe ?l ) (at ?p ?l))
           :effect (and (has-Axe ?l)(not (has-Axe ?p))))
	(:action Putdown_Cart
           :parameters (?p - person ?l - location )
           :precondition (and (has-Cart ?l ) (at ?p ?l))
           :effect (and (has-Cart ?l)(not (has-Cart ?p))))
	(:action Putdown_Rifle
           :parameters (?p - person ?l - location )
           :precondition (and (has-Rifle ?l ) (at ?p ?l))
           :effect (and (has-Rifle ?l)(not (has-Rifle ?p))))

		   
		   
		   
	(:action Family
           :parameters (?p1 ?p2 - person ?l - location)
           :precondition (and (at ?p1 ?l)(at ?p2 ?l)(is-turfhut ?l))
           :effect (and(isParent ?p1) (isParent ?p2)))
		   
	(:action Family_House
           :parameters (?p1 ?p2 - person ?l - location)
           :precondition (and (at ?p1 ?l)(at ?p2 ?l)(is-House ?l))
           :effect (and(isParent ?p1) (isParent ?p2)))

	(:action Cut_Tree
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (has-Lumberjack ?p)(is-tree ?l) )
           :effect (has-Timber ?p))	   
	(:action Smelt
           :parameters (?p - person ?l - location)
           :precondition (and (has-Ore ?l)(has-Coal ?l)(at ?p ?l)(has-Labourer ?p)(is-Smelter ?l))
           :effect (and (has-Iron ?l) (not (has-Ore ?l))(not (has-Coal ?l) )))
	(:action Quarry
           :parameters (?p - person ?l - location)
           :precondition (and (at ?p ?l) (has-Labourer ?p)(is-Quarry ?l))
           :effect (has-Stone ?p))
		   
	(:action Saw_Wood
           :parameters (?p - person ?l - location)
           :precondition (and (has-Timber ?l) (at ?p ?l)(is-Sawmill ?l))
           :effect (and (has-Wood ?l) (not(has-Timber ?l))))
	(:action Mine_Coal
           :parameters (?p - person ?l - location)
           :precondition (and(at ?p ?l)(is-Mine ?l))
           :effect (has-Coal ?p))
	(:action Mine_Ore
           :parameters (?p - person ?l - location)
           :precondition (and(at ?p ?l)(is-Mine ?l))
           :effect (has-Ore ?p))
		   
	(:action Make_Tool_Axe
           :parameters (?p - person ?l - location)
           :precondition (and (has-BlacksmithS ?p) (at ?p ?l)(is-Blacksmith ?l))
           :effect (has-Axe ?l))
		   
	(:action Make_Tool_Cart
           :parameters (?p - person ?l - location)
           :precondition (and (has-BlacksmithS ?p) (at ?p ?l)(is-Blacksmith ?l))
           :effect (has-Cart ?l))
		   
	(:action Make_Tool_Rifle
           :parameters (?p - person ?l - location)
           :precondition (and (has-BlacksmithS ?p) (at ?p ?l)(is-Blacksmith ?l))
           :effect (has-Rifle ?l))
		   
   (:action Build_Turf
           :parameters (?p1 - person ?l - location)
           :precondition (and(at ?p1 ?l)(is-BuildingSite ?l))
           :effect (is-turfhut ?l))
	(:action Build_House
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2)(is-BuildingSite ?l))
           :effect (is-House ?l))
	(:action Build_School
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Iron ?l)(has-Carpenter ?p1) (has-Labourer ?p2)(is-BuildingSite ?l))
           :effect (is-School ?l))
	(:action Build_Barracks
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2)(is-BuildingSite ?l))
           :effect (is-Barracks ?l))
	(:action Build_Storage
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Stone ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2)(is-BuildingSite ?l))
           :effect (is-Storage ?l))
	(:action Build_Mine
           :parameters (?p1 - person ?p2 - person ?p3 - person ?l - location)
           :precondition (and (has-Iron ?l)(has-Wood ?l)(has-Carpenter ?p1) (has-Labourer ?p2)(has-BlacksmithS ?p3)(is-BuildingSite ?l))
           :effect (is-Mine ?l))
	(:action Build_Smelter
           :parameters (?p1 - person ?l - location)
           :precondition (and (has-Stone ?l)(has-Labourer ?p1)(is-BuildingSite ?l))
           :effect (is-Smelter ?l))
	(:action Build_Quarry
           :parameters (?p1 - person ?l - location)
           :precondition (and(has-Labourer ?p1)(is-BuildingSite ?l))
           :effect (is-Quarry ?l))
	(:action Build_Sawmill
           :parameters (?p1 - person ?l - location)
           :precondition (and (has-Iron ?l)(has-Stone ?l)(has-Timber ?l) (has-Labourer ?p1)(is-BuildingSite ?l))
           :effect (is-Sawmill ?l))
	(:action Build_Blacksmith
           :parameters (?p1 - person ?l - location)
           :precondition (and (has-Iron ?l)(has-Stone ?l)(has-Timber ?l) (has-Labourer ?p1)(at ?p1 ?l)(is-BuildingSite ?l))
           :effect (is-Blacksmith ?l))
	(:action Build_Market
           :parameters (?p1 - person ?l - location)
           :precondition (and (has-Wood ?l)(has-Carpenter ?p1)(is-BuildingSite ?l))
           :effect (is-Market_Stall ?l))
		   
		   ;; ALL BELOW NEED COMPLETION
		   
		   
		   
		   

	;;(:action Buy
    ;;       :parameters ()
    ;;       :precondition ()
    ;;       :effect ())
	;;(:action Sell
    ;;       :parameters ()
    ;;       :precondition ()
    ;;       :effect ())
	;;	   
   ;;(:action Combat
   ;;        :parameters ()
   ;;        :precondition ()
    ;;       :effect ())
			
;; special cases build building train people
   (:action Train_Rifleman
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Labourer ?p2)(at ?p1 ?l)(at ?p2 ?l)(is-Barracks ?l))
           :effect (and (has-Rifleman ?p2) (not(has-Labourer ?p2))))
		   
   (:action Train_Lumberjack
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l)(is-School ?l))
           :effect (and (has-Lumberjack ?p2) (not(has-Labourer ?p2))))
   (:action Educate_Lumberjack
           :parameters (?p1 - person ?p2 - person)
           :precondition (has-Labourer ?p2)
           :effect (and (has-Lumberjack ?p2) (not(has-Labourer ?p2))))
		   
   (:action Train_Trader
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l)(is-School ?l))
           :effect (and (has-Trader ?p2) (not(has-Labourer ?p2))))
   (:action Educate_Trader
           :parameters (?p1 - person ?p2 - person)
           :precondition (has-Labourer ?p2)
           :effect (and (has-Trader ?p2) (not(has-Labourer ?p2))))
		   
   (:action Train_BlacksmithS
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l)(is-School ?l))
           :effect (and (has-BlacksmithS ?p2) (not(has-Labourer ?p2))))
   (:action Educate_BlacksmithS
           :parameters (?p1 - person ?p2 - person)
           :precondition (has-Labourer ?p2)
           :effect (and (has-BlacksmithS ?p2) (not(has-Labourer ?p2))))
		   
   (:action Train_Carpenter
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l)(is-School ?l))
           :effect (and (has-Carpenter ?p2) (not(has-Labourer ?p2))))
   (:action Educate_Carpenter
           :parameters (?p1 - person ?p2 - person)
           :precondition (has-Labourer ?p2)
           :effect (and (has-Carpenter ?p2) (not(has-Labourer ?p2))))
		   
   (:action Train_Miner
           :parameters (?p1 - person ?p2 - person ?l - location)
           :precondition (and (has-Labourer ?p2) (at ?p1 ?l)(at ?p2 ?l)(is-School ?l))
           :effect (and (has-Miner ?p2) (not(has-Labourer ?p2))))
   (:action Educate_Miner
           :parameters (?p1 - person ?p2 - person)
           :precondition (has-Labourer ?p2)
           :effect (and (has-Miner ?p2) (not(has-Labourer ?p2))))
		   
		   
		   
   ;;(:action Store
   ;;        :parameters (?i - item ?m - money)
   ;;        :precondition ((has-item ?))
   ;;        :effect ()
)
  