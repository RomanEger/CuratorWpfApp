using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Dapper;

namespace CuratorWpfApp.Models.ServicesDB
{
    class ImageService
    {
        private string conStr = ConfigStr.ConStr;

        public byte[] GetBytes(int id)
        {
            using (IDbConnection db = new SqlConnection(conStr))
            {
                var r = db.QueryFirstOrDefault<byte[]>($"SELECT Photo FROM StudentsT WHERE Id={id}");

                return r;
            }
        }

        public ImageSource ByteToImage(byte[] imageData)
        {
            MemoryStream ms = new MemoryStream(imageData);
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
