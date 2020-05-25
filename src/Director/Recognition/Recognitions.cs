using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Director.Recognition
{
    [Route("api/recognitions")]
    [ApiController]
    public class Recognitions : ControllerBase
    {
        private readonly Data.IStore _dataStore;
        private readonly ILogger<Recognitions> _logger;

        public Recognitions(Data.IStore dataStore, ILogger<Recognitions> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Recognition>))]
        public async Task<IActionResult> GetAll()
        {
            var recognitions = await _dataStore.GetRecognitionsAsync();

            return Ok(recognitions);
        }

        [HttpGet("{label}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Recognition>))]
        public async Task<IActionResult> GetAll([FromRoute] string label)
        {
            var recognitions = await _dataStore.GetRecognitionsAsync(label);

            return Ok(recognitions);
        }
    }
}
