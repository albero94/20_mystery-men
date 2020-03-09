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
        private static readonly string RECOGNITION_MODEL1 = RecognitionModel.Recognition01;
        private static readonly string _personGroupId = "myroomates";
        private static readonly IFaceClient _client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

        //public async static void Initialize(IServiceProvider serviceProvider)
        public async static void Initialize(DbContext context)
        {

            // Once you create it it stays there, check if exists before creating
            //await client.PersonGroup.GetAsync(personGroupId);

            await _client.PersonGroup.DeleteAsync(_personGroupId);
            await _client.PersonGroup.CreateAsync(_personGroupId, "My Roomates");

            // Define Bill Gates
            var person1 = await _client.PersonGroupPerson.CreateAsync(_personGroupId, "Bill");

            // Get images from DB (as DB not working at the moment I hardcode the path)
            List<FaceImage> faceImages = new List<FaceImage>();
            faceImages.Add(new FaceImage { Id = 1, Name = "image1", Path = $"{Environment.CurrentDirectory}\\..\\Images\\image1.jpg" });
            faceImages.Add(new FaceImage { Id = 2, Name = "image3", Path = $"{Environment.CurrentDirectory}\\..\\Images\\image3.jpg" });

            // Add images to the people in the person group
            foreach (string imagePath in (from faceImage in faceImages
                                          select faceImage.Path))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    await _client.PersonGroupPerson.AddFaceFromStreamAsync(_personGroupId, person1.PersonId, s);
                }
            }

            // Train person group
            await _client.PersonGroup.TrainAsync(_personGroupId);
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await _client.PersonGroup.GetTrainingStatusAsync(_personGroupId);
                Console.WriteLine($"Training status: {trainingStatus.Status}.");
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
        }

        public static async Task<bool> GetFaceMatch(Stream imageStream)
        {
            // problems with Guid? Guid

            IList<Person> people = await _client.PersonGroupPerson.ListAsync(_personGroupId);
            IList<Guid?> persistedFaceIds = new List<Guid?>();
            foreach (var person in people)
            {
                foreach (var face in person.PersistedFaceIds)
                {
                    persistedFaceIds.Add(face);
                }
            }

            List<Guid> sourceFaceIds = new List<Guid>();
            IList<DetectedFace> detectedFaces = await _client.Face.DetectWithStreamAsync(imageStream, recognitionModel: RECOGNITION_MODEL1);
            foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }

            var identifyResults = await _client.Face.IdentifyAsync(sourceFaceIds, _personGroupId);

            string message;
            foreach (var result in identifyResults)
            {
                Person person = await _client.PersonGroupPerson.GetAsync(_personGroupId, result.Candidates[0].PersonId);
                message = $"Person {person.Name} is identified for face in image with Face ID {result.FaceId} " +
                    $"and confidence {result.Candidates[0].Confidence}.";
                Console.WriteLine(message);
            }

            return identifyResults.Contains;
        }

        private static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}
