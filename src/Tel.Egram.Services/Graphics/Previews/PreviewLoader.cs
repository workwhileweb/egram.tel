using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using TdLib;
using Tel.Egram.Services.Persistance;

namespace Tel.Egram.Services.Graphics.Previews
{
    public class PreviewLoader : IPreviewLoader
    {
        private static readonly string[] LowQualitySizes = { "m", "b", "a", "s" };
        private static readonly string[] HighQualitySizes = { "x", "c" };
        private readonly IPreviewCache _cache;
        private readonly IFileLoader _fileLoader;

        private readonly object _locker;

        public PreviewLoader(
            IFileLoader fileLoader,
            IPreviewCache previewCache)
        {
            _fileLoader = fileLoader;
            _cache = previewCache;

            _locker = new object();
        }

        public Preview GetPreview(
            TdApi.Photo photo,
            PreviewQuality quality)
        {
            var types = GetTypesByQuality(quality);

            var file = photo.Sizes
                .Where(s => Array.IndexOf(types, s.Type) >= 0)
                .OrderBy(s => Array.IndexOf(types, s.Type))
                .FirstOrDefault()?.Photo;

            return new Preview
            {
                Bitmap = GetBitmap(file),
                Quality = PreviewQuality.High
            };
        }

        public IObservable<Preview> LoadPreview(
            TdApi.Photo photo,
            PreviewQuality quality)
        {
            var types = GetTypesByQuality(quality);

            var file = photo.Sizes
                .Where(s => Array.IndexOf(types, s.Type) >= 0)
                .OrderBy(s => Array.IndexOf(types, s.Type))
                .FirstOrDefault()?.Photo;

            return LoadBitmap(file)
                .Select(bitmap => new Preview
                {
                    Bitmap = bitmap,
                    Quality = quality
                });
        }

        public Preview GetPreview(TdApi.PhotoSize photoSize)
        {
            var file = photoSize?.Photo;

            return new Preview
            {
                Bitmap = GetBitmap(file),
                Quality = PreviewQuality.High
            };
        }

        public IObservable<Preview> LoadPreview(TdApi.PhotoSize photoSize)
        {
            var file = photoSize?.Photo;

            return LoadBitmap(file)
                .Select(bitmap => new Preview
                {
                    Bitmap = bitmap,
                    Quality = PreviewQuality.High
                });
        }

        public Preview GetPreview(TdApi.Sticker sticker)
        {
            var file = sticker.Sticker_;

            return new Preview
            {
                Bitmap = GetBitmap(file),
                Quality = PreviewQuality.High
            };
        }

        public IObservable<Preview> LoadPreview(TdApi.Sticker sticker)
        {
            var file = sticker.Sticker_;

            return LoadBitmap(file)
                .Select(bitmap => new Preview
                {
                    Bitmap = bitmap,
                    Quality = PreviewQuality.High
                });
        }

        private IObservable<IBitmap> LoadBitmap(TdApi.File file)
        {
            return file != null
                ? _fileLoader.LoadFile(file, LoadPriority.Mid)
                    .FirstAsync(f => f.Local is {IsDownloadingCompleted: true})
                    .Select(f => GetBitmap(f.Local.Path))
                : Observable.Return<Bitmap>(null);
        }

        private IBitmap GetBitmap(TdApi.File file)
        {
            return file?.Local?.Path != null && _cache.TryGetValue(file.Local.Path, out var bitmap) ? (IBitmap)bitmap : null;
        }

        private Bitmap GetBitmap(string filePath)
        {
            lock (_locker)
            {
                Bitmap bitmap = null;

                if (_cache.TryGetValue(filePath, out var item))
                {
                    bitmap = (Bitmap)item;
                }
                else if (File.Exists(filePath))
                {
                    bitmap = new Bitmap(filePath);
                    _cache.Set(filePath, bitmap, new MemoryCacheEntryOptions
                    {
                        Size = 1
                    });
                }

                return bitmap;
            }
        }

        private string[] GetTypesByQuality(PreviewQuality quality)
        {
            // s m x y w
            // a b c d
            return quality switch
            {
                PreviewQuality.Low => LowQualitySizes,
                _ => HighQualitySizes
            };
        }
    }
}