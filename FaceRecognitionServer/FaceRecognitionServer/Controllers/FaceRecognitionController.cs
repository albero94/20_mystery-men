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

        // GET: api/FaceImages
        [HttpGet]
        public async Task<ActionResult<Boolean>> GetFaceValidation(FaceImage faceImage)
        {
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

            findFaceMatch(faceImage);


            return await findFaceMatch(faceImage);
        }

        private async Task<bool> findFaceMatch(FaceImage faceImage)
        {
            throw new NotImplementedException();
        }

        private IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}