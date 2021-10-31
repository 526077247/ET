using UnityEditor;

public class AltasImporterPlatformSettings
{
    public static TextureImporterPlatformSettings ImportSettingsAndroid
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "Android",
                format = TextureImporterFormat.ETC_RGB4,
                allowsAlphaSplitting = true,
                maxTextureSize = 1024
            };
            return setting;
        }
    }

    public static TextureImporterPlatformSettings ImportSettingsIPhone
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "iPhone",
                format = TextureImporterFormat.PVRTC_RGBA4,
                maxTextureSize = 1024
            };
            return setting;
        }
    }

    public static TextureImporterPlatformSettings ImportSettingsPC
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "pc",
                format = TextureImporterFormat.DXT5,
                maxTextureSize = 1024
            };
            return setting;
        }
    }
    public static TextureImporterPlatformSettings Image2ImportSettingsAndroid
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "Android",
                format = TextureImporterFormat.RGBA32,
                maxTextureSize = 1024,
                textureCompression = TextureImporterCompression.Uncompressed
            };
            return setting;
        }
    }

    public static TextureImporterPlatformSettings Image2ImportSettingsIPhone
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "iPhone",
                format = TextureImporterFormat.RGBA32,
                maxTextureSize = 1024,
                textureCompression = TextureImporterCompression.Uncompressed
            };
            return setting;
        }
    }

    public static TextureImporterPlatformSettings Image2ImportSettingsPC
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "PC",
                format = TextureImporterFormat.RGBA32,
                maxTextureSize = 1024,
                textureCompression = TextureImporterCompression.Uncompressed
            };
            return setting;
        }
    }

    public static TextureImporterPlatformSettings BacgroundImgImportSettingsAndroid
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "Android",
                format = TextureImporterFormat.ETC_RGB4,
                //textureCompression = TextureImporterCompression.Compressed,
                //allowsAlphaSplitting = false,
                maxTextureSize = 2048
            };
            return setting;
        }
    }

    public static TextureImporterPlatformSettings BacgroundImgImportSettingsIPhone
    {
        get
        {
            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "iPhone",
                format = TextureImporterFormat.PVRTC_RGB4,
                maxTextureSize = 2048
            };
            return setting;
        }
    }

}
