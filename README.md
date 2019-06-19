# MathVisualization
Visualizing math concept with Unity

# Url parameters

## Usage:
https://uberyoji.github.io/MathVisualization/

| Param         | Type    | Purpose  |
|:--------------|:-------:|:-----|
| scene    | string | Loads the specified scene.            |

## Possible values for scenes
| Value              | Purpose  |
|:-------------------|:-------  |
| CellularAutomata   |  1d cellular automata |
| GameOfLife    	 |  2d cellular automata |
| GameOfLifeCube     |  3d cellular automata |
| Mandalas    		 |  Polygonal recursion fractal |
| Tree    			 | L-system fractal |
| GoldenRatio		 | Ratio visualizer (broken for now as it only shows the golden ratio) |
| Fourier 			 | Shows fourier transform (camera is a bit off in webgl. to fix.)

### CellularAutomata
Param that can be passed to the scene. Active cell at start is always the center one for now.

| Param         | Type    | Purpose  |
|:--------------|:-------:|:-----|
| size    | int | Specifies the number of cells in 1d |
| rule    | int | Specifies the rule to apply. See Wolfram (http://mathworld.wolfram.com/ElementaryCellularAutomaton.html) |
| freq    | float | Specifies the refresh rate of generation. |

### GameOfLife
Param that can be passed to the scene. Active cells at start are determined by seed value.

| Param         | Type    | Purpose  |
|:--------------|:-------:|:-----|
| size    | int | Specifies the number of cells in 2d (always square for now) |
| A    | int | Specifies the rule to apply. Birth A <= # <=B. |
| B    | int | Specifies the rule to apply. Birth A <= # <=B. |
| C    | int | Specifies the rule to apply. Survival C <= # <=D. |
| D    | int | Specifies the rule to apply. Survival C <= # <=D. |
| freq    | float | Specifies the refresh rate of generation. |
| seed    | float | Probability of cell begin active at start of simulation. |


### GameOfLifeCube
Param that can be passed to the scene. Active cells is always the center one for now.

| Param         | Type    | Purpose  |
|:--------------|:-------:|:-----|
| size    | int | Specifies the number of cells in 3d (always cube for now) |
| A    | int | Specifies the rule to apply. Birth A <= # <=B. |
| B    | int | Specifies the rule to apply. Birth A <= # <=B. |
| C    | int | Specifies the rule to apply. Survival C <= # <=D. |
| D    | int | Specifies the rule to apply. Survival C <= # <=D. |
| freq    | float | Specifies the refresh rate of generation. |
| seed    | float | Probability of cell begin active at start of simulation. |
| randomize | int | If set to 1, cells are actived according to seed on start. |

### Mandalas
No params for now.

### Tree
| Param         | Type    | Purpose  |
|:--------------|:-------:|:-----|
| variant    | int | Branching variant [0,3] |
| maxiteration    | int | Specifies the number of generation to spawn. |
| growthdelay    | float | Time for branch to grow in secs.  |

### GoldenRatio
No params for now.

### Fourier
No params for now.







