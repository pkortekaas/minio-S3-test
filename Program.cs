using System.Text;
using Microsoft.Extensions.Configuration;
using Amazon.S3;

namespace s3.test
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(MainAsync).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var settings = new ConfigurationBuilder()
                .AddJsonFile("settings.json", false, false)
                .Build();

            var minioSettings = settings.GetSection("MinIO");

            AmazonS3Config config = new()
            {
                AuthenticationRegion = minioSettings["Region"], // Should match MINIO_REGION or Server Location
                ServiceURL = minioSettings["ServiceURL"],       // Use URL of your MinIO server
                ForcePathStyle = true                           // MUST be true to work correctly with MinIO server
            };

            AmazonS3Client amazonS3Client = new(minioSettings["AccessKey"], minioSettings["SecretKey"], config);
            string bucket = minioSettings["Bucket"];
            string fileName = "chicken";

            // Create file
            byte[] data = Encoding.UTF8.GetBytes($"We do chicken right {DateTime.Now}.");
            using (MemoryStream ms = new(data))
            {
                var result = await amazonS3Client.CopyFileAsync(bucket, fileName, ms);
            }

            // Read file
            using (MemoryStream ms = new())
            {
                var result = await amazonS3Client.DownloadFileAsync(bucket, fileName, ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                Console.WriteLine(content);
            }

            // List buckets
            var listBucketResponse = await amazonS3Client.ListBucketsAsync();

            foreach (var b in listBucketResponse.Buckets)
            {
                Console.WriteLine($"bucket '{b.BucketName}' created at {b.CreationDate}");
            }

            // Show files in first bucket
            if (listBucketResponse.Buckets.Count > 0)
            {
                var bucketName = listBucketResponse.Buckets[0].BucketName;

                var listObjectsResponse = await amazonS3Client.ListObjectsAsync(bucketName);

                foreach (var obj in listObjectsResponse.S3Objects)
                {
                    Console.WriteLine($"key = '{obj.Key}' | size = {obj.Size} | tags = '{obj.ETag}' | modified = {obj.LastModified}");
                }
            }

            // Remove file
            if (await amazonS3Client.ExistsAsync(bucket, fileName))
            {
                await amazonS3Client.DeleteAsync(bucket, fileName);
            }
        }
    }
}