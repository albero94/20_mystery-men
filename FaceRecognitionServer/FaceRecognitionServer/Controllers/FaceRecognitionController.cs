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
    [Route("/{controller}/{action=ListPeople}")]
    [ApiController]
    public class FaceRecognitionController : ControllerBase
    {
        // GET: api/FaceRecognition/Sample
        [HttpGet]
        public async Task<ActionResult<Boolean>> Sample(string imageName = "")
        {
            try
            {
                if (imageName == "") return false;
                FileStream image = System.IO.File.OpenRead($"{Environment.CurrentDirectory}/Images/{imageName}.jpg");
                return await PersonGroup.IsFaceMatch(image);
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
                return await PersonGroup.IsFaceMatch(image);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
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
            return PersonGroup.CreatePersonFromForm(person);
        }

        // Post: api/FaceRecognition
        [HttpPost]
        //public async Task<ActionResult<Boolean>> Initialize(FaceImage faceImage)
        public async Task<bool> Initialize()
        {
            return await PersonGroup.Initialize();
        }
    }
}