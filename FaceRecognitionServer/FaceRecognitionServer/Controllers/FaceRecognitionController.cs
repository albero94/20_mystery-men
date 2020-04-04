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
using FaceRecognitionServer.Authorization;

namespace FaceRecognitionServer.Controllers
{
    [Route("/{controller}/{action=Index}")]
    [ApiController]
    [ApiKeyAuth]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly AzurePersonGroupRepository _azurePersonGroupRepository;
        private readonly PeopleRepository _peopleRepository;

        public FaceRecognitionController(ILogger<FaceRecognitionController> logger, 
            AzurePersonGroupRepository azurePersonGroupRepository,
            PeopleRepository peopleRepository)
        {
            _logger = logger;
            _azurePersonGroupRepository = azurePersonGroupRepository;
            this._peopleRepository = peopleRepository;
        }

        // GET: api/FaceRecognition/Sample
        [HttpGet]
        public async Task<ActionResult<Boolean>> Sample(string imageName = "")
        {
            _logger.LogTrace("Action: Sample");
            try
            {
                if (imageName == "") return false;

                FileStream image = System.IO.File.OpenRead($"./wwwroot/Images/{imageName}.jpg");

                return await _azurePersonGroupRepository.IsFaceMatch(image);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Boolean>> FaceMatch([FromForm] IFormFile file)
        {
            try
            {
                Stream image = file.OpenReadStream();
                return await _azurePersonGroupRepository.IsFaceMatch(image);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
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
            return await _azurePersonGroupRepository.ListPeople();
        }

        [HttpPost]
        public async Task<string> CreatePerson([FromForm] MyPerson person)
        {
            _logger.LogTrace("Action: CreatePerson");
            if (_azurePersonGroupRepository.CreatePersonFromForm(person).Result == "Person added")
            {
                _peopleRepository.AddPerson(person);
            }
            return "Person added";
        }

        [HttpDelete]
        public async Task<bool> DeletePerson(string name)
        {
            _logger.LogTrace("Action: DeletePerson");
            return await _azurePersonGroupRepository.DeletePerson(name);
        }

        // Post: api/FaceRecognition
        [HttpPost]
        //public async Task<ActionResult<Boolean>> Initialize(FaceImage faceImage)
        public async Task<bool> Initialize()
        {
            _logger.LogTrace("Action: Initialize");
            try
            {
                if (_azurePersonGroupRepository.Initialize().Result
                    && _peopleRepository.Initialize()) return true;
                else return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        [HttpGet]
        public Object ListPeopleWithImages()
        {
            return _peopleRepository.ListPeopleWithImages();
        }
    }
}