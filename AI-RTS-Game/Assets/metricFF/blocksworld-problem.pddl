;; problem file: blocksworld-prob1.pddl

(define (problem blocksworld-prob1)
  (:domain blocksworld)
  (:objects Stone Wood Iron Timber Ore Coal Axe Cart Rifle Money Goods
			Labourer Rifleman Trader Blacksmith Miner Lumberjack Carpenter
			Tree TurfHut House School Barracks Storage Mine Smelter Quarry Sawmill Blacksmith Marketstall)
  (:init (Item  Stone) (Item  Wood)(Item  Iron)(Item  Timber)(Item  Ore)(Item  Coal)(Item  Axe)(Item  Rifle)
		 (Item  Money) (Item  Goods)
		 (Skill  Labourer)(Skill  Rifleman)(Skill  Trader)(Skill  Blacksmith)(Skill  Miner)(Skill  Lumberjack)
		 (Skill  Carpenter)
		 (Building )
		 );; need to add in 
  (:goal (and (on a b) (on c a))))