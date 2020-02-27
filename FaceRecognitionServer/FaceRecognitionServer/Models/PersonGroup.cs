using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.IO;

namespace FaceRecognitionServer.Models
{
    public class PersonGroup
    {
        private static readonly string SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("AZURE_FACE_SUBSCRIPTION_KEY");
        private static readonly string ENDPOINT = Environment.GetEnvironmentVariable("AZURE_FACE_ENDPOINT");

        public async static void Initialize(IServiceProvider serviceProvider)
        {
            var context = new FaceImageContext(
                serviceProvider.GetRequiredService<DbContextOptions<FaceImageContext>>());

            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

            // Once you create it it stays there, check if exists before creating
            string personGroupId = "myroomates";
            await client.PersonGroup.GetAsync(personGroupId);
            await client.PersonGroup.CreateAsync(personGroupId, "My Roomates");

            // Define Bill Gates
            var person1 = await client.PersonGroupPerson.CreateAsync(personGroupId, "Bill");
            List<FaceImage> faceImages = new List<FaceImage>();
            faceImages.Add(new FaceImage { Id = 1, Name = "image1", Path = $"{Environment.CurrentDirectory}\\..\\Images\\image1.jpg" });
            faceImages.Add(new FaceImage { Id = 2, Name = "image3", Path = $"{Environment.CurrentDirectory}\\..\\Images\\image3.jpg" });

            foreach (string imagePath in
                    (from faceImage in faceImages
                     select faceImage.Path))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    await client.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, person1.PersonId, s);
                }
            }



            //}
        }

        private static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}
