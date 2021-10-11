using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using Tel.Egram.Model.Messenger.Explorer.Messages.Visual;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.Model.Messenger.Explorer.Messages
{
    public static class PreviewLoadingLogic
    {
        public static IDisposable BindPreviewLoading(
            this ReplyModel model)
        {
            return BindPreviewLoading(
                model,
                Locator.Current.GetService<IPreviewLoader>());
        }

        public static IDisposable BindPreviewLoading(
            this PhotoMessageModel model)
        {
            return BindPreviewLoading(
                model,
                Locator.Current.GetService<IPreviewLoader>());
        }

        public static IDisposable BindPreviewLoading(
            this VideoMessageModel model)
        {
            return BindPreviewLoading(
                model,
                Locator.Current.GetService<IPreviewLoader>());
        }

        public static IDisposable BindPreviewLoading(
            this StickerMessageModel model)
        {
            return BindPreviewLoading(
                model,
                Locator.Current.GetService<IPreviewLoader>());
        }

        public static IDisposable BindPreviewLoading(
            this ReplyModel model,
            IPreviewLoader previewLoader)
        {
            if (model.Preview == null)
            {
                model.Preview = GetPreview(previewLoader, model);

                if (model.Preview == null || model.Preview.Bitmap == null)
                {
                    return LoadPreview(previewLoader, model)
                        .SubscribeOn(RxApp.TaskpoolScheduler)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Accept(preview =>
                        {
                            model.Preview = preview;
                            model.HasPreview = true;
                        });
                }

                model.HasPreview = true;
            }

            return Disposable.Empty;
        }

        public static IDisposable BindPreviewLoading(
            this PhotoMessageModel model,
            IPreviewLoader previewLoader)
        {
            if (model.Preview == null)
            {
                model.Preview = GetPreview(previewLoader, model);

                if (model.Preview == null || model.Preview.Bitmap == null)
                {
                    return LoadPreview(previewLoader, model)
                        .SubscribeOn(RxApp.TaskpoolScheduler)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Accept(preview =>
                        {
                            model.Preview = preview;
                        });
                }
            }

            return Disposable.Empty;
        }

        public static IDisposable BindPreviewLoading(
            this VideoMessageModel model,
            IPreviewLoader previewLoader)
        {
            if (model.Preview == null)
            {
                model.Preview = GetPreview(previewLoader, model);

                if (model.Preview == null || model.Preview.Bitmap == null)
                {
                    return LoadPreview(previewLoader, model)
                        .SubscribeOn(RxApp.TaskpoolScheduler)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Accept(preview =>
                        {
                            model.Preview = preview;
                        });
                }
            }

            return Disposable.Empty;
        }

        public static IDisposable BindPreviewLoading(
            this StickerMessageModel model,
            IPreviewLoader previewLoader)
        {
            if (model.Preview == null)
            {
                model.Preview = GetPreview(previewLoader, model);

                if (model.Preview == null || model.Preview.Bitmap == null)
                {
                    return LoadPreview(previewLoader, model)
                        .SubscribeOn(RxApp.TaskpoolScheduler)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Accept(preview =>
                        {
                            model.Preview = preview;
                        });
                }
            }

            return Disposable.Empty;
        }

        private static Preview GetPreview(IPreviewLoader previewLoader, ReplyModel model)
        {
            if (model.PhotoData != null)
            {
                return previewLoader.GetPreview(model.PhotoData, PreviewQuality.Low);
            }

            if (model.VideoData?.Thumbnail != null)
            {
                return previewLoader.GetPreview(model.VideoData.Thumbnail);
            }

            return model.StickerData?.Thumbnail != null ? previewLoader.GetPreview(model.StickerData.Thumbnail) : null;
        }

        private static IObservable<Preview> LoadPreview(IPreviewLoader previewLoader, ReplyModel model)
        {
            if (model.PhotoData != null)
            {
                return previewLoader.LoadPreview(model.PhotoData, PreviewQuality.Low);
            }

            if (model.VideoData?.Thumbnail != null)
            {
                return previewLoader.LoadPreview(model.VideoData.Thumbnail);
            }

            return model.StickerData?.Thumbnail != null ? previewLoader.LoadPreview(model.StickerData.Thumbnail) : Observable.Empty<Preview>();
        }

        private static Preview GetPreview(IPreviewLoader previewLoader, PhotoMessageModel model)
        {
            return model.PhotoData != null ? previewLoader.GetPreview(model.PhotoData, PreviewQuality.High) : null;
        }

        private static IObservable<Preview> LoadPreview(IPreviewLoader previewLoader, PhotoMessageModel model)
        {
            return model.PhotoData != null
                ? previewLoader.LoadPreview(model.PhotoData, PreviewQuality.Low)
                    .Concat(previewLoader.LoadPreview(model.PhotoData, PreviewQuality.High))
                : Observable.Empty<Preview>();
        }

        private static Preview GetPreview(IPreviewLoader previewLoader, VideoMessageModel model)
        {
            return model.VideoData?.Thumbnail != null ? previewLoader.GetPreview(model.VideoData.Thumbnail) : null;
        }

        private static IObservable<Preview> LoadPreview(IPreviewLoader previewLoader, VideoMessageModel model)
        {
            return model.VideoData?.Thumbnail != null ? previewLoader.LoadPreview(model.VideoData.Thumbnail) : Observable.Empty<Preview>();
        }

        private static Preview GetPreview(IPreviewLoader previewLoader, StickerMessageModel model)
        {
            return model.StickerData?.Thumbnail != null ? previewLoader.GetPreview(model.StickerData.Thumbnail) : null;
        }

        private static IObservable<Preview> LoadPreview(IPreviewLoader previewLoader, StickerMessageModel model)
        {
            if (model.StickerData != null)
            {
                return model.StickerData?.Thumbnail != null
                    ? previewLoader.LoadPreview(model.StickerData.Thumbnail)
                        .Concat(previewLoader.LoadPreview(model.StickerData))
                    : previewLoader.LoadPreview(model.StickerData);
            }

            return Observable.Empty<Preview>();
        }

    }
}