using Avalonia.Media.Imaging;
using PropertyChanged;

namespace Tel.Egram.Model.Authentication.Phone
{
    [AddINotifyPropertyChangedInterface]
    public class PhoneCodeModel
    {
        public string Code { get; set; }

        public string CountryCode { get; set; }

        public IBitmap Flag { get; set; }

        public string Mask { get; set; }
    }
}