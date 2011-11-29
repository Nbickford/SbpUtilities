jim'slide v. 0.81
copyright Jim Leonard (vleonard@infinet.com) 1998 - 1999


Here is my attempt at a slide puzzle solver.  My object in designing it was to create something that was rather fast and have reasonable memory consumption.  The output is not pretty but is functional.

I thank Bob Henderson for his help with providing me information, ideas and feedback that greatly helped make this solver what it is.


Machine requirements:
must be running win95, win98, or winNT.  16mb ram should be adequate to work on many puzzles, and the more ram the better but you must have plenty of disk space to hold the puzzle move tree.  jimslide creates temp files in the current directory (js_tmp00.tmp, js_tmp01.tmp, and js_tmp02.tmp... defaults to 6 temp files) to store the move tree (which allows up to a 2 gigabyte limit per temp file for storing the move tree assuming you have the disk space available).  These may be deleted after running the program.  

If you have any questions or comments, please feel free to email me at
	vleonard@infinet.com
I really would like feedback, if nothing else just to say you downloaded it.  I will respond to messages as time permits (I'll try to respond to everything eventually but I don't want to make any promises).

To run it:
first create a config file to define the puzzle (more on that later and with this should be a few examples).
For this example let's call the config file puz.txt (which is the default config file name if none is supplied).

enter at a win32 console (dos box) prompt:
jimslide puz.txt

or to save the solution to a file (say puz.sol) enter:
jimslide puz.txt > puz.sol


The config file:
first off any line that doesn't start with a valid keyword is considered a comment.  I recommend starting the line with a '!' to denote a comment because future versions might actually have error checking in the config file parser.

There are two data types used - integers and paired integers.  Paired integers are expressed (without the quotes) as "(x, y)" ie (5, 6).

To help explain the config file we are going to use the following common puzzle (L'Ane Rouge?) as an example (where the object is to move the large square to the bottom center where the four small squares are) and then give a complete list and description of the command set.

+--+-----+--+
|  |     |  |
|  |     |  |
|  |     |  |
+--+-----+--+
|  |     |  |
|  +--+--+  |
|  |  |  |  |
+--+--+--+--+
   |  |  |
   +--+--+

first we need to specify the dimensions of the board
set horizontal size of puzzle to 4 unit squares
	xsize: 4
set vertical size of puzzle to 5 unit squares
	ysize: 5
since the puzzle is rectangular there's no need to define any internal walls.

Then we need to define the piece set:
	piece: 1 (0,0) (1,0) (0,1) (1,1) ! (2x2 square)
ie define piece type '1' to be made of of four blocks at relative positions (0,0), (1,0), (0,1) and (1,1).
	piece: 2 (0,0) (1,0)             ! (horizontal rectangle)
	piece: 3 (0,0) (0,1)             ! (vertical rectangle)
	piece: 4 (0,0)                   ! (1 block square)
All coordinates are 0 based.  Each piece must start with position (0,0) and that position must be the right most square of the uppermost row of squares.

We then place one or more of each type of piece on the board using the 'put:' command.
	put: 3 (0,0)
ie put a piece of type '3' at position (0,0).
	put: 1 (1,0)
	put: 3 (3,0)
	put: 3 (0,2)
	put: 2 (1,2)
	put: 3 (3,2)
	put: 4 (1,3)
	put: 4 (2,3)
	put: 4 (1,4)
	put: 4 (2,4)

Now we define the winning condition(s)
	win: 1 (1,3)
ie to win there must be a piece of type '1' based at position (1,3).  All win conditions must be met for the puzzle to be considered solved.

All that's left to define the puzzle is to specify the memory constraints that the solver will work in.  A decent set of values to work with is one half system ram for bigmem: and one eigth for smallmem:.  So for my system with 64 meg of ram I often use:
	bigmem: 32000000
	smallmem: 8000000
The program uses these values to create three pools of memory, one of size bigmem: and two of size smallmem:

save the config file and run the program.





Full Command Set Reference

A list of the keywords
  xsize: n
  ysize: n
  wall: (x,y)
  hwall: y (x1, x2)
  vwall: x (y1, y2)
  piece: n (x1, y1) [(x2, y2)...(x~, y~)]
  lockx: n
  locky: n
  xstep: n1 n2
  ystep: n1 n2
  put: n (x, y) ['c']
  win: n (x, y)
  nograph:
  useletters:
  bigmem: n
  smallmem: n
  numworkfiles: n
  noconcurrent:
  winonly:
  cutoff: n
  directed: n
  checkfulltree:


Each command is defined in the following format:
syntax
	definition
example

and the command set is broken up into three sections:
	defining puzzle dimensions
	defining puzzle pieces
	and solver flags and parameters

Defining Puzzle Dimensions
xsize: n
	set horizontal size of puzzle to n units
xsize: 4

ysize: n
	set vertical size of puzzle to n units
ysize: 5

wall: (x,y)
	adds a 1 unit square wall inside the puzzle boundaries
wall: (3,4)


hwall: y (x1, x2)
	adds a horizontal wall on row y from column x1 to x2

vwall: x (y1, y2)
	adds a vertical wall on column x from row y1 to y2


Defining Puzzle Pieces

piece: n (x1, y1) [(x2, y2)...(x24, y24)]
	define piece type n to be comprised of unit squares offset at
	(x1,y1), (x2,y2) ... there is a maximum of 24 squares per piece
	type.  The piece labels can be any integer, but don't use 0 as
	it is reserved for empty spaces.
piece: 1 (0,0) (1,0) (0,1) (1,1)

lockx: n
locky: n
	constrain a piece (n) horizontally (lockx:) or vertically (locky:)
	This is useful for things like Nob's "Rush Hour" (or also when a
	piece just isn't able to move in a direction this will save some
	processing time)

xstep: n1 n2
ystep: n1 n2
	If set for a piece of type n1, a position is only stored if the 
	net movement of that piece in the x (or y for ystep:) direction 
	of that piece is a multiple of n2 spaces.


put: n (x, y)
 - or -
put: n (x, y) 'c'
	place a piece of type n at location x,y.  Multiple instances of
	the same type of piece may be placed making them interchangable
	for win conditions.  You may optionally put a single character
	in single quotes so that when that piece is displayed it is
	displayed as 'c' instead of an arbitrary character.
put: 3 (3,0) 'T'

win: n (x, y)
	to win, a piece of type n must be based off of (x,y) or if
	n = 0 (x,y) must be an open space.  If multiple win conditions are
	specified they must all be satisfied.  There currently are
	no provisions for 'or'ing conditions.
win: 1 (1,4)

nograph:
	when the winning move sequence is displayed, don't print the
	intermediate board positions, just the text description of
	what happened.

useletters:
	use letters instead of numbers for the default piece labels.

Solver Flags and Parameters

bigmem: n
smallmem: n
	I'm still experimenting with appropriate values.  Both values
	are necessary but if they aren't specified it will default to
	what I'm finding are somewhat crummy values.  A good start
	for these values would be half of available ram for bigmem:
	and an eighth of available ram for smallmem:, ie for 64 meg
	of ram I use:
bigmem: 32000000
smallmem: 8000000
	Total memory used by the program will be bigmem + 2 * smallmem
	plus some additional overhead (should be well under a meg).
	This leaves some memory free for disk caching (very important)
	and windows system routines.

numworkfiles: n
	number of workfiles to use.  The default is 6.  If winonly is
	this should be a multiple of 3.  If winonly is set and the
	puzzle is being solved concurrently, this should be a multiple
	of 6.

noconcurrent:
	this will keep the program from attempting to solve the puzzle
	from both ends despite what it thinks it can do.  There are
	times when this is faster.

winonly:
	this will cause the program to only search for the solution
	and not save the entire move tree.  Useful to find the complete
	solution so that you can run a concurrent search later.  Used
	when you don't have the disk space.

cutoff: n
	abort the solve after n generations (moves).

directed: n
	performs a partial width search using only the best n
	positions in a generation as defined by a fitness function.
	The current fitness function is the sum of the size of each
	piece multiplied by the square of the horizontal and vertical 
	distances from it's win condition.  This function is rather
	arbitrary and could probably be improved upon.  If you have
	a C compiler this can be modified in the functions Distance()
	and CDistance().

checkfulltree:
	perform duplicate checks on the entire move tree instead of
	just the current and the previous two generations.  There is
	no reason to use this except while performing a directed
	search.


As almost an afterthought, I added one more small feature.
you can pause (but currently not save) the solve process.  To pause
the solve process create a file 'pause.txt' in the same directory
that you're running jimslide and within a few minutes it will stop
processing until you either rename, move or delete the file pause.txt.
It's kind of hokey but it works.


example puzzles:
************************
! The above example, (L'Ane Rouge)
xsize: 4
ysize: 5
piece: 1 (0,0) (1,0) (0,1) (1,1)
piece: 2 (0,0) (1,0)
piece: 3 (0,0) (0,1)
piece: 4 (0,0)
put: 3 (0,0)
put: 1 (1,0)
put: 3 (3,0)
put: 3 (0,2)
put: 2 (1,2)
put: 3 (3,2)
put: 4 (1,3)
put: 4 (2,3)
put: 4 (1,4)
put: 4 (2,4)
win: 1 (1,3)
bigmem: 32000000
smallmem: 8000000
************************
! rush hour puzzle #1
! up thru the put's same for all rh's
xsize: 6
ysize: 6
piece: 1 (0,0) (1,0) ! the red car
piece: 2 (0,0) (1,0) ! horizontal cars
piece: 3 (0,0) (0,1) ! vertical cars
piece: 4 (0,0) (1,0) (2,0) ! horizontal trucks
piece: 5 (0,0) (0,1) (0,2) ! vertical trucks
locky: 1
locky: 2
lockx: 3
locky: 4
lockx: 5
win: 1 (4,2)
! now we're at the puzzle specific code
put: 1 (1,2)
put: 2 (0,0)
put: 2 (4,4)
put: 3 (0,4)
put: 4 (2,5)
put: 5 (5,0)
put: 5 (0,1)
put: 5 (3,1)
bigmem: 32000000
smallmem: 8000000
****************************
! Nob's Tzer:
xsize: 6
ysize: 5
piece: 1 (0,0) (-1, 1) (0, 1) (1, 1);
piece: 2 (0,0) (-1, 1) (0, 1) (1, 1);
piece: 3 (0,0) (1, 0);
piece: 4 (0,0);
put: 1 (1,0)
put: 2 (4,0)
put: 3 (0,3)
put: 3 (2,3)
put: 3 (4,3)
put: 3 (2,4)
put: 4 (0,4)
put: 4 (1,4)
put: 4 (4,4)
put: 4 (5,4)
wall: (0,0)
wall: (2,0)
wall: (3,0)
wall: (5,0)
win: 1 (4,0)
win: 2 (1,0)
win: 3 (0,3)
win: 3 (2,3)
win: 3 (4,3)
win: 3 (2,4)
win: 4 (0,4)
win: 4 (1,4)
win: 4 (4,4)
win: 4 (5,4)
bigmem: 32000000
smallmem: 8000000
**************************
! Nob's Ultimate Tzer
xsize: 6
ysize: 5
piece: 1 (0,0) (-1, 1) (0, 1) (1, 1);
piece: 2 (0,0) (-1, 1) (0, 1) (1, 1);
piece: 3 (0,0) (1, 0);
piece: 4 (0,0);
put: 1 (1,0)
put: 2 (4,0)
put: 3 (2,2)
put: 3 (0,3)
put: 3 (2,3)
put: 3 (4,3)
put: 4 (2,4)
put: 4 (3,4)
put: 4 (0,4)
put: 4 (1,4)
put: 4 (4,4)
put: 4 (5,4)
wall: (0,0)
wall: (2,0)
wall: (3,0)
wall: (5,0)
win: 1 (4,0)
win: 2 (1,0)
win: 3 (2,2)
win: 3 (0,3)
win: 3 (2,3)
win: 3 (4,3)
win: 4 (2,4)
win: 4 (3,4)
win: 4 (0,4)
win: 4 (1,4)
win: 4 (4,4)
win: 4 (5,4)
bigmem: 32000000
smallmem: 8000000
**************************
! Nob's Pink & Blue
! While the eye piece is larger in the real puzzle
! it does follow the same constraints using the
! locky: parameter, and causes the same constraints
! because of the larger middle slide area on the real
! puzzle.
!
xsize: 4
ysize: 8
wall: (0,2)
wall: (2,2)
wall: (3,2)
wall: (0,5)
wall: (1,5)
wall: (3,5)
piece: 1 (0,0) (0,1)
piece: 2 (0,0) (0,1)
piece: 3 (0,0) (0,1)
piece: 4 (0,0) (0,1)
piece: 5 (0,0) (0,1)
piece: 6 (0,0) (0,1)
piece: 7 (0,0) (0,1)
piece: 8 (0,0) (0,1)
piece: 9 (0,0) (0,1)
locky: 9
put: 1 (0,0)
put: 2 (1,0)
put: 3 (2,0)
put: 4 (3,0)
put: 5 (0,6)
put: 6 (1,6)
put: 7 (2,6)
put: 8 (3,6)
put: 9 (3,3)
win: 1 (0,6)
win: 2 (1,6)
win: 3 (2,6)
win: 4 (3,6)
win: 5 (0,0)
win: 6 (1,0)
win: 7 (2,0)
win: 8 (3,0)
win: 9 (3,3)
bigmem: 32000000
smallmem: 8000000
**************************
! Dirty Dozen #12
xsize: 6
ysize: 5
piece: 1 (0,0) (1,0) (0,1) (1,1)
piece: 2 (0,0) (1,0) (0,1)
piece: 3 (0,0) (-1,1) (0,1)
piece: 4 (0,0) (1,0)
piece: 5 (0,0) (0,1)
piece: 6 (0,0)
put: 1 (2,2)
put: 2 (1,0)
put: 3 (3,0)
put: 2 (4,0)
put: 5 (5,1)
put: 6 (0,2)
put: 6 (1,2)
put: 5 (4,2)
put: 6 (0,3)
put: 6 (1,3)
put: 3 (5,3)
put: 6 (0,4)
put: 6 (1,4)
put: 4 (2,4)
win: 1 (4,3)
bigmem: 32000000
smallmem: 8000000
**************************
! Kuroko and Dairu
bigmem: 32000000
smallmem: 8000000
!
xsize: 13
ysize: 3
wall: (2,1)
wall: (3,1)
piece: 1 (0,0) (1,0)
piece: 2 (0,0) (1,0) (2,0)
piece: 3 (0,0) (1,0) (2,0)
piece: 4 (0,0) (1,0) (2,0)
piece: 5 (0,0) (1,0)
piece: 6 (0,0) (1,0) (2,0)
piece: 7 (0,0) (1,0)
piece: 8 (0,0) (1,0) (2,0) (3,0)
piece: 9 (0,0) (0,1) (1,0) (1,1)
piece: 10 (0,0) (1,0) (-2,1) (-1,1) (0,1) (1,1)
lockx: 9
put: 1 (0,0)
put: 2 (2,0)
put: 3 (5,0)
put: 4 (8,0)
put: 5 (2,2)
put: 6 (4,2)
put: 7 (7,2)
put: 8 (9,2)
put: 9 (0,1)
put: 10 (11,0)
win: 1 (2,2)
win: 2 (4,2)
win: 3 (7,2)
win: 4 (10,2)
win: 5 (0,0)
win: 6 (2,0)
win: 7 (5,0)
win: 8 (7,0)
win: 9 (0,1)
win: 10 (11,0)
***************************
!Mosaic C (pentomino puzzle)
!by Michael McKee
xsize: 9
ysize: 9
wall: (0,0)
wall: (8,8)
piece: 1 (0,0) (-1,1) (1,1) (0,2) (0,1) ! X
piece: 2 (0,0) (4,0) (1,0) (2,0) (3,0)  ! I
piece: 3 (0,0) (1,0) (-1,1) (0,1) (-1,2)! W
piece: 4 (0,0) (2,0) (-1,1) (0,1) (1,0) ! N
piece: 5 (0,0) (0,3) (1,1) (0,1) (0,2)  ! Y
piece: 6 (0,0) (1,0) (-1,2) (0,2) (0,1) ! Z
piece: 7 (0,0) (2,0) (1,2) (1,0) (1,1)  ! T
piece: 8 (0,0) (-2,2) (0,2) (0,1) (-1,2)! V
piece: 9 (0,0) (-1,2) (1,1) (0,2) (0,1) ! F
piece: 10 (0,0) (1,2) (1,0) (0,2) (1,1) ! U
piece: 11 (0,0) (-3,1) (0,1) (-2,1) (-1,1)! L
piece: 12 (0,0) (2,0) (0,1) (1,1) (1,0) ! P
put: 1 (1,0) 'X'
put: 2 (2,0) 'I'
put: 3 (3,1) 'W'
put: 4 (5,1) 'N'
put: 5 (0,2) 'Y'
put: 6 (6,2) 'Z'
put: 7 (3,3) 'T'
put: 8 (7,3) 'V'
put: 9 (1,4) 'F'
put: 10 (2,4) 'U'
put: 11 (4,6) 'L'
put: 12 (5,6) 'P'
win: 1 (7,6)
bigmem: 8000000
smallmem: 2000000
numworkfiles: 1
***************************
! Jerry Slocum puzzle (the really slow way)
! by Minoru Abe
!
xsize: 12
ysize: 3
piece: 1 (0, 0)(1, 0)
piece: 2 (0, 0)(1, 0)
piece: 3 (0, 0)(1, 0)
piece: 4 (0, 0)(1, 0)
piece: 5 (0, 0)(1, 0)
piece: 6 (0, 0)(1, 0)
piece: 7 (0, 0)(1, 0)(0, 1)(1, 1)
piece: 8 (0, 0)(1, 0)
piece: 9 (0, 0)(1, 0)
piece: 10 (0, 0)(1, 0)
piece: 11 (0, 0)(1, 0)
piece: 12 (0, 0)(1, 0)
put: 1 (0, 0) 'S'
put: 2 (2, 0) 'L'
put: 3 (4, 0) 'O'
put: 4 (6, 0) 'C'
put: 5 (8, 0) 'U'
put: 6 (10, 0) 'M'
put: 7 (10, 1) 'X'
put: 8 (0, 2) 'J'
put: 9 (2, 2) 'E'
put: 10 (4, 2) 'R'
put: 11 (6, 2) 'r'
put: 12 (8, 2) 'Y'
wall: (0, 1)
hwall: 1 (3, 5)
hwall: 1 (8, 9)
win: 1 (0, 2)
win: 2 (2, 2)
win: 3 (4, 2)
win: 4 (6, 2)
win: 5 (8, 2)
win: 6 (10, 2)
win: 7 (10, 0)
win: 8 (0, 0)
win: 9 (2, 0)
win: 10 (4, 0)
win: 11 (6, 0)
win: 12 (8, 0)
bigmem: 44000000
smallmem: 4000000
lockx: 7
***************************
! Jerry Slocum puzzle (faster)
! by Minoru Abe
!
! Warning, This puzzle can take several hours (or days) to
! solve.  It is included here for it's use of xstep and ystep.
! 
! The Jerry Slocum puzzle above gets tied down very quickly saving
! positions for half steps that you would never use.  Because there
! is a valid place to stop a piece on the half step, we cheat and move
! that space over and use xstep and ystep to constrain the what the
! solver considers a valid position.
!
xsize: 12
ysize: 5
piece: 1 (0, 0)(1, 0)
piece: 2 (0, 0)(1, 0)
piece: 3 (0, 0)(1, 0)
piece: 4 (0, 0)(1, 0)
piece: 5 (0, 0)(1, 0)
piece: 6 (0, 0)(1, 0)
piece: 7 (0, 0)(1, 0)(0, 1)(1, 1)(0, 2)(1, 2)(0, 3)(1, 3)
piece: 8 (0, 0)(1, 0)
piece: 9 (0, 0)(1, 0)
piece: 10 (0, 0)(1, 0)
piece: 11 (0, 0)(1, 0)
piece: 12 (0, 0)(1, 0)
put: 1 (0, 0) 'S'
put: 2 (2, 0) 'L'
put: 3 (4, 0) 'O'
put: 4 (6, 0) 'C'
put: 5 (8, 0) 'U'
put: 6 (10, 0) 'M'
put: 7 (10, 1) 'X'
put: 8 (0, 4) 'J'
put: 9 (2, 4) 'E'
put: 10 (4, 4) 'R'
put: 11 (6, 4) 'r'
put: 12 (8, 4) 'Y'
wall: (0, 1)
hwall: 1 (3, 5)
hwall: 2 (3, 5)
hwall: 3 (3, 5)
vwall: 8 (1, 3)
vwall: 9 (1, 3)
wall: (0, 3)
win: 1 (0, 4)
win: 2 (2, 4)
win: 3 (4, 4)
win: 4 (6, 4)
win: 5 (8, 4)
win: 6 (10, 4)
win: 7 (10, 0)
win: 8 (0, 0)
win: 9 (2, 0)
win: 10 (4, 0)
win: 11 (6, 0)
win: 12 (8, 0)
xstep: 1 2
xstep: 2 2
xstep: 3 2
xstep: 4 2
xstep: 5 2
xstep: 6 2
xstep: 8 2
xstep: 9 2
xstep: 10 2
xstep: 11 2
xstep: 12 2
ystep: 1 2
ystep: 2 2
ystep: 3 2
ystep: 4 2
ystep: 5 2
ystep: 6 2
ystep: 8 2
ystep: 9 2
ystep: 10 2
ystep: 11 2
ystep: 12 2
lockx: 7
bigmem: 40000000
smallmem: 8000000
