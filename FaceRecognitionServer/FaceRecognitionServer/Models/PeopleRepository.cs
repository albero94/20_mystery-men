using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace FaceRecognitionServer.Models
{
    public class PeopleRepository
    {
        private readonly List<MyPerson> _peopleList;
        private readonly ILogger<PeopleRepository> _logger;
        private readonly IWebHostEnvironment hostingEnvironment;

        public PeopleRepository(ILogger<PeopleRepository> logger,
            IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            _peopleList = new List<MyPerson>();
        }

        public bool Initialize()
        {
            try
            {
                // Delete exisiting People
                _peopleList.Clear();
                DirectoryInfo directory = new DirectoryInfo(Path.Combine(hostingEnvironment.WebRootPath, "people"));
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                // Define Bill Gates
                MyPerson person = new MyPerson("Bill", false);

                MemoryStream ms = new MemoryStream();
                File.OpenRead("./wwwroot/Images/image1.jpg").CopyTo(ms);
                person.Images.Add(ms);

                MemoryStream ms2 = new MemoryStream();
                File.OpenRead("./wwwroot/Images/image2.jpg").CopyTo(ms2);
                person.Images.Add(ms2);

                person.ImagesPaths = StoreImagesInFiles(person);
                _peopleList.Add(person);


                _logger.LogInformation("People repository initialized.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public bool AddPerson(MyPerson person)
        {
            person.ReadFormFiles();
            person.ImagesPaths = StoreImagesInFiles(person);
            _peopleList.Add(person);
            return true;
        }

        public bool DeletePerson(string name)
        {
            MyPerson person = _peopleList.Where(p => p.Name == name).FirstOrDefault();
            if (DeleteImages(person))
            {
                _peopleList.Remove(person);
                return true;
            }
            return false;
        }

        public List<Object> ListPeopleWithImages()
        {
            List<Object> list = new List<Object>();
            foreach (MyPerson person in _peopleList)
            {
                byte[] file = File.ReadAllBytes(Path.Combine(hostingEnvironment.WebRootPath, "people",
                    person.ImagesPaths.FirstOrDefault()));
                list.Add(new { name = person.Name, image = file });
            }

            //byte[] file = File.ReadAllBytes(Path.Combine(hostingEnvironment.WebRootPath, "people\\Steve Jobs_31aaefb4-1718-45bc-a821-6e382383bb91"));

            return list;
        }

        private List<string> StoreImagesInFiles(MyPerson person)
        {
            List<string> fileNames = new List<string>();
            string uniqueFileName = null;

            foreach (Stream image in person.Images)
            {
                image.Position = 0;
                string folderPath = Path.Combine(hostingEnvironment.WebRootPath, "People");
                uniqueFileName = person.Name + "_" + Guid.NewGuid();
                string filePath = Path.Combine(folderPath, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
                fileNames.Add(uniqueFileName);
            }

            return fileNames;
        }

        private bool DeleteImages(MyPerson person)
        {
            foreach (var imagePath in person.ImagesPaths)
            {
                try
                {
                    File.Delete(Path.Combine(hostingEnvironment.WebRootPath, "People", imagePath));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}