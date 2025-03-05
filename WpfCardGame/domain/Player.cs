using Steamworks;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfCardGame.network;

namespace WpfCardGame.domain;

public class Player
{
    private Player() { } // useless, but prevents from initializing players without steam

    [Key]
    public int Id { get; set; } // id db

    [Required]
    public ulong SteamId { get; set; }

    [Required] [MaxLength(100)]
    public string Name { get; set; } // name from steam

    public int Wins { get; set; } = 0;  // total wins yet
    public BitmapSource Avatar { get; set; }    // bitmap source => myImage.Source = player.Avatar

    protected Callback<AvatarImageLoaded_t> AvatarImageLoadedCallback;
    

    //// Navigation properties
    //public List<Match> Matches { get; set; }  // Matches played
    //public List<PlayerScore> Scores { get; set; }  // Head-to-head scores


    /// <summary>
    /// Creates a player based on the user currently logged on steam
    /// </summary>
    /// <returns>Player with the logged user's data</returns>
    /// <exception cref="Exception"></exception>
    public static Player CreateFromSteam()
    {
        if (!SteamManager.Initialized)
        {
            throw new Exception("Steam API not initialized");
        }

        

        return new Player
        {
            SteamId = SteamUser.GetSteamID().m_SteamID,
            Name = SteamFriends.GetPersonaName(),
            Avatar = GetAvatarImage(SteamUser.GetSteamID())
        };
    }

    public static Player CreateFromSteamId(CSteamID steamId)
    {
        if (!SteamManager.Initialized)
        {
            throw new Exception("Steam API not initialized");
        }

        return new Player
        {
            SteamId = steamId.m_SteamID,
            Name = SteamFriends.GetFriendPersonaName(steamId),
            Avatar = GetAvatarImage(steamId)
        };
    }

    private static BitmapSource GetAvatarImage(CSteamID steamId)
    {
        if (!SteamManager.Initialized)
        {
            throw new Exception("Steam API not initialized");
        }

        var picInt = SteamFriends.GetLargeFriendAvatar(steamId);
        bool success = SteamUtils.GetImageSize(picInt, out var avatarWidth, out var avatarHeight);
        if (!success)
        {
            Trace.WriteLine("[domain/Player.cs] Couldn't retrieve " + SteamFriends.GetFriendPersonaName(steamId) + " avatar");
            return null;
        }

        int imageSizeInBytes = (int)(avatarWidth * avatarHeight * 4);
        byte[] pAvatarRGBA = new byte[imageSizeInBytes];
        success = SteamUtils.GetImageRGBA(picInt, pAvatarRGBA, imageSizeInBytes);
        if (!success)
        {
            Trace.WriteLine("[domain/Player.cs] Couldn't retrieve " + SteamFriends.GetFriendPersonaName(steamId) + " avatar");
            return null;
        }

        Trace.WriteLine("Got avatar of " + SteamFriends.GetFriendPersonaName(steamId));

        return ConvertByteArrayToImage(pAvatarRGBA, (int)avatarWidth, (int)avatarHeight);
    }

    public static BitmapSource ConvertByteArrayToImage(byte[] pixelData, int width, int height)
    {
        int stride = width * 4; // 4 bytes per pixel (RGBA)

        if (pixelData.Length != width * height * 4)
        {
            MessageBox.Show($"Error: Byte array size {pixelData.Length} does not match expected {width * height * 4}.");
            return null;
        }

        // Swap Red and Blue channels (RGBA → BGRA)
        for (int i = 0; i < pixelData.Length; i += 4)
        {
            (pixelData[i], pixelData[i + 2]) = (pixelData[i + 2], pixelData[i]);
            pixelData[i + 3] = 255; // Ensure Alpha is fully opaque
        }

        return BitmapSource.Create(
            width, height, 96, 96, // Width, Height, DPI X, DPI Y
            PixelFormats.Bgra32,    // WPF uses BGRA instead of RGBA
            null,                   // No palette needed
            pixelData, stride);
    }


}