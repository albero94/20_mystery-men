using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaceRecognitionServer.Models;

namespace FaceRecognitionServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceImagesController : ControllerBase
    {
        private readonly FaceImageContext _context;

        public FaceImagesController(FaceImageContext context)
        {
            _context = context;
        }

        // GET: api/FaceImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FaceImage>>> GetFaceImages()
        {
            return await _context.FaceImages.ToListAsync();
        }

        // GET: api/FaceImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FaceImage>> GetFaceImage(int id)
        {
            var faceImage = await _context.FaceImages.FindAsync(id);

            if (faceImage == null)
            {
                return NotFound();
            }

            return faceImage;
        }


        private bool FaceImageExists(int id)
        {
            return _context.FaceImages.Any(e => e.Id == id);
        }
    }
}
