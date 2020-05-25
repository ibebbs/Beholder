using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Director.Label
{
    [Route("api/labels")]
    [ApiController]
    public class Labels : ControllerBase
    {
        private readonly Data.IStore _dataStore;
        private readonly ILogger<Labels> _logger;

        public Labels(Data.IStore dataStore, ILogger<Labels> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        public async Task<IActionResult> GetAll()
        {
            var faces = await _dataStore.GetLabelsAsync();

            return Ok(faces);
        }
    }
}
