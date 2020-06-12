using Microsoft.AspNetCore.Http;
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
        private readonly Blob.IStore _blobStore;
        private readonly IMapper _mapper;
        private readonly ILogger<Faces> _logger;

        public Faces(Data.IStore dataStore, Blob.IStore blobStore, IMapper mapper, ILogger<Faces> logger)
        {
            _dataStore = dataStore;
            _blobStore = blobStore;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetAll([FromQuery] Options options)
        {
            var faces = await _dataStore.GetFacesAsync(options.PageNumber, options.ItemsPerPage);

            return Ok(faces);
        }

        [HttpGet("{id}", Name = nameof(GetFace))]
        [ProducesResponseType(200, Type = typeof(Data.Face))]
        public async Task<IActionResult> GetFace([FromRoute] Guid id)
        {
            var faces = await _dataStore.GetFaceAsync(id);

            if (faces.Any())
            {
                return Ok(faces.Select(face => _mapper.Map(face, Request)).First());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("unrecognised", Name = nameof(GetUnrecognised))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetUnrecognised([FromQuery] Options options)
        {
            var faces = await _dataStore.GetUnrecognisedAsync(options.PageNumber, options.ItemsPerPage);

            return Ok(faces.Select(face => _mapper.Map(face, Request)));
        }

        [HttpGet("unrecognised/at/{confidence}", Name = nameof(GetUnrecognisedAtConfidence))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetUnrecognisedAtConfidence([FromRoute] float confidence, [FromQuery] Options options)
        {
            var faces = await _dataStore.GetUnrecognisedAsync(confidence, options.PageNumber, options.ItemsPerPage);

            return Ok(faces.Select(face => _mapper.Map(face, Request)));
        }

        [HttpGet("unrecognised/by/{recogniser}", Name = nameof(GetUnrecognisedBy))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Data.Face>))]
        public async Task<IActionResult> GetUnrecognisedBy([FromRoute] Guid recogniser, [FromQuery] Options options)
        {
            var faces = await _dataStore.GetUnrecognisedByAsync(recogniser, options.PageNumber, options.ItemsPerPage);

            return Ok(faces.Select(face => _mapper.Map(face, Request)));
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
        [ProducesResponseType(201, Type = typeof(Data.Face))]
        public async Task<IActionResult> Add(IFormFile file, string location)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var uri = await _blobStore.SaveImage(stream);

                    var face = new Data.Face { Id = Guid.NewGuid(), Uri = uri.ToString(), Location = location, Created = DateTime.UtcNow };

                    var id = await _dataStore.AddAsync(face);

                    return CreatedAtRoute(nameof(GetFace), new { id }, face);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");

                return StatusCode(500);
            }
        }
    }
}