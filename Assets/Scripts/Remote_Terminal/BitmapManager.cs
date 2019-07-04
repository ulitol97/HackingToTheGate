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
	/// As a singleton, a maximum of one instance of this class will be managed by the game.
	/// </summary>
	/// <remarks>The usage of the System.Drawing namespace imposed by the VNC API limits the platforms in which the game
	/// may run to Windows operating systems</remarks>
	public class BitmapManager
	{

		/// <summary>
		/// Creates a new instance of the BitmapManager class.
		/// </summary>
		/// <remarks>Made private to prevent non-singleton constructor use.</remarks>
		private BitmapManager() {}
		
		/// <summary>
		/// Holds the current and only simultaneous instance of the BitmapManager in use by the game.
		/// </summary>
		private static BitmapManager _instance;
	
		/// <summary>
		/// Creates, configures and returns a new singleton instance of the BitmapManager if none has been created,
		/// or returns the currently existing one.
		/// </summary>
		/// <returns>A new BitmapManager instance or the current existing singleton instance.</returns>
		public static BitmapManager GetInstance()
		{
			if (_instance == null)
			{
				_instance = new BitmapManager();
			}
			return _instance;
		}

		/// <summary>
		/// Transforms a Bitmap C# object into a drawable sprite manageable by Unity2D engine.
		/// </summary>
		/// <param name="bmp">Bitmap object to be transformed</param>
		/// <returns>An object of the UnityEngine.Sprite class holding the original Bitmap image.</returns>
		public Sprite SpriteFromBitmap(Bitmap bmp)
		{
			Texture2D texture = BitmapToTexture2D(bmp);
			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}

		/// <summary>
		/// Transforms a Bitmap C# object into a 2 dimensional texture manageable by Unity2D engine.
		/// </summary>
		/// <param name="bmp">Bitmap object to be transformed</param>
		/// <returns>An object of the UnityEngine.Texture2D class holding the original Bitmap image.</returns>
		private Texture2D BitmapToTexture2D(Bitmap bmp)
		{
			byte[] desktopBytes = Bitmap2RawBytes(bmp);

			Texture2D desktopTexture = new Texture2D(bmp.Width, bmp.Height, TextureFormat.BGRA32, false);
			desktopTexture.LoadRawTextureData(desktopBytes);
			desktopTexture.Apply();
			return desktopTexture;
		}

		/// <summary>
		/// Transforms a Bitmap C# object into a 2 dimensional texture manageable by Unity2D engine.
		/// </summary>
		/// <param name="bmp">Bitmap object used as reference for the Texture size.</param>
		/// <param name="bytes">Image bytes to be written into the Texture.</param>
		/// <returns>An object of the UnityEngine.Texture2D class holding the image in the original bytes.</returns>
		public Texture2D BitmapToTexture2D(Bitmap bmp, byte[] bytes)
		{
			Texture2D desktopTexture = new Texture2D(bmp.Width, bmp.Height, TextureFormat.BGRA32, false);
			desktopTexture.LoadRawTextureData(bytes);
			desktopTexture.Apply();
			return desktopTexture;
		}

		/// <summary>
		/// Given a Bitmap object, retrieves the raw pixel data from it and stores it into an array.
		/// </summary>
		/// <param name="bmp">Bitmap whose bytes will be extracted</param>
		/// <returns>A byte array containing the image information of the Bitmap (bmp) provided.</returns>
		public byte[] Bitmap2RawBytes(Bitmap bmp)
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