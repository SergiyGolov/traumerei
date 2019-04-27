using System.IO;
using System.Threading.Tasks;

namespace Traumerei.SaveImage
{

    //source: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving
    public interface IPhotoLibrary
    {

        Task<Stream> PickPhotoAsync();
        Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
    }
}
