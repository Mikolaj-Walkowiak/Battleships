﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameLogicController : ControllerBase
    {
        private readonly IGameLogicService _GameLogicService;
        public GameLogicController(IGameLogicService gameLogicService)
        {
            _GameLogicService = gameLogicService;
        }
        [HttpGet]
        public IEnumerable<GameLogicModel> Get()
        {
            yield return _GameLogicService.Move();
        }
    }
}
