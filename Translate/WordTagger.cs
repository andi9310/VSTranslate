using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Operations;

namespace VSTranslate
{

    internal sealed class WordTagger
    {
        private ITextStructureNavigator TextStructureNavigator { get; }
        private readonly glosbeClient.GlosbeClient _client = new glosbeClient.GlosbeClient();

        private readonly IAdornmentLayer _layer;

        private readonly IWpfTextView _view;

        private readonly Brush _brush;

        private readonly Pen _pen;

        public WordTagger(IWpfTextView view, ITextStructureNavigator textStructureNavigator)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }
            TextStructureNavigator = textStructureNavigator;

            _layer = view.GetAdornmentLayer("WordTagger");

            _view = view;
            _view.LayoutChanged += OnLayoutChanged;

            _brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            _brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Blue);
            penBrush.Freeze();
            _pen = new Pen(penBrush, 0.5);
            _pen.Freeze();
        }
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var line in e.NewOrReformattedLines)
            {
                CreateVisuals(line);
            }
        }

        private void CreateVisuals(ITextViewLine line)
        {
            var textViewLines = _view.TextViewLines;

            for (var charIndex = line.Start; charIndex < line.End;)
            {
                var word = TextStructureNavigator.GetExtentOfWord(charIndex);

                if (WordExtentIsValid(charIndex, word))
                { 
                    charIndex = charIndex.Add(word.Span.Length);
                    var trans = _client.GetTranslations(word.Span.GetText().ToLower());
                    if (trans.Any()) continue;
                    var geometry = textViewLines.GetMarkerGeometry(word.Span);
                    var drawing = new GeometryDrawing(_brush, _pen, geometry);
                    drawing.Freeze();

                    var drawingImage = new DrawingImage(drawing);
                    drawingImage.Freeze();

                    var image = new Image
                    {
                        Source = drawingImage,
                    };

                    Canvas.SetLeft(image, geometry.Bounds.Left);
                    Canvas.SetTop(image, geometry.Bounds.Top);

                    _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, word.Span, null, image, null);
                }
                else
                {
                    charIndex = charIndex.Add(1);
                }
            }

        }

        private static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word)
        {
            return word.IsSignificant && currentRequest.Snapshot.GetText(word.Span).Any(char.IsLetter);
        }
    }
}
