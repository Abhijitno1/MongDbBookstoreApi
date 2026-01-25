using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbBookstoreApi.Models;
using MongoDbBookstoreApi.Services;
using System.Linq.Expressions;

namespace MongoDbBookstoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaRepository _repo;
        public MediaController(IMediaRepository repo) => _repo = repo;

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file.Length > 16 * 1024 * 1024) return BadRequest("File exceeds 16MB limit.");

            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                var media = new MediaFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = ms.ToArray()
                };
                // If ID is missing, generate it locally before sending to Mongo
                if (string.IsNullOrEmpty(media.Id))
                {
                    media.Id = ObjectId.GenerateNewId().ToString();
                }

                var id = await _repo.UpsertAsync(media);
                return Ok(new { id });
            }
            catch (MongoWriteException ex) when(ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // Return 409 Conflict if the filename already exists
                return Conflict(new { message = $"A file named '{file.FileName}' already exists." });
            }
        }

        [EnableCors("AllowAll")]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(string id)
        {
            var media = await _repo.GetByIdAsync(id);
            if (media == null) return NotFound();
            return File(media.Data, media.ContentType, media.FileName);
        }
    }
}
