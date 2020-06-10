using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Director.Face
{
    public interface IMapper
    {
        Data.Face Map(Data.Face face, HttpRequest request);
    }

    public class Mapper : IMapper
    {
        private static Data.Face MapToRequest(Data.Face face, HttpRequest request)
        {
            var builder = new UriBuilder(face.Uri);
            builder.Host = request.Host.Host;
            builder.Port = 10000;

            return new Data.Face
            {
                Id = face.Id,
                Location = face.Location,
                Uri = builder.Uri.ToString(),
                Created = face.Created
            };
        }

        private static Data.Face MapToHost(Data.Face face, string host)
        {
            var builder = new UriBuilder(face.Uri);
            builder.Host = host;
            builder.Port = 10000;

            return new Data.Face
            {
                Id = face.Id,
                Location = face.Location,
                Uri = builder.Uri.ToString(),
                Created = face.Created
            };
        }

        private static Data.Face NoMapping(Data.Face face, HttpRequest request)
        {
            return face;
        }

        private static Func<Data.Face, HttpRequest, Data.Face> MapFunc(Configuration config)
        {
            if (config.MapBlobsToRequestHost)
            {
                return MapToRequest;
            }
            else if (!string.IsNullOrWhiteSpace(config.MapBlobsToSpecificHost))
            {
                return (face, request) => MapToHost(face, config.MapBlobsToSpecificHost);
            }
            else
            {
                return NoMapping;
            }
        }

        private readonly IOptions<Configuration> _options;
        private readonly Func<Data.Face, HttpRequest, Data.Face> _func;

        public Mapper(IOptions<Configuration> options)
        {
            _options = options;

            _func = MapFunc(_options.Value);
        }

        public Data.Face Map(Data.Face face, HttpRequest request)
        {
            return _func(face, request);
        }
    }
}
