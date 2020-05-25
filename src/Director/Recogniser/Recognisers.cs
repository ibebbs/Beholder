using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Director.Recogniser
{
    [Route("api/recognisers")]
    [ApiController]
    public class Recognisers : ControllerBase
    {
        private readonly Data.IStore _dataStore;
        private readonly ILogger<Recognisers> _logger;

        public Recognisers(Data.IStore dataStore, ILogger<Recognisers> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Recogniser>))]
        public async Task<IActionResult> GetAll()
        {
            var recognisers = await _dataStore.GetRecognisersAsync();

            return Ok(recognisers);
        }
    }
}
