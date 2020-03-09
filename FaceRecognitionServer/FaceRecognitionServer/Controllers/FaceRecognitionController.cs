using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FaceRecognitionServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.IO;

namespace FaceRecognitionServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly FaceImageContext _context;
        private readonly string SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("AZURE_FACE_SUBSCRIPTION_KEY");
        private readonly string ENDPOINT = Environment.GetEnvironmentVariable("AZURE_FACE_ENDPOINT");

        public FaceRecognitionController(FaceImageContext context)
        {
            _context = context;
        }

        // GET: api/FaceRecognition
        [HttpGet]
        public async Task<ActionResult<Boolean>> GetSampleValidation()
        {
            FileStream image = System.IO.File.OpenRead($"{Environment.CurrentDirectory}/../Images/image4.jpg");
            return await FaceRecognitionServer.Models.PersonGroup.GetFaceMatch(image);
        }

        private IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }


        // Post: api/FaceRecognition
        [HttpPost]
        //public async Task<ActionResult<Boolean>> Initialize(FaceImage faceImage)
        public async void Initialize()
        {
                FaceRecognitionServer.Models.PersonGroup.Initialize(_context);
        }
    }
}