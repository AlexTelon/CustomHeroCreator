# CustomHeroCreator

Custom Hero Creator is an tool that uses AI to understand, analyze and generate skill-trees in games.

# The goal
The goal is that this is what a chess-engine is to chess. The goal is to help game-makers to make fun, challenging and balanced skill trees. Either static traditional skill-trees or much more dynamic ones. With the help of this tool a game should be able to generate new skill-trees for every individual character, both players and NPCs alike. This will open up for new types of gameplay where there is no answers to what the best builds are online.

So while this tool can be used to evaluate skill-trees and help you to find the best builds in skill-trees its ultimate goal is to create more challenging and dynamic experiences in games. Not to create an even more stale environment where this tool is used together with expert oppinion to gauge what the best builds in existing games are.


# How to build
Here is an explanation of how to build the latest code

## docker
Download the source code and in the repo run:

`docker build . -t customhero`

once that has run (takes some time the first time) run:

`docker run customhero`


## Build with dotnet build tools
Download the source code and in the repo run:

`dotnet publish -c Release -r win10-x64 -o out`

and you will find a executable in the out folder!
