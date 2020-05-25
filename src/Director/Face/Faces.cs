using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Director.Face
{
    [Route("api/faces")]
    [ApiController]
    public class Faces : ControllerBase
    {
        private readonly Data.IStore _dataStore;
        private readonly ILogger<Faces> _logger;

        public Faces(Data.IStore dataStore, ILogger<Faces> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetAll()
        {
            var faces = await _dataStore.GetFacesAsync();

            return Ok(faces);
        }

        [HttpGet("{id}", Name = nameof(GetFace))]
        [ProducesResponseType(200, Type = typeof(Data.Face))]
        public async Task<IActionResult> GetFace([FromRoute] Guid id)
        {
            var faces = await _dataStore.GetFaceAsync(id);

            if (faces.Any())
            {
                return Ok(faces.First());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("unrecognised", Name = nameof(GetUnrecognised))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetUnrecognised()
        {
            var faces = await _dataStore.GetUnrecognisedAsync();

            return Ok(faces);
        }

        [HttpGet("unrecognised/{confidence}", Name = nameof(GetUnrecognisedAtConfidence))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetUnrecognisedAtConfidence(float confidence)
        {
            var faces = await _dataStore.GetUnrecognisedAsync(confidence);

            return Ok(faces);
        }

        [HttpGet("{id}/recognitions", Name = nameof(GetRecognitions))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Recognition>))]
        public async Task<IActionResult> GetRecognitions([FromRoute] Guid id)
        {
            var recognitions = await _dataStore.GetRecognitionsAsync(id);

            return Ok(recognitions);
        }

        [HttpGet("{faceId}/recognitions/{recognitionId}", Name = nameof(GetRecognition))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Recognition>))]
        public async Task<IActionResult> GetRecognition([FromRoute] Guid faceId, [FromRoute] Guid recognitionId)
        {
            var recognitions = await _dataStore.GetRecognitionAsync(faceId, recognitionId);

            if (recognitions.Any())
            {
                return Ok(recognitions.First());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("{faceId}/recognitions", Name = nameof(AddRecognition))]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(201, Type = typeof(Data.Recognition))]
        public async Task<IActionResult> AddRecognition([FromRoute] Guid faceId, [FromForm] Guid recogniserId, [FromForm] string label, [FromForm] float confidence)
        {
            var recognition = new Data.Recognition
            {
                Id = Guid.NewGuid(),
                FaceId = faceId,
                RecogniserId = recogniserId,
                Label = label,
                Confidence = confidence,
                Created = DateTime.UtcNow
            };

            var recognitionId = await _dataStore.AddAsync(recognition);

            return CreatedAtRoute(nameof(GetRecognition), new { faceId, recognitionId }, recognition);
        }

        [HttpPost()]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(201, Type = typeof(Data.Face))]
        public async Task<IActionResult> Add([FromForm] Uri uri, [FromForm] string location)
        {
            var face = new Data.Face { Id = Guid.NewGuid(), Uri = uri.ToString(), Location = location, Created = DateTime.UtcNow };

            try
            {
                var id = await _dataStore.AddAsync(face);

                return CreatedAtRoute(nameof(GetFace), new { id }, face);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");

                return StatusCode(500);
            }
        }
    }
}