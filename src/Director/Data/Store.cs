using Microsoft.CodeAnalysis.CSharp.Syntax;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Director.Data
{
    public interface IStore
    {
        Task<IReadOnlyCollection<Face>> GetFacesAsync(long? pageNumber, long? itemsPerPage);

        Task<IEnumerable<Face>> GetFaceAsync(Guid id);

        Task<Guid> AddAsync(Face face);

        Task<IReadOnlyCollection<Face>> GetUnrecognisedAsync(long? pageNumber, long? itemsPerPage);

        Task<IReadOnlyCollection<Face>> GetUnrecognisedAsync(float confidence, long? pageNumber, long? itemsPerPage);

        Task<IReadOnlyCollection<Face>> GetUnrecognisedByAsync(Guid recogniser, long? pageNumber, long? itemsPerPage);

        Task<IReadOnlyCollection<Recogniser>> GetRecognisersAsync();

        Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync();

        Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync(string label);

        Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync(Guid faceId);

        Task<IEnumerable<Recognition>> GetRecognitionAsync(Guid faceId, Guid recognitionId);

        Task<Guid> AddAsync(Recognition recognition);

        Task<IReadOnlyCollection<string>> GetLabelsAsync();
    }

    public class Store : IStore
    {
        private readonly IDatabase _database;

        public Store(IDatabase database)
        {
            _database = database;
        }

        public async Task<IReadOnlyCollection<Face>> GetFacesAsync(long? pageNumber, long? itemsPerPage)
        {
            var result = await _database.FetchAsync<Face>(pageNumber ?? Default.Page, itemsPerPage ?? Default.ItemsPerPage).ConfigureAwait(false);

            return result;
        }
        
        public async Task<IReadOnlyCollection<Face>> GetUnrecognisedAsync(long? pageNumber, long? itemsPerPage)
        {
            var sql = Sql.Builder
                .Select("f.id", "f.uri", "f.location", "f.created")
                .From("faces as f")
                .LeftJoin("recognition as r").On("r.face_id = f.id WHERE r.id IS NULL");

            var result = await _database.FetchAsync<Face>(pageNumber ?? Default.Page, itemsPerPage ?? Default.ItemsPerPage, sql).ConfigureAwait(false);

            return result;
        }
        
        public async Task<IReadOnlyCollection<Face>> GetUnrecognisedAsync(float confidence, long? pageNumber, long? itemsPerPage)
        {
            var sql = Sql.Builder
                .Select("f.id", "f.uri", "f.location", "f.created")
                .From("faces as f")
                .Where("NOT EXISTS (SELECT 1 FROM recognition r WHERE r.face_id = f.id AND r.confidence > @0)");

            sql.Arguments[0] = confidence;

            var result = await _database.FetchAsync<Face>(pageNumber ?? Default.Page, itemsPerPage ?? Default.ItemsPerPage, sql).ConfigureAwait(false);

            return result;
        }

        public async Task<IReadOnlyCollection<Face>> GetUnrecognisedByAsync(Guid recogniser, long? pageNumber, long? itemsPerPage)
        {
            var sql = Sql.Builder
                .Select("f.id", "f.uri", "f.location", "f.created")
                .From("faces as f")
                .Where("NOT EXISTS (SELECT 1 FROM recognition r WHERE r.face_id = f.id AND r.recogniser_id = @0)");

            sql.Arguments[0] = recogniser;

            var result = await _database.FetchAsync<Face>(pageNumber ?? Default.Page, itemsPerPage ?? Default.ItemsPerPage, sql).ConfigureAwait(false);

            return result;
        }

        public async Task<IReadOnlyCollection<Recogniser>> GetRecognisersAsync()
        {
            var result = await _database.FetchAsync<Recogniser>().ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<Face>> GetFaceAsync(Guid id)
        {
            var result = await _database.FetchAsync<Face>("WHERE id = @0", id).ConfigureAwait(false);

            return result;
        }

        public async Task<Guid> AddAsync(Face face)
        {
            await _database.InsertAsync(face).ConfigureAwait(false);

            return face.Id;
        }

        public async Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync()
        {
            var result = await _database.FetchAsync<Recognition>().ConfigureAwait(false);

            return result;
        }
        
        public async Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync(string label)
        {
            var result = await _database.FetchAsync<Recognition>("WHERE label = @0", label).ConfigureAwait(false);

            return result;
        }

        public async Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync(Guid faceId)
        {
            var result = await _database.FetchAsync<Recognition>("WHERE face_id = @0", faceId).ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<Recognition>> GetRecognitionAsync(Guid faceId, Guid recognitionId)
        {
            var result = await _database.FetchAsync<Recognition>("WHERE id = @0", recognitionId).ConfigureAwait(false);

            return result;
        }

        public async Task<Guid> AddAsync(Recognition recognition)
        {
            await _database.InsertAsync(recognition).ConfigureAwait(false);

            return recognition.Id;
        }

        public async Task<IReadOnlyCollection<string>> GetLabelsAsync()
        {
            var results = await _database.FetchAsync<string>("SELECT DISTINCT label FROM recognition").ConfigureAwait(false);

            return results;
        }
    }
}
