using Microsoft.CodeAnalysis.CSharp.Syntax;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Director.Data
{
    public interface IStore
    {
        Task<IReadOnlyCollection<Face>> GetFacesAsync();

        Task<IEnumerable<Face>> GetFaceAsync(Guid id);

        Task<Guid> AddAsync(Face face);

        Task<IReadOnlyCollection<Recogniser>> GetRecognisersAsync();

        Task<IReadOnlyCollection<Recognition>> GetRecognitionsAsync(Guid faceId);

        Task<IEnumerable<Recognition>> GetRecognitionAsync(Guid faceId, Guid recognitionId);

        Task<Guid> AddAsync(Recognition recognition);
    }

    public class Store : IStore
    {
        private readonly IDatabase _database;

        public Store(IDatabase database)
        {
            _database = database;
        }

        public async Task<IReadOnlyCollection<Face>> GetFacesAsync()
        {
            var result = await _database.FetchAsync<Face>().ConfigureAwait(false);

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
    }
}
