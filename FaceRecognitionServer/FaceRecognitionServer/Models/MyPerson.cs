using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace FaceRecognitionServer.Models
{
    public class MyPerson
    {
        public string Name { get; set; }
        public IList<IFormFile> FormFiles { get; set; }
        public IList<Stream> Images { get; set; }

        public void ReadFormFiles()
        {
            foreach(var file in FormFiles)
            {
                Images.Add(file.OpenReadStream());
            }
        }
    }
}
