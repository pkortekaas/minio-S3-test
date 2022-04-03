using Amazon.S3;
using Amazon.S3.Model;
using System.Net;

namespace s3.test
{
    internal static class AmazonS3ClientExtensions
    {
        public static async Task<HttpStatusCode> UploadToAsync(this AmazonS3Client client, string bucketName,
                                                                string path, Stream stream)
        {
            PutObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = path,
                InputStream = stream
            };
            PutObjectResponse response = await client.PutObjectAsync(request);
            return response.HttpStatusCode;
        }

        public static async Task<HttpStatusCode> DownloadToAsync(this AmazonS3Client client, string bucketName,
                                                                    string path, Stream stream)
        {
            GetObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = path
            };
            GetObjectResponse response = await client.GetObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.ResponseStream)
                {
                    await responseStream.CopyToAsync(stream);
                }
            }
            return response.HttpStatusCode;
        }

        public static async Task<HttpStatusCode> CreateAsync(this AmazonS3Client client, string bucketName, string path)
        {
            PutObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = path
            };
            PutObjectResponse response = await client.PutObjectAsync(request);
            return response.HttpStatusCode;
        }

        public static async Task<HttpStatusCode> DeleteAsync(this AmazonS3Client client, string bucketName, string path)
        {
            DeleteObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = path
            };
            DeleteObjectResponse response = await client.DeleteObjectAsync(request);
            return response.HttpStatusCode;
        }

        public static async Task<HttpStatusCode> DeleteIfExistsAsync(this AmazonS3Client client, string bucketName, string path)
        {
            if (await client.ExistsAsync(bucketName, path))
            {
                DeleteObjectRequest request = new()
                {
                    BucketName = bucketName,
                    Key = path
                };
                DeleteObjectResponse response = await client.DeleteObjectAsync(request);
                return response.HttpStatusCode;
            }
            return HttpStatusCode.OK;
        }

        public static async Task<bool> ExistsAsync(this AmazonS3Client client, string bucketName, string path)
        {
            ListObjectsRequest request = new()
            {
                BucketName = bucketName,
                Prefix = path,
                MaxKeys = 1
            };
            ListObjectsResponse response = await client.ListObjectsAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK &&
                response.S3Objects.Count == 1 &&
                string.Equals(response.S3Objects[0].Key, path, StringComparison.InvariantCulture);
        }
    }
}