using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace FaceRecognitionServer.Models
{
    public class MyPerson
    {
        public string Name { get; set; }
        public IList<IFormFile> FormFiles { get; set; }
        public IList<Stream> Images { get; set; }
        public bool IsFromForm { get; set; }

        public MyPerson()
        {
            this.Name = "";
            this.FormFiles = new List<IFormFile>();
            this.Images = new List<Stream>();
            IsFromForm = true;
        }
        public MyPerson(string name, bool isFromForm)
        {
            this.Name = name;
            this.IsFromForm = IsFromForm;
            this.FormFiles = new List<IFormFile>();
            this.Images = new List<Stream>();
        }

        public void ReadFormFiles()
        {
            if (IsFromForm)
            {
                Images.Clear();
                foreach (var file in FormFiles)
                {
                    Images.Add(file.OpenReadStream());
                }
            }
        }
    }
}
