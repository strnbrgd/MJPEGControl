using System.Globalization;
using CommunityToolkit.Maui.Converters;


namespace MJPEGControl
{
    /// <summary>
    /// View that shows MJPEG stream
    /// Uses Maui Toolkit - must add UseMauiCommunityToolkit
    /// </summary>
    public class MJPEGImage : Image
    {
        private readonly MjpegProcessor.MjpegDecoder Decoder = new();

        #region MJPEGSource Property

        public static readonly BindableProperty MJPEGSourceProperty = BindableProperty.Create("MJPEGSource",
            typeof(string),
            typeof(MJPEGImage),propertyChanged:MJPEGSourceChanged);

        public string MJPEGSource
        {
            get => (string) GetValue(MJPEGSourceProperty);
            set => SetValue(MJPEGSourceProperty, value);
        }

        

        private static void MJPEGSourceChanged(BindableObject Bindable, object OldValue, object NewValue)
        {
            if (!(Bindable is MJPEGImage image)) return;
            var newStreamUri = NewValue as string;
            if (string.IsNullOrEmpty(newStreamUri))
                image.Decoder.StopStream();
            else
                image.Decoder.ParseStream(new Uri(newStreamUri));
        }

        #endregion

        private readonly ByteArrayToImageSourceConverter m_converter = new();

        public delegate void StringDelegate(string ErrorMessage);

        public event StringDelegate Error;

        public MJPEGImage()
        {
            Decoder.FrameReady += (s, e) => { Source = m_converter.ConvertFrom(e.FrameBuffer, CultureInfo.CurrentCulture);};
            Decoder.Error += (s,e)=> {Error?.Invoke(e.Message);};
        }

        
    }
}