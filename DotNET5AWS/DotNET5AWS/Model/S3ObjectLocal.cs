using System;
using System.Collections.Generic;
using Amazon.S3.Model;
using System.Threading.Tasks;

namespace DotNet5_Angular_AWS.Model
{
    public class S3ObjectLocal
    {
        public string bucket { get; set; }
        public string key { get; set; }
        public string type { get; set; }
        public DateTime lastmodified { get; set; }
        public float size { get; set; }

        public S3ObjectLocal() { }
        public S3ObjectLocal(S3Object s3object)
        {
            this.bucket = s3object.BucketName;
            this.key = s3object.Key;
            this.size = s3object.Size;
            this.lastmodified = DateTime.Parse(s3object.LastModified.ToLongDateString());
            this.type = this.key.Substring(this.key.LastIndexOf('.')+1).ToUpper();
        }

    }
}
