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
using Microsoft.Extensions.Logging;

namespace FaceRecognitionServer.Models
{
    public class AzurePersonGroupRepository
    {
        private readonly string SUBSCRIPTION_KEY;
        private readonly string ENDPOINT;
        private readonly string RECOGNITION_MODEL1;
        private readonly string _personGroupId;
        private readonly IFaceClient _client;
        private readonly double _confidenceCoefficient;

        private readonly ILogger _logger;
        public AzurePersonGroupRepository(ILogger<AzurePersonGroupRepository> logger)
        {
            _logger = logger;
            SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("AZURE_FACE_SUBSCRIPTION_KEY");
            ENDPOINT = Environment.GetEnvironmentVariable("AZURE_FACE_ENDPOINT");
            RECOGNITION_MODEL1 = RecognitionModel.Recognition01;
            _personGroupId = "myroomates";
            _client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);
            _confidenceCoefficient = 0.5;
        }
        public async Task<bool> Initialize()
        {
            try
            {

                await _client.PersonGroup.DeleteAsync(_personGroupId);
                await _client.PersonGroup.CreateAsync(_personGroupId, "My Roomates");

                // Define Bill Gates
                MyPerson person = new MyPerson("Bill", false);
;
                person.Images.Add(File.OpenRead("./wwwroot/Images/image1.jpg"));
                person.Images.Add(File.OpenRead("./wwwroot/Images/image3.jpg"));

                await AddPersonToPersonGroup(person);
                _logger.LogInformation("Repository initialized.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        public async Task<bool> AddPersonToPersonGroup(MyPerson myPerson)
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
                    _logger.LogTrace($"Training status: {trainingStatus.Status}.");
                    if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }

                    if (trainingStatus.Status == TrainingStatusType.Failed)
                    {
                        throw new Exception(message: "Training failed");
                    }
                }

                _logger.LogInformation("Person added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }


        internal async Task<string> CreatePersonFromForm(MyPerson person)
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

                if (AddPersonToPersonGroup(person).Result)
                {
                    return "Person added";
                }

                return "Person was not created";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "Error, person was not created";
            }

        }

        public async Task<bool> IsFaceMatch(Stream imageStream)
        {
            List<Guid> sourceFaceIds = new List<Guid>();
            try
            {
                IList<DetectedFace> detectedFaces = await _client.Face.DetectWithStreamAsync(imageStream, recognitionModel: RECOGNITION_MODEL1);

                foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }

                var result = (await _client.Face.IdentifyAsync(sourceFaceIds, _personGroupId))[0];

                if (result.Candidates.Count > 0)
                {
                    foreach (var candidate in result.Candidates)
                    {
                        Person person = await _client.PersonGroupPerson.GetAsync(_personGroupId, candidate.PersonId);

                        _logger.LogInformation($"Person {person.Name} is identified for face in image with Face ID {result.FaceId} " +
                            $"and confidence {result.Candidates[0].Confidence}.");
                    }
                }
                else _logger.LogInformation("Face match not found");

                if (result.Candidates.Any(c => c.Confidence > _confidenceCoefficient))
                {
                    _logger.LogInformation("Face match found");
                    return true;
                }
                else
                {
                    _logger.LogInformation("Face match not found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<IList<string>> ListPeople()
        {
            try
            {
                IList<Person> people = await _client.PersonGroupPerson.ListAsync(_personGroupId);
                IList<string> names = (from person in people select person.Name).ToList();

                _logger.LogInformation("Listing people");
                return names;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<string>();
            }

        }

        public async Task<bool> DeletePerson(string name)
        {
            try
            {
                var people = await _client.PersonGroupPerson.ListAsync(_personGroupId);
                var person = people.Where(p => p.Name == name).FirstOrDefault();
                await _client.PersonGroupPerson.DeleteAsync(_personGroupId, person.PersonId);

                _logger.LogInformation($"{person.Name} deleted successfully.");
                return true;
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        private IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}
