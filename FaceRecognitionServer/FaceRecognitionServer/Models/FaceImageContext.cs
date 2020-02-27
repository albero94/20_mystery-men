using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceRecognitionServer.Models
{
    public class FaceImageContext : DbContext
    {
        public FaceImageContext(DbContextOptions<FaceImageContext> options) : base(options)
        {
        }

        public DbSet<FaceImage> FaceImages { get; set; }

    }
}
