using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Remote_Terminal
{
	
	/// <summary>
	/// The BitmapManager class is in charge of handling the conversion from the Bitmap objects received from the VNC
	/// server to Unity suitable sprites or textures that can be rendered in-game by the engine.
	/// </summary>
	/// <remarks>The usage of the System.Drawing namespace imposed by the VNC API limits the platforms in which the game
	/// may run to Windows operating systems</remarks>
	public static class BitmapManager
	{
		
		/// <summary>
		/// Transforms a Bitmap C# object into a 2 dimensional texture manageable by Unity2D engine.
		/// </summary>
		/// <param name="bmp">Bitmap object used as reference for the Texture size.</param>
		/// <param name="bytes">Image bytes to be written into the Texture.</param>
		/// <returns>An object of the UnityEngine.Texture2D class holding the image in the original bytes.</returns>
		public static Texture2D BitmapToTexture2D(Bitmap bmp, byte[] bytes)
		{
			Texture2D desktopTexture = new Texture2D(bmp.Width, bmp.Height, TextureFormat.BGRA32, false);
			desktopTexture.LoadRawTextureData(bytes);
			desktopTexture.Apply();
			bmp.Dispose();
			return desktopTexture;
		}

		/// <summary>
		/// Given a Bitmap object, retrieves the raw pixel data from it and stores it into an array.
		/// </summary>
		/// <param name="bmp">Bitmap whose bytes will be extracted</param>
		/// <returns>A byte array containing the image information of the Bitmap (bmp) provided.</returns>
		public static byte[] Bitmap2RawBytes(Bitmap bmp)
		{
			byte[] bytesStream;
			
			using (var stream = new MemoryStream())
			{
				bmp.Save(stream, ImageFormat.Bmp);
				bytesStream = stream.ToArray().Skip(54).ToArray();
			}
			return bytesStream;
		}
	}
}