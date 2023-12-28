# Checkers
Over the summer break, I undertook this project as part of my preparation for an [assignment](https://github.com/GitHubByJelle/Cannon) in one of the courses within my Master's program in Artificial Intelligence. This project contains an implementation of [Checkers](https://www.chessprogramming.org/Checkers) as a console application, as well as implementation of search algorithms trained with self-play.

<p align="center" width="100%">
    <img src="images/checkers.gif" alt="Visualisation of RandomBot playing a game of Checkers against AlphaBetaBot" width="70%">
</p>

## Implementation Details
The code is written in C#. No packages have been used for the implementations of the search algorithms. For AI functionality, the code leverages the following techniques/algorithms:
* One-ply search
* Alpha-Beta Search
* Neural Networks
* Genetic Algorithm

Based on these algorithms, combined with different feature evaluation functions, multiple bots have been implemented:
* Random bot (Makes random move, but captures when possible)
* NNbot (One-ply search)
* AlphaBetaBot (Alpha-Beta search)

Both NNBot as AlphaBetaBot utilize a Neural Network (NN) as evaluation function. The NN is trained by using an Genetic Algorithm and self-play.

As the assignment for which this project was undertaken does not necessitate visualizations, no visual representations have been incorporated, except for those displayed in the terminal.

## How to use
To run the game, open Visual Studio (2019), open the folder and run `Programm.cs`. Please note that the base directory to the `Text` folder needs to be updated in `Game/GameLoop.cs`.

## Known improvements
The code has been implemented during my summer break with a limited amount of time available. Because of that, there is enough room for improvement:
* Use inheritance for players
* Add comments / documentation
* Use simple evalution function
* Implement other self-learning algorithms
* Improve Alpha-Beta Search (e.g. Transposition Tables, Iterative Deepening, etc.)
