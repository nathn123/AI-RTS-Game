;; domain file: blocksworld-domain.pddl

(define (domain blocksworld)
  (:requirements :strips)

  (:predicates (is-at ?Person ?Location)
               (holding ?Item)
			   (has-skill ?Skill)
			   ())

	(:action pickup
           :parameters ()
           :precondition ()
           :effect ())
	(:action move
           :parameters ()
           :precondition ()
           :effect ())
	(:action Family
           :parameters ()
           :precondition ()
           :effect ())
	(:action Family_House
           :parameters ()
           :precondition ()
           :effect ())
	(:action Educate
           :parameters ()
           :precondition ()
           :effect ())
	(:action Educate_Barracks
           :parameters ()
           :precondition ()
           :effect ())
	(:action Train
           :parameters ()
           :precondition ()
           :effect ())
	(:action Cut_Tree
           :parameters ()
           :precondition ()
           :effect ())
	(:action Mine
           :parameters ()
           :precondition ()
           :effect ())
	(:action Store
           :parameters ()
           :precondition ()
           :effect ())
	(:action Smelt
           :parameters ()
           :precondition ()
           :effect ())
	(:action Quarry
           :parameters ()
           :precondition ()
           :effect ())
	(:action Saw_Wood
           :parameters ()
           :precondition ()
           :effect ())
	(:action Make_Tool
           :parameters ()
           :precondition ()
           :effect ())
	(:action Buy_Sell
           :parameters ()
           :precondition ()
           :effect ())
	(:action Combat
           :parameters ()
           :precondition ()
           :effect ())
)