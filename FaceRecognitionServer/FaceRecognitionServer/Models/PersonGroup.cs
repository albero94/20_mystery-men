using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaceRecognitionServer.Models
{
    public class PersonGroup
    {
        private static readonly string SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("AZURE_FACE_SUBSCRIPTION_KEY");
        private static readonly string ENDPOINT = Environment.GetEnvironmentVariable("AZURE_FACE_ENDPOINT");
        private static readonly string RECOGNITION_MODEL1 = RecognitionModel.Recognition01;
        private static readonly string _personGroupId = "myroomates";
        private static readonly IFaceClient _client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);
        private static double _confidenceCoefficient = 0.5;
        //public async static void Initialize(IServiceProvider serviceProvider)
        public async static Task<bool> Initialize()
        {
            try
            {

                await _client.PersonGroup.DeleteAsync(_personGroupId);
                await _client.PersonGroup.CreateAsync(_personGroupId, "My Roomates");

                // Define Bill Gates
                MyPerson person = new MyPerson("Bill", false);
                person.Images.Add(File.OpenRead($"{Environment.CurrentDirectory}\\Images\\image1.jpg"));
                person.Images.Add(File.OpenRead($"{Environment.CurrentDirectory}\\Images\\image3.jpg"));

                await AddPersonToPersonGroup(person);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }

        }

        public async static Task<bool> AddPersonToPersonGroup(MyPerson myPerson)
        {
            try
            {

                var azurePerson = await _client.PersonGroupPerson.CreateAsync(_personGroupId, myPerson.Name);

                myPerson.ReadFormFiles();
                foreach (var image in myPerson.Images)
                {
                    await _client.PersonGroupPerson.AddFaceFromStreamAsync(_personGroupId, azurePerson.PersonId, image);
                }

                await _client.PersonGroup.TrainAsync(_personGroupId);
                
                while (true)
                {
                    await Task.Delay(1000);
                    var trainingStatus = await _client.PersonGroup.GetTrainingStatusAsync(_personGroupId);
                    Console.WriteLine($"Training status: {trainingStatus.Status}.");
                    if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }


        internal static string CreatePersonFromForm(MyPerson person)
        {
            try
            {
                person.ReadFormFiles();
                foreach (var image in person.Images)
                {
                    if (IsFaceMatch(image).Result)
                    {
                        return "This person already exists";
                    }
                }

                if (AddPersonToPersonGroup(person).Result) return "Person added";
                return "Error, person was not created";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return "Error, person was not created";
            }

        }

        public static async Task<bool> IsFaceMatch(Stream imageStream)
        {
            List<Guid> sourceFaceIds = new List<Guid>();
            try
            {
                IList<DetectedFace> detectedFaces = await _client.Face.DetectWithStreamAsync(imageStream, recognitionModel: RECOGNITION_MODEL1);

                foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }

                var result = (await _client.Face.IdentifyAsync(sourceFaceIds, _personGroupId))[0];

                string message;
                if (result.Candidates.Count > 0)
                {
                    foreach (var candidate in result.Candidates)
                    {
                        Person person = await _client.PersonGroupPerson.GetAsync(_personGroupId, candidate.PersonId);

                        Console.WriteLine($"Person {person.Name} is identified for face in image with Face ID {result.FaceId} " +
                            $"and confidence {result.Candidates[0].Confidence}.");
                    }
                }
                else Console.WriteLine("No match was found");

                return result.Candidates.Any(c => c.Confidence > _confidenceCoefficient);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false;
            }
        }

        public static async Task<IList<string>> ListPeople()
        {
            try
            {
                IList<Person> people = await _client.PersonGroupPerson.ListAsync(_personGroupId);
                IList<string> names = (from person in people select person.Name).ToList();
                return names;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw ex;
            }

        }


        private static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}
