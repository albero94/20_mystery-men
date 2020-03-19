using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FaceRecognitionServer.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FaceRecognitionServer.Controllers
{
    [Route("/{controller}/{action=Index}")]
    [ApiController]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly ILogger _logger;
        public FaceRecognitionController(ILogger<FaceRecognitionController> logger)
        {
            _logger = logger;
        }

        // GET: api/FaceRecognition/Sample
        [HttpGet]
        public async Task<ActionResult<Boolean>> Sample(string imageName = "")
        {
            _logger.LogTrace("Action: Sample");
            try
            {
                if (imageName == "") return false;
                FileStream image = System.IO.File.OpenRead($"{Environment.CurrentDirectory}/Images/{imageName}.jpg");
                return await PersonGroupRepository.IsFaceMatch(image);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Boolean>> FaceMatch([FromForm] IFormFile file)
        {
            try
            {
                Stream image = file.OpenReadStream();
                return await PersonGroupRepository.IsFaceMatch(image);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }

        }

        [HttpGet]
        public string Index()
        {
            _logger.LogTrace("Action: Index");
            return "You are in the face recognition controller";
        }

        [HttpGet]
        public async Task<IList<string>> ListPeople()
        {
            _logger.LogTrace("Action: ListPeople");
            return await PersonGroupRepository.ListPeople();
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreatePerson([FromForm] MyPerson person)
        {
            _logger.LogTrace("Action: CreatePerson");
            return PersonGroupRepository.CreatePersonFromForm(person);
        }

        // Post: api/FaceRecognition
        [HttpPost]
        //public async Task<ActionResult<Boolean>> Initialize(FaceImage faceImage)
        public async Task<bool> Initialize()
        {
            _logger.LogTrace("Action: Initialize");
            return await PersonGroupRepository.Initialize();
        }
    }
}