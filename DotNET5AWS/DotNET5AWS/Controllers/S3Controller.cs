using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using Amazon.Runtime;
using System.IO;
using System.Net.Http.Headers;
using DotNet5_Angular_AWS.Model;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotNET5AWS
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3Controller : ControllerBase
    {
        private AmazonS3Client s3Client;
        public S3Controller()
        {
            s3Client = new AmazonS3Client();
        }

        // GET: api/<S3Controller>
        [HttpGet]
        public async Task<ListBucketsResponse> Get()
        {

            return await s3Client.ListBucketsAsync();
        }

        // GET api/<S3Controller>/5
        [HttpGet("{bucketName}")]
        public async Task<IEnumerable<S3ObjectLocal>> Get(string bucketName)
        {
            var listObjectsV2Paginator = s3Client.Paginators.ListObjectsV2(new ListObjectsV2Request
            {
                BucketName = bucketName,
                FetchOwner = true,
                Delimiter = "/",
                Prefix = "PricingImports/"
            });

            //await foreach(var entry in listObjectsV2Paginator.S3Objects)
            //{
            //    Console.WriteLine($"key = {entry.Key} size = {entry.Size}");

            //}

            List<S3ObjectLocal> S3Objects = new List<S3ObjectLocal>();

            await foreach(S3Object ob in listObjectsV2Paginator.S3Objects )
            {
                if(ob.Size > 0)
                {
                    S3Objects.Add(new S3ObjectLocal(ob));
                }
            }

            return S3Objects.AsEnumerable();
        }

        // GET api/<S3Controller>/5
        [HttpGet("{bucketName}/{objectKey}")]
        public async Task<Boolean> GetObject([FromRoute] string bucketName, [FromRoute] string objectKey)
        {
            try
            {
                var getObjectRequest = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = objectKey.Replace('@', '/')
                };

                ResponseHeaderOverrides responseHeaders = new ResponseHeaderOverrides();
                responseHeaders.CacheControl = "No-cache";
                responseHeaders.ContentDisposition = "attachment";

                getObjectRequest.ResponseHeaderOverrides = responseHeaders;

                using (GetObjectResponse response = await s3Client.GetObjectAsync(getObjectRequest))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"];
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);
                    await response.WriteResponseStreamToFileAsync(objectKey, false, default);

                }
                return true;
            }
            catch(AmazonS3Exception e)
            {
                return false;
            }
        }


        // POST api/<S3Controller>
        [HttpPost]
        public async Task<IEnumerable<S3ObjectLocal>> Post()
        {
            List<S3ObjectLocal> S3Objects = new List<S3ObjectLocal>();
            try
            {
                var form = await Request.ReadFormAsync();
                var files = form.Files;
                int nfiles = 0;

                if(files.Any( file => file.Length == 0))
                {
                    return null;
                }

                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    try
                    {
                        var putRequest = new PutObjectRequest
                        {
                            BucketName = "test-208561-pricex-pricex-s3",
                            Key = "PricingImports/" + fileName,
                            InputStream = file.OpenReadStream()
                        };

                        putRequest.Metadata.Add("x-amz-meta-title", fileName);

                        PutObjectResponse response = await s3Client.PutObjectAsync(putRequest);

                        S3Objects.Add(new S3ObjectLocal()
                        {
                            bucket = "test-208561-pricex-pricex-s3",
                            key = "PricingImports/" + fileName,
                        });
                    }
                    catch (AmazonS3Exception e)
                    {
                        return S3Objects.AsEnumerable();
                    }
                }

                return S3Objects.AsEnumerable();
            }
            catch (Exception ex)
            {
                return S3Objects.AsEnumerable();
            }
        }

        // PUT api/<S3Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<S3Controller>/5
        [HttpDelete("{bucketName}/{objectKey}")]
        public async Task<Boolean> Delete(string bucketName, string objectkey)
        {
            objectkey = objectkey.Replace('@', '/');
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = objectkey
            };

            DeleteObjectResponse response = await s3Client.DeleteObjectAsync(deleteRequest);

            if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
