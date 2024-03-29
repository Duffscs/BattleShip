﻿using BattleShip.API.Exception;
using BattleShip.Models;
using OneOf;

namespace BattleShip.API.Domain;

[GenerateOneOf]
public partial class GameOrNotFound : OneOfBase<Game, NotFound> {}

[GenerateOneOf]
public partial class GameStateOrGameAlreadyFinished: OneOfBase<GameState, GameAlreadyFinished> { }