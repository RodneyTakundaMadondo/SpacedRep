using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace StudyReminder.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        public string ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")??string.Empty;
        public async Task<(string, string)> SaveUserNotes(IFormFile file)
        {
           
            Cloudinary cloudinary = new Cloudinary(ApiKey); 
            var uploadParamas = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                PublicId = Guid.NewGuid().ToString(),
                AccessMode ="public"
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParamas);
            string fileUrl = uploadResult.SecureUrl.ToString();
            string publicId = uploadResult.PublicId;

            return (fileUrl, publicId);
        }
        public async Task DeleteUserNote(string publicId)
        {
            try
            {
                var cloudinary = new Cloudinary(ApiKey);

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Raw
                };
                await cloudinary.DestroyAsync(deleteParams);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }


    }
}
