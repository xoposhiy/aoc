import functools
import re

test_inp = """shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags."""

inp = """mirrored gold bags contain 3 light teal bags.
clear gold bags contain 5 light maroon bags, 4 pale tomato bags, 5 clear blue bags.
dark olive bags contain 5 plaid black bags, 2 dim plum bags, 2 light cyan bags.
bright white bags contain 2 pale violet bags, 5 mirrored orange bags, 3 faded beige bags.
posh green bags contain 4 shiny gray bags.
posh lime bags contain 3 muted lavender bags, 1 clear magenta bag, 5 muted orange bags, 3 mirrored cyan bags.
striped turquoise bags contain 3 pale red bags, 4 wavy lime bags, 4 wavy aqua bags.
pale fuchsia bags contain 1 striped purple bag.
dark magenta bags contain 4 light indigo bags, 1 wavy lavender bag, 1 clear teal bag.
drab teal bags contain 5 pale bronze bags.
muted aqua bags contain 4 wavy bronze bags, 1 pale plum bag.
vibrant brown bags contain 4 dull turquoise bags.
vibrant green bags contain 5 dim violet bags, 2 dotted red bags, 2 dull indigo bags.
wavy black bags contain 3 posh yellow bags, 5 wavy coral bags.
light fuchsia bags contain 1 dull violet bag, 5 dim indigo bags, 4 plaid red bags, 5 dotted bronze bags.
drab bronze bags contain 5 dim tan bags, 4 mirrored yellow bags, 5 dim indigo bags.
plaid silver bags contain 4 pale orange bags.
wavy red bags contain 3 muted chartreuse bags, 4 light silver bags.
light aqua bags contain 4 dark cyan bags, 3 shiny aqua bags, 1 light olive bag, 3 wavy purple bags.
wavy salmon bags contain 1 shiny blue bag, 2 bright green bags, 5 dark beige bags.
vibrant indigo bags contain 4 drab lime bags, 5 plaid turquoise bags.
striped violet bags contain 3 dim violet bags, 3 striped purple bags.
dull salmon bags contain 5 vibrant turquoise bags, 5 muted black bags.
dark lavender bags contain 5 pale gold bags, 5 pale silver bags.
clear lavender bags contain 1 faded turquoise bag.
striped crimson bags contain 2 dull turquoise bags, 1 dotted red bag, 5 striped yellow bags.
posh chartreuse bags contain 2 wavy lavender bags, 2 striped black bags, 5 dim blue bags, 5 bright fuchsia bags.
clear tomato bags contain 1 mirrored violet bag, 4 pale gold bags, 5 bright bronze bags.
clear brown bags contain 3 plaid fuchsia bags.
pale aqua bags contain 3 pale indigo bags, 1 dark lime bag, 4 dim magenta bags.
bright maroon bags contain 4 wavy yellow bags, 4 bright tan bags, 2 posh indigo bags, 4 shiny white bags.
wavy purple bags contain 1 faded beige bag, 2 drab salmon bags, 3 mirrored yellow bags.
muted salmon bags contain 5 bright olive bags, 2 striped white bags, 3 dotted coral bags.
dotted maroon bags contain 4 striped brown bags.
wavy turquoise bags contain 1 dim indigo bag, 3 clear lavender bags.
muted tomato bags contain 2 wavy turquoise bags, 4 pale crimson bags.
faded teal bags contain 2 dotted yellow bags, 4 dull olive bags.
dotted blue bags contain 4 striped plum bags, 4 striped cyan bags, 2 drab bronze bags.
posh teal bags contain 2 vibrant purple bags, 4 plaid fuchsia bags, 2 light green bags, 3 light chartreuse bags.
muted gold bags contain 1 muted green bag, 1 vibrant brown bag, 5 pale crimson bags.
clear olive bags contain no other bags.
dotted beige bags contain 4 posh beige bags, 2 pale silver bags.
vibrant lavender bags contain 4 dark tomato bags, 5 dim black bags, 3 dim aqua bags.
wavy maroon bags contain 1 drab turquoise bag, 1 shiny indigo bag.
muted magenta bags contain 4 posh indigo bags, 4 faded brown bags.
dull tomato bags contain 1 pale green bag, 1 plaid gold bag, 5 plaid yellow bags, 5 mirrored blue bags.
mirrored black bags contain 5 drab crimson bags.
dull green bags contain 4 shiny orange bags.
dark cyan bags contain 3 pale gold bags.
faded black bags contain 2 clear beige bags, 4 bright red bags, 1 striped black bag.
wavy indigo bags contain 2 pale brown bags.
dim white bags contain 3 clear violet bags, 3 vibrant indigo bags.
muted green bags contain 1 bright white bag.
posh coral bags contain 3 shiny purple bags, 2 light brown bags, 3 pale gray bags, 1 dull turquoise bag.
dull white bags contain 5 bright plum bags, 5 light green bags, 3 plaid purple bags, 1 faded white bag.
bright salmon bags contain 3 dull indigo bags, 3 muted plum bags, 3 muted bronze bags.
dotted green bags contain 1 posh purple bag.
posh salmon bags contain 1 shiny brown bag, 2 dark red bags, 3 drab gold bags.
pale olive bags contain 4 shiny maroon bags, 3 shiny white bags, 2 dim lavender bags.
dotted lime bags contain 4 mirrored plum bags, 2 dotted blue bags, 3 light salmon bags.
dim blue bags contain 5 shiny lime bags, 5 wavy cyan bags, 3 dotted cyan bags, 5 vibrant turquoise bags.
dim gold bags contain 3 wavy lavender bags, 4 dark silver bags, 4 striped green bags, 4 posh plum bags.
drab lime bags contain 3 clear black bags.
dim red bags contain 1 mirrored olive bag, 1 plaid violet bag.
posh white bags contain 3 pale indigo bags, 1 pale black bag, 5 light green bags, 2 light maroon bags.
dim brown bags contain 5 dotted black bags, 5 dotted red bags.
drab gold bags contain 4 mirrored orange bags, 5 drab beige bags, 4 pale gold bags.
shiny green bags contain 5 mirrored gray bags, 4 pale maroon bags, 1 striped coral bag.
clear teal bags contain 3 vibrant crimson bags, 4 posh teal bags, 5 striped lime bags, 3 plaid green bags.
wavy coral bags contain 3 drab beige bags, 2 drab magenta bags.
plaid tan bags contain 4 faded yellow bags, 4 dull gray bags, 2 shiny lime bags.
shiny blue bags contain 1 faded white bag, 5 dull turquoise bags.
striped green bags contain 4 dull blue bags, 1 muted tan bag, 1 wavy lavender bag, 1 muted gray bag.
wavy tan bags contain 4 wavy bronze bags, 2 pale plum bags, 2 plaid indigo bags, 2 light lavender bags.
faded violet bags contain 5 dull turquoise bags, 4 bright orange bags, 5 drab lavender bags.
striped magenta bags contain 5 dotted orange bags, 5 wavy red bags.
drab beige bags contain 4 pale gold bags.
shiny coral bags contain 3 mirrored bronze bags.
faded indigo bags contain 5 clear violet bags, 5 vibrant fuchsia bags, 5 posh violet bags.
muted maroon bags contain 4 wavy coral bags.
pale violet bags contain 1 clear olive bag, 1 wavy fuchsia bag, 1 dull lime bag.
wavy chartreuse bags contain 2 faded blue bags.
faded purple bags contain 5 clear olive bags, 1 wavy orange bag, 2 posh beige bags, 1 mirrored salmon bag.
dull fuchsia bags contain 2 dark tan bags.
light gold bags contain 1 pale blue bag, 4 wavy cyan bags, 1 shiny chartreuse bag.
bright tan bags contain 2 shiny brown bags, 4 drab gold bags.
plaid fuchsia bags contain 4 shiny gold bags, 1 mirrored orange bag, 4 dark chartreuse bags, 1 faded tomato bag.
striped indigo bags contain 4 clear brown bags, 3 wavy silver bags, 1 plaid teal bag, 3 pale maroon bags.
drab salmon bags contain 1 wavy bronze bag, 2 pale violet bags, 5 plaid indigo bags.
muted teal bags contain 1 wavy maroon bag, 2 pale purple bags, 4 clear blue bags, 2 bright salmon bags.
clear violet bags contain 2 light chartreuse bags, 1 light silver bag, 4 dark brown bags.
posh red bags contain 4 light lavender bags, 4 plaid brown bags.
muted gray bags contain 1 pale olive bag.
posh black bags contain 1 muted maroon bag.
dotted silver bags contain 3 dull turquoise bags, 4 dotted black bags, 3 mirrored black bags.
wavy olive bags contain 3 dotted coral bags, 5 dim silver bags.
plaid olive bags contain 3 muted plum bags.
dim lavender bags contain 2 pale gold bags, 5 mirrored turquoise bags, 1 dull lime bag, 1 dull beige bag.
dark silver bags contain 4 light silver bags, 1 plaid fuchsia bag.
shiny tomato bags contain 2 pale coral bags, 4 dim lavender bags, 1 dim purple bag, 3 wavy magenta bags.
dull purple bags contain 1 shiny maroon bag.
wavy orange bags contain 2 dim black bags, 1 wavy aqua bag, 4 dull turquoise bags, 3 wavy bronze bags.
dim gray bags contain 2 faded bronze bags, 3 dull beige bags, 3 pale purple bags, 5 drab gold bags.
muted yellow bags contain 4 wavy aqua bags.
striped maroon bags contain 1 plaid plum bag, 5 dotted chartreuse bags, 3 dotted cyan bags, 2 bright plum bags.
muted white bags contain 2 dark blue bags, 4 posh aqua bags, 5 mirrored violet bags, 5 posh lavender bags.
wavy cyan bags contain 3 dim black bags, 2 striped lime bags.
striped teal bags contain 2 bright blue bags, 1 vibrant black bag.
posh olive bags contain 2 dim plum bags, 5 shiny turquoise bags, 1 vibrant tomato bag, 5 bright magenta bags.
dotted crimson bags contain 2 clear gray bags.
clear plum bags contain 4 vibrant teal bags.
vibrant chartreuse bags contain 4 posh bronze bags, 5 light purple bags.
vibrant teal bags contain 3 clear blue bags, 4 muted tan bags, 2 wavy fuchsia bags.
striped beige bags contain 4 faded tomato bags.
muted bronze bags contain 1 clear lavender bag, 4 mirrored gray bags.
light lavender bags contain 2 plaid turquoise bags.
dotted white bags contain 1 light yellow bag, 5 dull maroon bags, 5 posh brown bags, 2 clear olive bags.
pale teal bags contain 4 dark aqua bags, 5 dull violet bags.
wavy lime bags contain 2 drab red bags, 1 clear olive bag.
vibrant black bags contain 3 dim tan bags.
pale coral bags contain 5 dull gold bags.
plaid maroon bags contain 2 light fuchsia bags.
wavy aqua bags contain 5 dark coral bags, 5 mirrored orange bags, 3 muted aqua bags.
bright blue bags contain 3 faded bronze bags, 1 bright red bag, 5 mirrored coral bags.
wavy silver bags contain 1 posh olive bag, 1 shiny lime bag, 3 shiny beige bags.
faded maroon bags contain 1 dull tan bag.
light yellow bags contain 5 light lavender bags, 5 pale black bags, 1 plaid brown bag, 3 pale violet bags.
wavy fuchsia bags contain no other bags.
pale plum bags contain 4 light teal bags, 2 pale black bags, 1 pale green bag, 4 wavy bronze bags.
drab white bags contain 3 dark tan bags, 5 muted orange bags, 5 dull violet bags, 1 mirrored fuchsia bag.
pale salmon bags contain 5 faded violet bags, 1 clear cyan bag.
clear crimson bags contain 4 pale lavender bags, 1 dull turquoise bag, 2 mirrored blue bags.
posh gray bags contain 4 muted brown bags, 3 shiny teal bags.
dark aqua bags contain 1 shiny maroon bag, 5 dim brown bags, 4 striped maroon bags, 5 dim blue bags.
muted red bags contain 3 muted chartreuse bags, 4 wavy red bags, 1 drab tomato bag, 3 shiny yellow bags.
mirrored purple bags contain 2 clear blue bags, 2 mirrored lavender bags, 1 pale gold bag, 3 bright silver bags.
striped yellow bags contain 4 light silver bags, 2 dotted red bags, 3 mirrored black bags, 2 faded tomato bags.
dotted teal bags contain 1 drab bronze bag, 3 muted red bags, 5 mirrored gray bags, 4 bright olive bags.
clear turquoise bags contain 5 dim bronze bags, 3 faded silver bags, 1 dotted olive bag.
dull lavender bags contain 5 clear olive bags, 1 mirrored orange bag.
mirrored indigo bags contain 3 bright fuchsia bags, 3 dotted salmon bags.
shiny salmon bags contain 2 bright turquoise bags, 1 light plum bag.
plaid red bags contain 3 clear brown bags, 4 drab tomato bags, 2 plaid indigo bags.
drab black bags contain 5 dull maroon bags, 2 dark silver bags, 5 bright red bags, 5 bright cyan bags.
pale purple bags contain 5 light blue bags, 1 dull maroon bag.
plaid crimson bags contain 2 dark turquoise bags, 5 mirrored bronze bags, 5 dull gold bags, 2 posh yellow bags.
striped lime bags contain 2 wavy lime bags, 2 shiny teal bags.
plaid yellow bags contain 5 muted chartreuse bags.
dark violet bags contain 2 plaid fuchsia bags, 2 muted bronze bags, 3 posh green bags.
bright red bags contain 4 faded plum bags, 3 dim lavender bags.
striped purple bags contain 3 mirrored turquoise bags, 1 shiny yellow bag.
shiny red bags contain 4 wavy coral bags, 3 vibrant salmon bags.
striped cyan bags contain 2 dark yellow bags.
posh magenta bags contain 3 vibrant indigo bags, 2 pale violet bags, 3 bright fuchsia bags, 1 shiny teal bag.
drab cyan bags contain 3 plaid gold bags, 3 clear black bags, 5 striped green bags.
dim chartreuse bags contain 2 wavy coral bags, 2 striped black bags, 4 dotted tomato bags, 1 muted maroon bag.
faded beige bags contain 5 dim tomato bags, 3 plaid turquoise bags.
mirrored teal bags contain 4 posh silver bags, 3 dotted brown bags.
light turquoise bags contain 2 bright gray bags.
drab fuchsia bags contain 4 shiny brown bags, 1 muted olive bag.
mirrored bronze bags contain 3 dull turquoise bags, 3 mirrored turquoise bags, 2 drab red bags.
shiny fuchsia bags contain 3 striped gray bags, 3 faded blue bags.
dim beige bags contain 4 dark blue bags, 1 vibrant aqua bag, 2 shiny brown bags, 5 dull violet bags.
drab gray bags contain 1 wavy maroon bag.
drab orange bags contain 5 plaid brown bags, 1 vibrant yellow bag, 1 mirrored beige bag, 4 pale white bags.
dull magenta bags contain 4 light purple bags, 3 dark brown bags.
vibrant magenta bags contain 4 clear gray bags, 2 shiny aqua bags, 4 striped yellow bags, 3 shiny magenta bags.
drab tomato bags contain 5 drab crimson bags, 2 dull gold bags, 4 light silver bags.
wavy violet bags contain 3 faded plum bags.
mirrored fuchsia bags contain 1 mirrored black bag.
shiny white bags contain 4 light silver bags, 3 dotted fuchsia bags, 2 faded brown bags, 1 bright bronze bag.
clear chartreuse bags contain 4 plaid cyan bags, 1 striped gold bag.
muted turquoise bags contain 3 drab crimson bags, 3 drab olive bags.
faded aqua bags contain 1 dotted teal bag.
vibrant gold bags contain 2 light green bags, 1 dotted gold bag.
drab plum bags contain 5 dim salmon bags, 3 dotted teal bags.
dotted chartreuse bags contain 2 dull turquoise bags, 1 bright chartreuse bag, 4 faded tan bags, 1 mirrored lavender bag.
dull silver bags contain 1 mirrored indigo bag.
pale yellow bags contain 4 drab coral bags.
clear black bags contain 3 dim coral bags.
faded gold bags contain 3 faded chartreuse bags, 1 posh gray bag, 3 drab tomato bags.
muted chartreuse bags contain 1 clear olive bag.
shiny indigo bags contain 3 vibrant yellow bags, 2 plaid orange bags.
dull indigo bags contain 5 bright fuchsia bags, 2 shiny yellow bags, 2 plaid indigo bags, 4 bright olive bags.
faded silver bags contain 2 vibrant olive bags, 4 muted tomato bags.
pale green bags contain 1 pale red bag, 5 dull lavender bags, 4 clear olive bags.
dull violet bags contain 1 shiny black bag, 1 shiny fuchsia bag, 3 plaid indigo bags, 2 posh crimson bags.
dotted lavender bags contain 2 striped lavender bags.
clear orange bags contain 1 dotted cyan bag, 4 clear tomato bags.
bright crimson bags contain 1 dull black bag, 2 dim tomato bags, 1 drab bronze bag, 4 wavy orange bags.
dark orange bags contain 1 vibrant teal bag, 3 dull maroon bags, 4 light purple bags.
bright teal bags contain 5 mirrored tomato bags, 4 dull lime bags, 4 shiny olive bags.
vibrant beige bags contain 2 dark turquoise bags.
dark yellow bags contain 5 dark orange bags, 4 pale red bags, 2 striped fuchsia bags, 5 faded turquoise bags.
bright fuchsia bags contain 5 striped gray bags, 3 light teal bags, 5 mirrored orange bags.
clear maroon bags contain 2 dotted yellow bags, 1 mirrored green bag, 4 dotted coral bags, 3 dull indigo bags.
dim lime bags contain 3 dull fuchsia bags.
striped olive bags contain 5 wavy fuchsia bags, 2 light purple bags.
dull gray bags contain 4 dark beige bags, 1 mirrored white bag.
dim orange bags contain 5 mirrored white bags.
faded salmon bags contain 2 muted cyan bags.
clear red bags contain 2 dim violet bags, 1 shiny purple bag, 3 faded salmon bags, 2 mirrored brown bags.
plaid white bags contain 1 shiny aqua bag, 1 vibrant teal bag.
shiny violet bags contain 3 striped lime bags, 5 mirrored chartreuse bags, 3 pale red bags, 1 muted orange bag.
wavy green bags contain 2 dark tomato bags.
pale lavender bags contain 3 pale orange bags.
dark fuchsia bags contain 4 pale turquoise bags, 4 pale plum bags, 5 light purple bags, 3 dark beige bags.
posh indigo bags contain 2 pale violet bags, 5 shiny coral bags, 1 faded chartreuse bag, 2 plaid indigo bags.
light brown bags contain 1 posh black bag, 4 clear cyan bags.
faded red bags contain 1 drab maroon bag.
dark bronze bags contain 1 pale green bag, 4 shiny blue bags, 1 mirrored lime bag.
pale orange bags contain 4 plaid violet bags, 5 dim silver bags, 1 dim cyan bag, 3 clear blue bags.
dull brown bags contain 1 light tomato bag, 4 pale tomato bags.
mirrored plum bags contain 1 shiny tan bag, 5 wavy brown bags.
dull lime bags contain 4 mirrored orange bags.
drab yellow bags contain 3 clear teal bags, 3 posh aqua bags, 4 posh fuchsia bags, 3 posh turquoise bags.
bright cyan bags contain 5 muted lavender bags, 1 wavy purple bag.
drab chartreuse bags contain 1 wavy tan bag.
drab tan bags contain 5 drab bronze bags, 5 pale green bags, 1 wavy fuchsia bag.
shiny tan bags contain 1 pale red bag, 5 clear aqua bags.
dotted black bags contain 2 dim black bags.
dull blue bags contain 3 clear blue bags, 5 bright gold bags, 4 pale tomato bags, 4 drab beige bags.
light olive bags contain 2 mirrored salmon bags, 2 drab violet bags, 2 bright beige bags.
mirrored magenta bags contain 4 drab tomato bags, 5 clear teal bags, 3 dark coral bags, 5 shiny white bags.
bright silver bags contain 1 posh brown bag.
vibrant blue bags contain 3 clear tan bags.
dark indigo bags contain 3 dim aqua bags, 5 bright aqua bags.
bright brown bags contain 3 muted purple bags, 1 muted blue bag, 1 light green bag, 1 dark red bag.
dark salmon bags contain 3 dotted fuchsia bags, 2 dim tan bags.
mirrored maroon bags contain 2 posh purple bags.
faded crimson bags contain 1 posh white bag, 2 muted blue bags, 3 dark fuchsia bags, 3 light red bags.
bright lavender bags contain 4 bright chartreuse bags, 5 muted purple bags, 4 dull aqua bags, 1 shiny blue bag.
mirrored orange bags contain no other bags.
wavy bronze bags contain no other bags.
wavy yellow bags contain 2 mirrored yellow bags, 1 muted maroon bag, 2 striped purple bags, 4 pale silver bags.
drab magenta bags contain 3 dim indigo bags, 2 wavy bronze bags, 5 shiny yellow bags.
wavy blue bags contain 3 mirrored gold bags, 3 shiny lavender bags, 1 dim teal bag, 4 clear crimson bags.
clear purple bags contain 2 dark beige bags.
light cyan bags contain 1 dotted white bag, 5 muted yellow bags, 1 wavy lime bag.
clear yellow bags contain 2 plaid olive bags, 1 muted red bag, 1 mirrored bronze bag, 5 striped violet bags.
light plum bags contain 5 dotted fuchsia bags, 4 vibrant white bags, 5 muted red bags, 5 drab red bags.
vibrant fuchsia bags contain 1 posh green bag.
pale maroon bags contain 5 light purple bags, 5 shiny aqua bags, 3 clear teal bags, 2 clear green bags.
dark teal bags contain 2 dark lavender bags, 4 striped black bags, 1 drab lime bag, 2 dim teal bags.
clear fuchsia bags contain 2 shiny violet bags, 4 posh aqua bags, 4 bright red bags.
dotted magenta bags contain 2 dull olive bags, 1 clear magenta bag, 5 light beige bags, 1 dark brown bag.
muted black bags contain 2 clear blue bags, 2 muted red bags.
dim magenta bags contain 4 plaid silver bags, 4 plaid olive bags.
striped lavender bags contain 4 clear blue bags.
dull beige bags contain no other bags.
wavy lavender bags contain 2 shiny blue bags, 3 faded tomato bags, 5 dotted tan bags, 5 dull maroon bags.
light maroon bags contain 5 dim violet bags, 3 wavy turquoise bags, 3 mirrored salmon bags.
plaid green bags contain 1 dim teal bag, 5 pale brown bags, 2 striped chartreuse bags.
shiny plum bags contain 1 posh lavender bag, 3 vibrant red bags, 5 dark maroon bags, 1 dotted bronze bag.
plaid turquoise bags contain 4 light silver bags.
dim cyan bags contain 1 shiny coral bag, 4 bright lime bags.
shiny lime bags contain 3 wavy green bags, 5 posh white bags, 1 striped purple bag.
dim maroon bags contain 1 muted crimson bag.
dim aqua bags contain 1 dotted silver bag, 3 faded blue bags, 4 striped purple bags.
muted blue bags contain 2 posh yellow bags, 3 bright tan bags, 1 vibrant salmon bag.
vibrant olive bags contain 5 clear aqua bags.
plaid black bags contain 3 striped orange bags.
vibrant violet bags contain 2 pale gold bags.
light bronze bags contain 1 shiny maroon bag, 3 light blue bags.
pale magenta bags contain 5 pale tomato bags.
vibrant yellow bags contain 4 striped gray bags, 4 dotted yellow bags, 1 wavy fuchsia bag, 3 wavy purple bags.
shiny black bags contain 5 shiny yellow bags, 3 light purple bags, 1 pale olive bag, 1 clear blue bag.
dim crimson bags contain 5 posh gray bags, 1 bright chartreuse bag.
plaid salmon bags contain 2 wavy fuchsia bags, 1 posh olive bag, 2 dim tan bags.
shiny cyan bags contain 5 light green bags, 1 clear olive bag, 5 pale olive bags.
pale silver bags contain 3 mirrored orange bags, 1 light lavender bag, 2 faded tomato bags, 1 muted chartreuse bag.
light red bags contain 4 shiny gray bags, 4 wavy salmon bags, 4 clear lime bags.
dark turquoise bags contain 4 mirrored orange bags, 4 vibrant brown bags, 4 striped brown bags, 4 mirrored purple bags.
dark gray bags contain 1 pale violet bag, 5 striped turquoise bags, 2 wavy red bags, 2 light bronze bags.
pale gray bags contain 5 drab tan bags, 3 clear gray bags.
striped gold bags contain 2 dotted beige bags.
muted cyan bags contain 2 light bronze bags, 3 clear green bags.
faded white bags contain 3 striped tomato bags, 4 shiny black bags, 2 drab tomato bags.
clear lime bags contain 3 dotted fuchsia bags, 3 vibrant purple bags, 1 shiny bronze bag, 3 muted green bags.
dull bronze bags contain 5 plaid turquoise bags, 4 dark crimson bags, 3 pale yellow bags, 3 posh chartreuse bags.
mirrored cyan bags contain 4 clear olive bags, 2 vibrant purple bags, 4 dull olive bags.
pale bronze bags contain 3 vibrant purple bags, 4 dotted silver bags, 2 mirrored black bags.
pale blue bags contain 1 dark brown bag, 2 light brown bags, 2 drab lime bags.
clear bronze bags contain 4 clear blue bags, 2 light chartreuse bags, 1 pale violet bag.
clear aqua bags contain 2 clear cyan bags.
plaid plum bags contain 3 faded plum bags.
mirrored lime bags contain 4 posh brown bags.
plaid lavender bags contain 1 bright beige bag, 5 bright lime bags, 4 faded aqua bags.
drab crimson bags contain 2 shiny yellow bags, 1 posh brown bag.
shiny beige bags contain 2 muted aqua bags, 1 vibrant purple bag, 4 pale black bags, 3 dark chartreuse bags.
dim green bags contain 4 bright yellow bags, 1 shiny fuchsia bag, 1 wavy tomato bag, 4 shiny black bags.
drab coral bags contain 5 dark coral bags.
posh turquoise bags contain 1 dim violet bag, 2 faded turquoise bags.
dotted plum bags contain 2 dull salmon bags, 5 light green bags, 3 pale red bags, 2 posh yellow bags.
posh yellow bags contain 1 mirrored salmon bag, 1 drab tomato bag, 3 dark tomato bags, 2 dark chartreuse bags.
faded orange bags contain 1 drab yellow bag.
dull teal bags contain 2 plaid crimson bags, 5 shiny violet bags, 1 dull coral bag.
faded brown bags contain 5 muted red bags.
bright chartreuse bags contain 5 clear lime bags.
mirrored tan bags contain 4 striped lavender bags, 4 light plum bags.
clear indigo bags contain 5 plaid fuchsia bags, 2 plaid brown bags, 5 striped gray bags.
light teal bags contain 1 dull beige bag, 1 mirrored turquoise bag, 1 mirrored orange bag, 5 wavy bronze bags.
shiny gold bags contain 2 dark coral bags, 1 mirrored orange bag.
striped white bags contain 5 dull fuchsia bags.
wavy white bags contain 5 dull chartreuse bags, 5 wavy green bags.
mirrored beige bags contain 4 bright white bags, 5 plaid magenta bags, 3 plaid crimson bags, 1 shiny teal bag.
muted purple bags contain 5 pale black bags, 5 wavy aqua bags.
vibrant crimson bags contain 5 dim bronze bags, 2 vibrant plum bags.
striped aqua bags contain 3 plaid cyan bags, 3 light beige bags, 2 drab cyan bags.
dull crimson bags contain 1 light white bag, 3 drab indigo bags, 1 clear fuchsia bag.
clear magenta bags contain 4 light brown bags.
posh maroon bags contain 1 faded plum bag, 3 light olive bags.
bright violet bags contain 2 dotted bronze bags, 4 clear teal bags, 3 striped magenta bags, 3 muted indigo bags.
faded coral bags contain 5 dark violet bags, 3 plaid teal bags.
faded bronze bags contain 5 dotted fuchsia bags, 5 wavy bronze bags, 2 dotted purple bags, 5 dim black bags.
light beige bags contain 2 shiny olive bags.
mirrored white bags contain 1 dark indigo bag, 1 drab maroon bag, 3 shiny beige bags, 5 light blue bags.
muted silver bags contain 4 wavy lime bags, 1 dark tomato bag, 1 bright fuchsia bag, 4 dotted olive bags.
wavy gray bags contain 2 mirrored turquoise bags, 5 light lavender bags, 3 muted chartreuse bags.
clear salmon bags contain 5 bright tomato bags, 2 wavy aqua bags.
dull turquoise bags contain 1 pale tomato bag.
shiny yellow bags contain 4 dull lime bags, 3 shiny maroon bags, 3 light silver bags.
mirrored aqua bags contain 3 posh magenta bags, 4 drab tomato bags.
dull orange bags contain 5 dim teal bags, 3 light olive bags, 2 light magenta bags, 2 plaid orange bags.
plaid blue bags contain 3 pale purple bags, 1 shiny turquoise bag.
dim olive bags contain 3 shiny tomato bags, 4 dim black bags, 1 light plum bag.
dim fuchsia bags contain 1 dark chartreuse bag, 1 mirrored cyan bag, 3 plaid red bags.
dark plum bags contain 1 light yellow bag, 1 mirrored lime bag, 2 bright salmon bags.
faded tomato bags contain 2 muted chartreuse bags, 4 dark chartreuse bags.
dotted aqua bags contain 2 dull lavender bags, 4 pale tomato bags.
plaid lime bags contain 3 vibrant indigo bags, 1 muted red bag, 1 posh silver bag, 1 dim indigo bag.
vibrant lime bags contain 5 drab turquoise bags, 2 vibrant chartreuse bags, 4 clear blue bags.
posh brown bags contain 2 wavy red bags, 2 drab beige bags.
bright beige bags contain 3 faded blue bags, 4 muted red bags, 5 mirrored lavender bags, 2 pale red bags.
dotted yellow bags contain 5 striped gray bags, 3 pale gold bags, 2 clear olive bags, 5 mirrored yellow bags.
dull gold bags contain 5 clear tomato bags, 5 drab beige bags, 4 bright silver bags, 4 drab crimson bags.
dark maroon bags contain 2 wavy turquoise bags, 4 dark chartreuse bags.
drab olive bags contain 4 faded bronze bags.
posh plum bags contain 2 dull tan bags.
light indigo bags contain 2 wavy yellow bags.
vibrant tan bags contain 5 shiny teal bags.
dim purple bags contain 5 light olive bags, 5 mirrored violet bags.
vibrant gray bags contain 3 bright turquoise bags, 4 dim maroon bags, 4 vibrant chartreuse bags, 2 faded turquoise bags.
pale tan bags contain 2 light salmon bags, 3 dotted silver bags, 5 clear lime bags, 1 shiny teal bag.
dark tan bags contain 5 dim indigo bags, 3 shiny beige bags, 5 dim tan bags, 2 pale black bags.
dotted tomato bags contain 3 shiny black bags, 3 pale crimson bags, 2 dim tan bags.
posh violet bags contain 4 posh crimson bags, 1 muted fuchsia bag, 4 pale cyan bags, 3 mirrored gold bags.
clear tan bags contain 2 bright tan bags, 5 faded beige bags.
bright bronze bags contain no other bags.
dotted brown bags contain 2 dotted black bags, 3 wavy coral bags.
muted violet bags contain 3 faded magenta bags, 3 bright magenta bags.
faded plum bags contain 2 dull lime bags, 4 dim coral bags, 5 shiny maroon bags, 3 pale red bags.
dark lime bags contain 4 dim tomato bags.
dull aqua bags contain 4 drab beige bags, 4 drab maroon bags.
posh purple bags contain 2 vibrant purple bags, 5 shiny gold bags, 2 plaid turquoise bags.
dull plum bags contain 5 dark brown bags, 4 muted black bags, 1 striped lavender bag.
faded turquoise bags contain 3 shiny yellow bags, 4 faded plum bags, 5 pale violet bags, 4 dim lavender bags.
striped bronze bags contain 1 muted bronze bag, 4 clear green bags, 4 shiny fuchsia bags, 5 pale red bags.
mirrored blue bags contain 4 clear tomato bags, 5 shiny orange bags, 1 dull beige bag, 1 striped silver bag.
shiny orange bags contain 2 light bronze bags, 4 dark tomato bags, 2 drab gold bags.
shiny gray bags contain 1 dark maroon bag.
drab red bags contain 5 wavy purple bags.
posh fuchsia bags contain 3 mirrored chartreuse bags, 3 vibrant teal bags.
light crimson bags contain 2 shiny aqua bags, 2 plaid chartreuse bags, 1 shiny gold bag, 1 wavy maroon bag.
vibrant white bags contain 1 shiny brown bag, 5 light purple bags, 4 dull olive bags, 5 clear cyan bags.
dark green bags contain 5 drab indigo bags, 1 striped coral bag.
striped fuchsia bags contain 5 vibrant plum bags.
bright turquoise bags contain 1 striped fuchsia bag, 3 posh purple bags, 5 dim aqua bags.
striped tan bags contain 1 dotted cyan bag.
shiny magenta bags contain 2 dark yellow bags, 1 faded silver bag, 4 muted orange bags, 3 bright fuchsia bags.
dim violet bags contain 4 wavy bronze bags, 1 bright bronze bag.
vibrant orange bags contain 2 faded chartreuse bags, 2 vibrant turquoise bags, 2 dull white bags.
pale gold bags contain 5 light lavender bags, 4 mirrored orange bags, 4 muted chartreuse bags.
pale red bags contain no other bags.
plaid brown bags contain 3 mirrored orange bags.
posh aqua bags contain 5 light blue bags, 1 dark indigo bag, 1 pale red bag, 2 vibrant turquoise bags.
muted lime bags contain 1 dim chartreuse bag, 1 dotted olive bag, 4 pale violet bags, 3 muted fuchsia bags.
dotted fuchsia bags contain 4 light teal bags, 1 muted chartreuse bag, 3 dark chartreuse bags, 4 dim lavender bags.
dim turquoise bags contain 4 clear beige bags, 1 posh magenta bag, 3 faded brown bags.
muted coral bags contain 4 dark salmon bags, 1 pale tomato bag, 2 shiny brown bags.
dull chartreuse bags contain 3 shiny white bags, 4 shiny blue bags, 1 pale crimson bag.
muted indigo bags contain 4 posh silver bags, 1 pale maroon bag, 1 dotted red bag, 1 drab coral bag.
dim salmon bags contain 5 pale black bags, 3 dark salmon bags, 3 pale red bags, 5 dotted purple bags.
dotted bronze bags contain 1 dim aqua bag, 2 mirrored maroon bags, 4 muted olive bags, 3 dull fuchsia bags.
drab silver bags contain 2 dim coral bags, 3 dim salmon bags.
striped salmon bags contain 2 dotted purple bags, 2 posh green bags, 3 plaid beige bags.
mirrored tomato bags contain 5 faded tomato bags, 4 wavy red bags.
dotted cyan bags contain 1 posh white bag, 3 striped lime bags, 4 dotted purple bags, 2 muted red bags.
striped blue bags contain 4 dark silver bags, 4 pale silver bags.
faded green bags contain 3 dotted violet bags, 3 pale turquoise bags.
striped gray bags contain 2 pale tomato bags, 2 dark chartreuse bags, 5 muted red bags, 3 pale violet bags.
pale lime bags contain 4 posh maroon bags, 2 drab plum bags, 3 drab coral bags.
posh beige bags contain 4 wavy purple bags, 2 shiny gold bags, 1 dark tan bag.
dotted tan bags contain 2 bright silver bags, 1 posh bronze bag, 2 dim coral bags, 2 dim tomato bags.
shiny brown bags contain 4 dim violet bags, 5 bright silver bags.
mirrored silver bags contain 1 striped blue bag, 2 shiny teal bags, 2 light coral bags, 1 dim beige bag.
clear silver bags contain 4 mirrored fuchsia bags, 5 dull olive bags, 1 plaid crimson bag.
dotted orange bags contain 1 mirrored turquoise bag, 3 wavy lime bags, 3 dotted yellow bags, 5 muted lavender bags.
shiny purple bags contain 1 shiny beige bag, 3 plaid chartreuse bags.
drab brown bags contain 2 dull indigo bags, 3 mirrored cyan bags, 1 dim crimson bag.
vibrant tomato bags contain 5 faded plum bags.
light tan bags contain 1 dark gold bag, 3 light salmon bags.
vibrant cyan bags contain 4 shiny aqua bags.
dotted purple bags contain 4 wavy coral bags.
vibrant red bags contain 4 faded tan bags, 1 drab aqua bag, 4 striped black bags.
faded lime bags contain 3 light olive bags, 3 striped yellow bags.
posh gold bags contain 1 dull indigo bag, 1 mirrored chartreuse bag.
shiny aqua bags contain 2 vibrant turquoise bags.
light orange bags contain 3 vibrant violet bags, 2 dotted yellow bags, 1 dark gray bag, 5 striped chartreuse bags.
pale black bags contain 1 plaid brown bag, 2 muted chartreuse bags.
drab violet bags contain 5 clear cyan bags, 1 pale gold bag, 4 drab salmon bags, 2 posh orange bags.
dim silver bags contain 5 clear cyan bags.
muted beige bags contain 1 dim tomato bag, 3 light olive bags.
drab turquoise bags contain 3 plaid orange bags, 1 light purple bag.
dark red bags contain 2 vibrant yellow bags, 4 mirrored fuchsia bags, 5 posh bronze bags, 2 dark gold bags.
vibrant maroon bags contain 5 drab beige bags, 1 dark beige bag, 1 dark brown bag, 4 pale maroon bags.
wavy magenta bags contain 3 light tomato bags, 5 muted gold bags, 3 muted cyan bags.
shiny chartreuse bags contain 5 pale yellow bags.
dull cyan bags contain 4 bright maroon bags, 4 faded bronze bags.
light black bags contain 4 shiny teal bags, 5 faded beige bags, 1 muted gold bag, 4 plaid silver bags.
bright green bags contain 5 striped lime bags, 4 muted yellow bags, 4 light beige bags, 4 plaid violet bags.
striped orange bags contain 1 mirrored orange bag, 2 bright salmon bags.
muted plum bags contain 3 mirrored orange bags, 1 muted chartreuse bag, 2 dull olive bags, 4 dull brown bags.
pale white bags contain 5 light silver bags.
mirrored gray bags contain 1 mirrored violet bag, 5 striped gray bags, 5 dim aqua bags, 4 wavy fuchsia bags.
wavy beige bags contain 2 shiny beige bags.
dotted red bags contain 4 dotted purple bags, 3 muted aqua bags, 2 pale tomato bags, 4 pale bronze bags.
posh lavender bags contain 2 dotted tomato bags, 3 dim lavender bags, 2 muted gray bags, 3 light plum bags.
mirrored yellow bags contain 4 wavy gold bags.
wavy brown bags contain 3 dim fuchsia bags, 4 dull turquoise bags.
plaid chartreuse bags contain 5 plaid red bags.
shiny turquoise bags contain 2 faded turquoise bags, 3 muted tan bags.
mirrored brown bags contain 4 dotted fuchsia bags, 4 dim red bags, 2 dotted plum bags.
dim tan bags contain 1 drab tomato bag, 3 wavy red bags, 1 plaid turquoise bag, 2 clear lavender bags.
faded yellow bags contain 1 clear bronze bag, 3 clear olive bags.
bright orange bags contain 5 mirrored blue bags, 3 shiny blue bags.
striped red bags contain 5 bright green bags, 3 wavy yellow bags.
bright indigo bags contain 2 light maroon bags, 5 shiny bronze bags, 2 bright white bags.
light gray bags contain 2 dotted maroon bags.
light silver bags contain no other bags.
bright gold bags contain 2 striped fuchsia bags.
drab purple bags contain 5 light silver bags, 2 dull yellow bags, 2 shiny teal bags.
mirrored coral bags contain 4 dim plum bags.
light coral bags contain 3 dim lavender bags, 5 wavy green bags, 2 light bronze bags.
plaid indigo bags contain 3 mirrored orange bags, 5 pale gold bags.
dark crimson bags contain 4 dark turquoise bags, 4 plaid green bags, 4 clear fuchsia bags.
wavy teal bags contain 1 pale silver bag, 4 mirrored chartreuse bags, 4 shiny black bags.
dull tan bags contain 4 muted magenta bags.
light violet bags contain 4 faded yellow bags, 5 light maroon bags, 3 clear teal bags, 2 pale crimson bags.
striped silver bags contain 3 plaid fuchsia bags.
shiny crimson bags contain 5 muted yellow bags.
mirrored salmon bags contain 1 dim tan bag.
dotted coral bags contain 5 dark tan bags.
wavy gold bags contain 2 wavy turquoise bags, 4 dim indigo bags, 3 wavy bronze bags.
vibrant bronze bags contain 1 pale purple bag.
posh silver bags contain 4 posh chartreuse bags, 3 bright salmon bags, 1 dotted bronze bag, 3 shiny coral bags.
bright purple bags contain 5 clear beige bags, 3 pale yellow bags, 1 wavy tomato bag, 5 pale lavender bags.
muted tan bags contain 4 bright white bags, 5 dotted silver bags, 2 clear blue bags, 4 mirrored turquoise bags.
light white bags contain 2 shiny purple bags, 1 dull aqua bag, 2 vibrant blue bags.
faded fuchsia bags contain 2 dark green bags, 3 shiny violet bags, 4 drab plum bags, 3 vibrant olive bags.
clear gray bags contain 2 muted red bags.
dark tomato bags contain 4 dotted purple bags, 3 faded turquoise bags.
light magenta bags contain 1 dull fuchsia bag.
light purple bags contain 3 drab magenta bags, 4 dark coral bags, 3 light silver bags.
vibrant turquoise bags contain 4 dotted black bags.
dim indigo bags contain 4 muted chartreuse bags.
dim bronze bags contain 3 drab gold bags, 4 clear brown bags, 2 muted tan bags.
clear green bags contain 2 plaid fuchsia bags, 1 wavy bronze bag.
dull maroon bags contain 2 wavy yellow bags, 1 dotted fuchsia bag, 4 mirrored yellow bags.
dotted gray bags contain 5 dotted white bags, 2 pale tomato bags, 5 bright tan bags, 3 plaid turquoise bags.
plaid gray bags contain 5 wavy lime bags, 4 dull aqua bags.
mirrored crimson bags contain 1 shiny lavender bag, 4 plaid purple bags.
shiny silver bags contain 5 vibrant lavender bags, 3 light coral bags, 5 dark aqua bags.
dotted salmon bags contain 3 pale tomato bags.
muted crimson bags contain 4 clear olive bags, 3 dull plum bags, 1 dark gray bag, 1 wavy lime bag.
muted lavender bags contain 1 bright chartreuse bag, 3 pale yellow bags, 3 pale crimson bags.
posh tan bags contain 4 faded tomato bags, 3 dotted olive bags, 4 striped maroon bags.
vibrant salmon bags contain 4 vibrant indigo bags, 1 plaid orange bag.
faded chartreuse bags contain 2 mirrored orange bags, 1 pale crimson bag.
striped tomato bags contain 3 dim tan bags, 2 wavy gold bags.
dim yellow bags contain 1 clear purple bag, 1 wavy red bag, 1 drab bronze bag.
wavy tomato bags contain 5 dotted tomato bags, 3 dim salmon bags, 5 light tan bags, 3 dull brown bags.
bright lime bags contain 3 mirrored turquoise bags, 3 dotted tan bags, 3 posh black bags, 5 wavy gold bags.
muted brown bags contain 4 shiny indigo bags, 4 muted tan bags, 3 wavy tan bags.
pale cyan bags contain 1 dotted black bag.
drab green bags contain 1 muted cyan bag.
dim teal bags contain 4 faded lavender bags, 2 pale violet bags.
drab lavender bags contain 1 pale gold bag, 5 vibrant brown bags, 4 posh indigo bags, 1 plaid olive bag.
dotted turquoise bags contain 2 muted lime bags, 4 striped indigo bags.
clear white bags contain 2 bright lime bags, 1 mirrored coral bag.
dark purple bags contain 5 wavy crimson bags, 5 light olive bags.
light tomato bags contain 4 posh black bags, 1 faded blue bag, 5 dim black bags, 3 dull lime bags.
bright aqua bags contain 3 clear tomato bags.
bright tomato bags contain 1 clear green bag, 3 bright gray bags, 3 mirrored cyan bags, 5 posh chartreuse bags.
dull yellow bags contain 5 wavy purple bags, 3 light purple bags, 5 clear cyan bags, 2 wavy coral bags.
striped coral bags contain 3 mirrored black bags, 5 wavy coral bags, 2 posh white bags.
shiny bronze bags contain 2 dim tomato bags, 3 dull lime bags.
vibrant aqua bags contain 4 dull olive bags, 4 mirrored turquoise bags, 1 vibrant teal bag.
faded magenta bags contain 3 plaid chartreuse bags, 5 plaid white bags, 1 drab lime bag, 3 mirrored fuchsia bags.
pale beige bags contain 3 wavy gray bags, 5 mirrored tomato bags, 1 bright beige bag.
pale crimson bags contain 1 pale violet bag, 4 pale black bags, 5 dim tomato bags, 1 mirrored turquoise bag.
dark blue bags contain 4 dark maroon bags, 1 faded brown bag, 2 wavy red bags.
posh tomato bags contain 2 posh beige bags, 4 dark maroon bags.
dim plum bags contain 2 muted crimson bags, 1 striped coral bag, 2 plaid yellow bags, 5 striped violet bags.
dull black bags contain 3 posh tomato bags, 1 vibrant yellow bag, 4 mirrored salmon bags, 2 light green bags.
wavy plum bags contain 4 wavy blue bags.
bright coral bags contain 4 posh gold bags, 2 plaid lime bags, 3 shiny black bags, 5 dim magenta bags.
posh blue bags contain 2 dotted turquoise bags, 2 bright olive bags, 2 plaid gold bags.
vibrant purple bags contain 2 mirrored orange bags, 4 dull lavender bags, 2 pale red bags.
dark white bags contain 4 drab lime bags.
wavy crimson bags contain 2 dark salmon bags, 5 wavy yellow bags, 2 bright bronze bags, 2 drab crimson bags.
clear coral bags contain 4 vibrant blue bags.
muted olive bags contain 3 plaid brown bags, 1 dim fuchsia bag, 2 posh yellow bags.
plaid violet bags contain 4 muted aqua bags.
plaid teal bags contain 4 bright tan bags, 2 dark tomato bags.
plaid purple bags contain 3 posh teal bags.
pale turquoise bags contain 2 faded purple bags, 4 mirrored maroon bags, 1 mirrored salmon bag.
muted fuchsia bags contain 5 bright bronze bags, 4 muted gold bags.
bright olive bags contain 2 faded blue bags, 3 faded turquoise bags, 5 light bronze bags, 3 dim tan bags.
bright gray bags contain 1 bright white bag, 2 muted orange bags.
light blue bags contain 1 dotted purple bag, 3 dim black bags, 3 posh purple bags, 2 mirrored violet bags.
posh bronze bags contain 3 plaid fuchsia bags, 1 drab gold bag, 4 mirrored orange bags, 4 light teal bags.
faded olive bags contain 3 striped blue bags, 3 dim salmon bags, 4 pale red bags, 2 dull maroon bags.
dull coral bags contain 2 wavy gold bags, 1 dark magenta bag.
dull olive bags contain 3 plaid turquoise bags, 3 plaid fuchsia bags, 4 wavy lime bags, 1 dotted black bag.
plaid orange bags contain 1 dark salmon bag, 1 striped turquoise bag.
clear blue bags contain 3 dim tan bags, 3 muted red bags, 3 faded tomato bags.
dark chartreuse bags contain 4 bright bronze bags, 1 dim indigo bag.
plaid tomato bags contain 5 shiny turquoise bags, 4 clear tan bags, 2 dotted bronze bags, 5 muted magenta bags.
mirrored green bags contain 2 light silver bags, 3 light brown bags, 4 vibrant purple bags, 2 posh green bags.
drab blue bags contain 4 drab indigo bags.
vibrant silver bags contain 3 striped violet bags, 1 striped white bag, 2 bright blue bags.
shiny olive bags contain 3 drab tomato bags.
dull red bags contain 4 dim cyan bags, 3 wavy lime bags.
vibrant plum bags contain 3 wavy coral bags, 2 light lavender bags.
faded cyan bags contain 1 dim brown bag, 5 posh indigo bags.
dotted violet bags contain 1 pale tomato bag, 5 dim plum bags.
striped brown bags contain 5 dim coral bags, 4 clear blue bags.
dotted indigo bags contain 5 clear salmon bags, 4 dim bronze bags.
posh cyan bags contain 2 vibrant magenta bags.
clear beige bags contain 1 bright olive bag.
dark black bags contain 4 dull crimson bags, 3 plaid green bags.
mirrored lavender bags contain 5 dark silver bags.
bright black bags contain 4 shiny brown bags, 1 vibrant crimson bag, 2 posh white bags, 3 wavy gold bags.
pale indigo bags contain 5 shiny gold bags, 3 light bronze bags, 3 striped turquoise bags.
plaid cyan bags contain 1 shiny tan bag, 4 dark tan bags, 1 dull yellow bag.
bright plum bags contain 1 pale chartreuse bag, 3 plaid red bags, 1 faded beige bag, 1 posh gray bag.
drab maroon bags contain 3 dull olive bags, 1 pale green bag, 2 mirrored turquoise bags.
pale chartreuse bags contain 4 dim tomato bags.
striped black bags contain 4 vibrant turquoise bags, 3 faded white bags, 4 light plum bags, 1 pale black bag.
dotted gold bags contain 5 posh teal bags, 5 striped violet bags.
posh orange bags contain 1 clear green bag.
light green bags contain 5 wavy turquoise bags, 2 pale red bags, 3 dark tomato bags, 3 dull turquoise bags.
plaid beige bags contain 4 light silver bags, 4 plaid turquoise bags, 5 drab purple bags, 4 clear lime bags.
mirrored violet bags contain 2 wavy fuchsia bags, 5 bright bronze bags.
striped plum bags contain 2 plaid indigo bags, 5 bright yellow bags, 1 wavy magenta bag.
bright yellow bags contain 3 wavy yellow bags.
drab aqua bags contain 5 dark red bags, 2 light gold bags.
plaid bronze bags contain 1 drab magenta bag.
faded tan bags contain 4 shiny aqua bags, 2 light chartreuse bags.
drab indigo bags contain 5 dotted beige bags, 3 dull lime bags, 1 striped violet bag, 4 vibrant tan bags.
dark gold bags contain 4 mirrored lime bags, 3 dark salmon bags.
muted orange bags contain 1 dull yellow bag, 2 shiny brown bags.
plaid gold bags contain 4 drab coral bags, 5 shiny cyan bags.
posh crimson bags contain 3 light brown bags, 3 bright bronze bags, 3 shiny yellow bags.
bright magenta bags contain 5 dim lavender bags.
light chartreuse bags contain 2 drab bronze bags, 3 dim coral bags.
dim tomato bags contain no other bags.
dark beige bags contain 3 mirrored yellow bags, 2 dull olive bags, 5 striped brown bags.
clear cyan bags contain 5 drab beige bags, 1 shiny yellow bag, 5 wavy purple bags, 5 dark chartreuse bags.
dark coral bags contain 3 dim lavender bags, 3 pale green bags, 4 mirrored orange bags, 1 dim coral bag.
light lime bags contain 5 mirrored brown bags.
shiny lavender bags contain 5 drab tan bags.
faded lavender bags contain 1 light salmon bag, 2 dotted purple bags, 5 pale gold bags, 3 muted gold bags.
pale tomato bags contain 5 dim coral bags.
plaid coral bags contain 3 dull blue bags, 1 posh white bag, 3 light orange bags.
dim coral bags contain no other bags.
dotted olive bags contain 5 mirrored green bags, 3 drab aqua bags, 2 dull beige bags, 3 dim indigo bags.
dark brown bags contain 1 muted tan bag, 5 wavy lime bags.
mirrored red bags contain 5 vibrant aqua bags, 3 bright gray bags, 4 striped gray bags, 2 light lavender bags.
mirrored olive bags contain 5 dark chartreuse bags, 1 dim lavender bag, 5 faded plum bags.
dim black bags contain 1 shiny yellow bag, 4 plaid indigo bags.
shiny maroon bags contain 1 mirrored orange bag, 1 muted chartreuse bag, 5 wavy bronze bags, 1 dull lavender bag.
pale brown bags contain 4 light teal bags, 1 light plum bag, 3 bright aqua bags.
striped chartreuse bags contain 2 shiny maroon bags.
mirrored chartreuse bags contain 4 dotted tan bags, 4 bright bronze bags.
plaid magenta bags contain 1 vibrant yellow bag, 5 wavy aqua bags, 5 dull turquoise bags, 2 wavy fuchsia bags.
light salmon bags contain 3 muted tan bags, 3 faded turquoise bags, 4 drab bronze bags, 3 bright bronze bags.
faded blue bags contain 1 wavy red bag, 5 wavy fuchsia bags, 4 bright bronze bags, 3 faded turquoise bags.
plaid aqua bags contain 1 pale tan bag, 3 light white bags.
mirrored turquoise bags contain 5 drab beige bags, 1 faded plum bag, 4 mirrored orange bags.
shiny teal bags contain 2 pale black bags, 4 dull indigo bags, 4 vibrant teal bags.
vibrant coral bags contain 3 drab turquoise bags, 4 striped beige bags, 1 wavy turquoise bag, 4 light crimson bags.
faded gray bags contain 3 striped purple bags."""


# inp = test_inp


def parsereq(req):
    i = req.find(" ")
    return int(req[0:i]), req[i + 1:]


def parseline(line):
    m = re.match("^(.+) bags contain (.+)\\.$", line)
    who = m.group(1)
    if m.group(2) == "no other bags":
        return who, []
    else:
        v = m.group(2)
        v = v.replace(" bags", "")
        v = v.replace(" bag", "")
        return who, list(map(parsereq, re.split(", ", v)))


rules = list(map(parseline, inp.split("\n")))


def search_outer_bags(start, v):
    v.add(start)
    for rule in rules:
        for req in rule[1]:
            if req[1] == start:
                search_outer_bags(rule[0], v)
    return v


def count_inner_bags(start):
    s = 1
    for rule in rules:
        if rule[0] == start:
            for req in rule[1]:
                s += req[0] * count_inner_bags(req[1])
    return s


print("Part One", len(search_outer_bags("shiny gold", set())) - 1)
print("Part Two", count_inner_bags("shiny gold") - 1)
