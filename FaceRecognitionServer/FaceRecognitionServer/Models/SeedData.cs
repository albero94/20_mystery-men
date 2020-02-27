using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceRecognitionServer.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new FaceImageContext(
                serviceProvider.GetRequiredService<DbContextOptions<FaceImageContext>>()))
            {
                if (context.FaceImages.Any()) return;

                context.FaceImages.Add(
                new FaceImage { Id = 1, Name = "image1", Path = $"{Environment.CurrentDirectory}\\..\\Images\\image1.jpg" }
                );
                context.FaceImages.Add(
                new FaceImage { Id = 2, Name = "image2", Path = $"{Environment.CurrentDirectory}\\..\\Images\\image2.jpg" }
                );

                context.SaveChanges();
            }
        }
    }
}
