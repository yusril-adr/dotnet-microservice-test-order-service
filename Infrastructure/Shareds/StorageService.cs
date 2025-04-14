using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DotNetOrderService.Constants.Logger;
using DotNetOrderService.Constants.Storage;
using DotNetOrderService.Domain.File.Dto;
using DotNetOrderService.Infrastructure.Exceptions;

namespace DotNetOrderService.Infrastructure.Shareds
{

    public class StorageService
    {
        public StorageService(IConfiguration config, ILoggerFactory loggerFactory
        )
        {
            _config = config;
            _logger = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);

            storage = _config["Storage"] ?? StorageConstant.LOCAL;

            switch (storage)
            {
                case StorageConstant.AWS:
                    AmazonS3Config awsS3Config = new()
                    {
                        ServiceURL = _config["S3:endpoint"],
                        DisableHostPrefixInjection = true,
                        ForcePathStyle = true
                    };

                    awsS3Client = new(
                        _config["S3:accessKeyId"],
                        _config["S3:secretAccessKey"],
                        awsS3Config
                    );
                    break;

                case StorageConstant.MINIO:
                    // TODO: need to implement minio integration
                    break;
            }
        }

        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        readonly string storage;
        readonly AmazonS3Client awsS3Client;

        public async Task<FileDto> UploadAsync(FileUploadDto fileUpload)
        {
            fileUpload.Path ??= fileUpload.File.GenerateDestPath();

            switch (storage)
            {
                case StorageConstant.AWS:
                    try
                    {
                        AWSConfigsS3.UseSignatureVersion4 = true;
                        using var newMemoryStream = new MemoryStream();
                        fileUpload.File.CopyTo(newMemoryStream);
                        string fileKey = $"{fileUpload.Path}/{fileUpload.File.FileName}";

                        PutObjectRequest request = new()
                        {
                            BucketName = _config["S3:bucketName"],
                            Key = fileKey,
                            ContentType = fileUpload.File.ContentType,
                            InputStream = newMemoryStream,
                        };

                        await awsS3Client.PutObjectAsync(request);

                        return Download(fileKey);
                    }
                    catch (AmazonS3Exception e)
                    {
                        _logger.LogError("Error uploading file to S3: {e.Message}", e.Message);
                        throw new ServiceUnavailableException();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Server error: {e.Message}", e.Message);
                        throw;
                    }
                case StorageConstant.LOCAL:
                    return Download(fileUpload.Path);
                case StorageConstant.MINIO:
                    // TODO: need to implement minio integration
                    break;
            }

            return null;
        }

        public FileDto Download(string fileKey)
        {
            FileDto FileDto = new();

            switch (storage)
            {
                case StorageConstant.AWS:
                    try
                    {
                        AWSConfigsS3.UseSignatureVersion4 = true;
                        GetPreSignedUrlRequest requestGet = new()
                        {
                            BucketName = _config["S3:bucketName"],
                            Key = fileKey,
                            Expires = DateTime.Now.AddHours(1),
                            Protocol = Protocol.HTTPS
                        };

                        FileDto.Name = Path.GetFileName(fileKey);
                        FileDto.Key = fileKey;
                        FileDto.DownloadUrl = awsS3Client.GetPreSignedURL(requestGet);

                        return FileDto;
                    }
                    catch (AmazonS3Exception e)
                    {
                        _logger.LogError("Error downloading file to S3: {e.Message}", e.Message);
                        throw new ServiceUnavailableException();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Server error: {e.Message}", e.Message);
                        throw;
                    }

                case StorageConstant.LOCAL:
                    var url = "{0}{1}/{2}";
                    FileDto.Key = fileKey;
                    FileDto.DownloadUrl = string.Format(url, _config["App:BaseURL"] ?? "http://localhost:5001", _config["LocalStorage:VirtualPath"] ?? "/temp", fileKey);
                    return FileDto;
                case StorageConstant.MINIO:
                    // TODO Need to be code here if going to use AWS
                    break;
            }

            throw new FileNotFoundException("Not Found");
        }
    }
}