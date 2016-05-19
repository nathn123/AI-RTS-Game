(define (problem prob)
  (:domain dom)
  	
  (:objects 
	alex - person 
	tree1 tree2 - location)

  (:init 	
			(in-balloon alex)
			(at-balloon tree1)
			(has-coconuts tree1)
			(has-coconuts tree2)
	)
  
  (:goal 	
  ;; dont forget the (and ) block to enclose everything together
		(and
			(got-coconuts alex tree1) 
			(got-coconuts alex tree2) 
			)
	)
)
