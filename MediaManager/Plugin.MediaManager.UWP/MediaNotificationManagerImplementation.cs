using System;
using Windows.Media;
using Windows.Storage;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;

        public MediaNotificationManagerImplementation()
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
        }

        public void StartNotification(IMediaFile mediaFile)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        public void StopNotifications()
        {
            _systemMediaTransportControls.DisplayUpdater.ClearAll();
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            UpdateInfoFromMediaFile(mediaFile);
        }

        private async void UpdateInfoFromMediaFile(IMediaFile mediaFile)
        {
            var updater = _systemMediaTransportControls.DisplayUpdater;
            switch (mediaFile.Type)
            {
                case MediaFileType.AudioUrl:
                    break;
                case MediaFileType.AudioFile:
                    if (!await updater.CopyFromFileAsync(MediaPlaybackType.Music,
                            await StorageFile.GetFileFromPathAsync(mediaFile.Url)))
                    {
                        updater.Type = MediaPlaybackType.Music;
                        updater.MusicProperties.Title = mediaFile?.Metadata?.Title ?? string.Empty;
                        updater.MusicProperties.Artist = mediaFile?.Metadata?.Artist ?? string.Empty;
                        updater.MusicProperties.AlbumTitle = mediaFile?.Metadata?.Album ?? string.Empty;
                        updater.MusicProperties.AlbumArtist = mediaFile?.Metadata?.AlbumArtist ?? string.Empty;
                    }
                    break;
                case MediaFileType.VideoUrl:
                    break;
                case MediaFileType.VideoFile:
                    if (!await updater.CopyFromFileAsync(MediaPlaybackType.Video,
                            await StorageFile.GetFileFromPathAsync(mediaFile.Url)))
                    {
                        updater.Type = MediaPlaybackType.Video;
                        updater.VideoProperties.Title = mediaFile?.Metadata?.Title ?? string.Empty;
                        updater.VideoProperties.Subtitle = mediaFile?.Metadata?.DisplaySubtitle ?? string.Empty;
                    }
                    break;
                case MediaFileType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            updater.Update();
        }
    }
}