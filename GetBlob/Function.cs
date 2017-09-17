using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using Amazon.S3.Model;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GetBlob
{
    public class Function
    {
        public async Task<byte[]> FunctionHandler(Request request, ILambdaContext context)
        {
            using (var client = new AmazonS3Client())
            {
                try
                {
                    var getRequest = new GetObjectRequest { BucketName = request.Bucket, Key = request.Key };
                    using (var response = await client.GetObjectAsync(getRequest))
                        return ReadStream(response.ResponseStream);
                }
                catch (AmazonS3Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new byte[0];
                }
            }
        }

        public byte[] ReadStream(Stream responseStream)
        {
            using (var ms = new MemoryStream())
            {
                responseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
