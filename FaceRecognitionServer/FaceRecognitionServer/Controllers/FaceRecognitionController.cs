using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FaceRecognitionServer.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FaceRecognitionServer.Controllers
{
    [Route("api/{controller}/{action}")]
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

        // GET: api/FaceRecognition/Sample
        [HttpGet]
        public async Task<ActionResult<Boolean>> Sample(string imageName = "")
        {
            if (imageName == "") return false;
            FileStream image = System.IO.File.OpenRead($"{Environment.CurrentDirectory}/../Images/{imageName}.jpg");
            return await PersonGroup.IsFaceMatch(image);
        }

        [HttpPost]
        public async Task<ActionResult<Boolean>> FaceMatch([FromForm] IFormFile file)
        {
            try
            {
                Stream image = file.OpenReadStream();
                if (true) return await PersonGroup.IsFaceMatch(image);
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        [HttpGet]
        public async Task<IList<string>> ListPeople()
        {
            return await PersonGroup.ListPeople();
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreatePerson([FromForm] MyPerson person)
        {
            PersonGroup.CreatePersonFromForm(person);
            return "Person uploaded";
        }

        // Post: api/FaceRecognition
        [HttpPost]
        //public async Task<ActionResult<Boolean>> Initialize(FaceImage faceImage)
        public async void Initialize()
        {
            PersonGroup.Initialize(_context);
        }
    }
}