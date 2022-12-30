using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minio;

public class MultipartUploadHandler
{
    public readonly MinioClient client;
    public readonly string bucketName;
    public readonly string objectName;
    public readonly string uploadId;

    public int NextPartNumber { get; private set; } = 1;

    private readonly Dictionary<int, string> etags = new();

    internal MultipartUploadHandler(MinioClient client, string bucketName, string objectName, string uploadId)
    {
        this.client = client;
        this.bucketName = bucketName;
        this.objectName = objectName;
        this.uploadId = uploadId;
    }

    public int ReservePartNumber()
    {
        return NextPartNumber++;
    }

    public async Task PutObjectAsync(Stream objectStreamData, int? partNumber = null, long? objectSize = null, string contentType = null, byte[] requestBody = null, Dictionary<string, string> hdr = null, CancellationToken cancellationToken = default)
    {
        int thisPartNumber = partNumber ?? NextPartNumber++;

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithObjectSize(objectSize ?? objectStreamData.Length)
            .WithUploadId(uploadId)
            .WithRequestBodyStream(objectStreamData)
            .WithPartNumber(thisPartNumber);

        if (contentType != null)
            putObjectArgs.WithContentType(contentType);

        if (requestBody != null)
            putObjectArgs.WithRequestBody(requestBody);

        if (hdr != null)
            putObjectArgs.WithHeaders(hdr);

        var etag = await client.PutObjectSinglePartAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);

        lock (etags)
        {
            etags[thisPartNumber] = etag;
        }
    }

    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        var completeMultipartUploadArgs = new CompleteMultipartUploadArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithUploadId(uploadId)
            .WithETags(etags);

        return client.CompleteMultipartUploadAsync(completeMultipartUploadArgs, cancellationToken);
    }
}