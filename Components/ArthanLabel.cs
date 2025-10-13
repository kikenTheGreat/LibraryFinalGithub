using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;


namespace LibraryCGC.Components
{
    public class ArthanLabel : Control
    {
        // Private fields
        private string _text = "ArthanLabel";
        private Color _foreColor = Color.Black;
        private Color _outlineColor = Color.Black;
        private Color _shadowColor = Color.FromArgb(128, 0, 0, 0);
        private Font _font = new Font("Segoe UI", 9F);
        private ContentAlignment _textAlign = ContentAlignment.MiddleLeft;
        private bool _enableOutline = false;
        private bool _enableShadow = false;
        private bool _enableGradientText = false;
        private Color _gradientStartColor = Color.Blue;
        private Color _gradientEndColor = Color.Red;
        private LinearGradientMode _gradientDirection = LinearGradientMode.Horizontal;
        private int _outlineWidth = 1;
        private Point _shadowOffset = new Point(2, 2);
        private int _shadowBlur = 3;
        private TextRenderingHint _textRenderingHint = TextRenderingHint.ClearTypeGridFit;
        private bool _autoEllipsis = false;
        private int _lineSpacing = 0;
        private bool _wordWrap = false;

        public ArthanLabel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.Opaque, true);

            BackColor = Color.Transparent;
            Size = new Size(100, 23);
        }

        #region Properties

        [Category("Appearance")]
        [Description("The text displayed by the label")]
        public override string Text
        {
            get { return _text; }
            set { _text = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The foreground color of the text")]
        public override Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The font used to display text")]
        public override Font Font
        {
            get { return _font; }
            set { _font = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text alignment within the control")]
        public ContentAlignment TextAlign
        {
            get { return _textAlign; }
            set { _textAlign = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Enable text outline")]
        public bool EnableOutline
        {
            get { return _enableOutline; }
            set { _enableOutline = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color of the text outline")]
        public Color OutlineColor
        {
            get { return _outlineColor; }
            set { _outlineColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Width of the text outline")]
        public int OutlineWidth
        {
            get { return _outlineWidth; }
            set { _outlineWidth = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Enable text shadow")]
        public bool EnableShadow
        {
            get { return _enableShadow; }
            set { _enableShadow = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color of the text shadow")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Offset of the text shadow")]
        public Point ShadowOffset
        {
            get { return _shadowOffset; }
            set { _shadowOffset = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Blur amount of the text shadow")]
        public int ShadowBlur
        {
            get { return _shadowBlur; }
            set { _shadowBlur = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Enable gradient text coloring")]
        public bool EnableGradientText
        {
            get { return _enableGradientText; }
            set { _enableGradientText = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Start color for gradient text")]
        public Color GradientStartColor
        {
            get { return _gradientStartColor; }
            set { _gradientStartColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("End color for gradient text")]
        public Color GradientEndColor
        {
            get { return _gradientEndColor; }
            set { _gradientEndColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Direction of the gradient text")]
        public LinearGradientMode GradientDirection
        {
            get { return _gradientDirection; }
            set { _gradientDirection = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Text rendering quality")]
        public TextRenderingHint TextRenderingHint
        {
            get { return _textRenderingHint; }
            set { _textRenderingHint = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Automatically add ellipsis when text is too long")]
        public bool AutoEllipsis
        {
            get { return _autoEllipsis; }
            set { _autoEllipsis = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Additional spacing between lines")]
        public int LineSpacing
        {
            get { return _lineSpacing; }
            set { _lineSpacing = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Enable word wrapping")]
        public bool WordWrap
        {
            get { return _wordWrap; }
            set { _wordWrap = value; Invalidate(); }
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = _textRenderingHint;
            g.CompositingQuality = CompositingQuality.HighQuality;

            if (string.IsNullOrEmpty(_text))
                return;

            Rectangle textBounds = GetTextBounds();
            StringFormat stringFormat = GetStringFormat();

            // Draw shadow first (behind text)
            if (_enableShadow)
            {
                DrawTextShadow(g, textBounds, stringFormat);
            }

            // Draw outline (behind text)
            if (_enableOutline)
            {
                DrawTextOutline(g, textBounds, stringFormat);
            }

            // Draw main text
            DrawMainText(g, textBounds, stringFormat);

            stringFormat.Dispose();
        }

        private Rectangle GetTextBounds()
        {
            Rectangle bounds = ClientRectangle;

            // Adjust for shadow and outline
            if (_enableShadow)
            {
                bounds.Width -= Math.Abs(_shadowOffset.X) + _shadowBlur;
                bounds.Height -= Math.Abs(_shadowOffset.Y) + _shadowBlur;
            }

            if (_enableOutline)
            {
                bounds.Inflate(-_outlineWidth, -_outlineWidth);
            }

            return bounds;
        }

        private StringFormat GetStringFormat()
        {
            StringFormat format = new StringFormat();

            // Set alignment
            switch (_textAlign)
            {
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }

            // Set text wrapping and ellipsis
            if (_wordWrap)
            {
                format.FormatFlags &= ~StringFormatFlags.NoWrap;
            }
            else
            {
                format.FormatFlags |= StringFormatFlags.NoWrap;
            }

            if (_autoEllipsis)
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
            }

            return format;
        }

        private void DrawTextShadow(Graphics g, Rectangle bounds, StringFormat format)
        {
            // Create multiple shadow layers for blur effect
            for (int i = _shadowBlur; i >= 1; i--)
            {
                int alpha = Math.Max(10, _shadowColor.A / _shadowBlur * i);
                Color blurredShadowColor = Color.FromArgb(alpha, _shadowColor.R, _shadowColor.G, _shadowColor.B);

                Rectangle shadowBounds = new Rectangle(
                    bounds.X + _shadowOffset.X + (i - 1),
                    bounds.Y + _shadowOffset.Y + (i - 1),
                    bounds.Width,
                    bounds.Height);

                using (SolidBrush shadowBrush = new SolidBrush(blurredShadowColor))
                {
                    if (_lineSpacing > 0)
                    {
                        DrawTextWithLineSpacing(g, _text, _font, shadowBrush, shadowBounds, format);
                    }
                    else
                    {
                        g.DrawString(_text, _font, shadowBrush, shadowBounds, format);
                    }
                }
            }
        }

        private void DrawTextOutline(Graphics g, Rectangle bounds, StringFormat format)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                // Add text to path for outline
                path.AddString(_text, _font.FontFamily, (int)_font.Style,
                    g.DpiY * _font.SizeInPoints / 72, bounds, format);

                using (Pen outlinePen = new Pen(_outlineColor, _outlineWidth))
                {
                    outlinePen.LineJoin = LineJoin.Round;
                    g.DrawPath(outlinePen, path);
                }
            }
        }

        private void DrawMainText(Graphics g, Rectangle bounds, StringFormat format)
        {
            Brush textBrush;

            if (_enableGradientText)
            {
                textBrush = new LinearGradientBrush(bounds, _gradientStartColor, _gradientEndColor, _gradientDirection);
            }
            else
            {
                textBrush = new SolidBrush(_foreColor);
            }

            using (textBrush)
            {
                if (_lineSpacing > 0)
                {
                    DrawTextWithLineSpacing(g, _text, _font, textBrush, bounds, format);
                }
                else
                {
                    g.DrawString(_text, _font, textBrush, bounds, format);
                }
            }
        }

        private void DrawTextWithLineSpacing(Graphics g, string text, Font font, Brush brush, Rectangle bounds, StringFormat format)
        {
            string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            float lineHeight = font.GetHeight(g) + _lineSpacing;
            float totalHeight = lines.Length * lineHeight - _lineSpacing;

            float startY = bounds.Y;
            if (format.LineAlignment == StringAlignment.Center)
                startY = bounds.Y + (bounds.Height - totalHeight) / 2;
            else if (format.LineAlignment == StringAlignment.Far)
                startY = bounds.Y + bounds.Height - totalHeight;

            for (int i = 0; i < lines.Length; i++)
            {
                Rectangle lineRect = new Rectangle(bounds.X, (int)(startY + i * lineHeight), bounds.Width, (int)lineHeight);
                StringFormat lineFormat = (StringFormat)format.Clone();
                lineFormat.LineAlignment = StringAlignment.Near;

                g.DrawString(lines[i], font, brush, lineRect, lineFormat);
                lineFormat.Dispose();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Set a predefined text style theme
        /// </summary>
        /// <param name="style">Style name</param>
        public void SetTextStyle(string style)
        {
            switch (style.ToLower())
            {
                case "title":
                    Font = new Font("Segoe UI", 16, FontStyle.Bold);
                    ForeColor = Color.FromArgb(33, 37, 41);
                    EnableShadow = false;
                    EnableOutline = false;
                    break;
                case "subtitle":
                    Font = new Font("Segoe UI", 12, FontStyle.Regular);
                    ForeColor = Color.FromArgb(108, 117, 125);
                    EnableShadow = false;
                    EnableOutline = false;
                    break;
                case "caption":
                    Font = new Font("Segoe UI", 8, FontStyle.Italic);
                    ForeColor = Color.FromArgb(134, 142, 150);
                    EnableShadow = false;
                    EnableOutline = false;
                    break;
                case "highlight":
                    Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    EnableGradientText = true;
                    GradientStartColor = Color.FromArgb(255, 193, 7);
                    GradientEndColor = Color.FromArgb(255, 87, 34);
                    break;
                case "glow":
                    Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    ForeColor = Color.White;
                    EnableShadow = true;
                    ShadowColor = Color.FromArgb(200, 0, 123, 255);
                    ShadowBlur = 8;
                    ShadowOffset = new Point(0, 0);
                    break;
                case "outline":
                    Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    ForeColor = Color.White;
                    EnableOutline = true;
                    OutlineColor = Color.Black;
                    OutlineWidth = 2;
                    break;
                case "retro":
                    Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    EnableGradientText = true;
                    GradientStartColor = Color.FromArgb(255, 20, 147);
                    GradientEndColor = Color.FromArgb(0, 191, 255);
                    GradientDirection = LinearGradientMode.ForwardDiagonal;
                    EnableShadow = true;
                    ShadowColor = Color.FromArgb(150, 255, 20, 147);
                    break;
                default:
                    // Default style
                    Font = new Font("Segoe UI", 9, FontStyle.Regular);
                    ForeColor = Color.Black;
                    EnableShadow = false;
                    EnableOutline = false;
                    EnableGradientText = false;
                    break;
            }
        }

        /// <summary>
        /// Set transparent background with specific opacity
        /// </summary>
        /// <param name="opacity">Opacity value (0-255)</param>
        public void SetTransparency(int opacity)
        {
            opacity = Math.Max(0, Math.Min(255, opacity));
            BackColor = Color.FromArgb(opacity, BackColor.R, BackColor.G, BackColor.B);
        }

        /// <summary>
        /// Animate text color change
        /// </summary>
        /// <param name="targetColor">Target color</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public void AnimateColorTo(Color targetColor, int duration = 300)
        {
            System.Windows.Forms.Timer colorTimer = new System.Windows.Forms.Timer();
            DateTime startTime = DateTime.Now;
            Color startColor = _foreColor;

            colorTimer.Interval = 16; // ~60 FPS
            colorTimer.Tick += (s, e) =>
            {
                double elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                double progress = Math.Min(elapsed / duration, 1.0);

                // Easing function
                progress = 1 - Math.Pow(1 - progress, 3);

                int r = (int)(startColor.R + (targetColor.R - startColor.R) * progress);
                int g = (int)(startColor.G + (targetColor.G - startColor.G) * progress);
                int b = (int)(startColor.B + (targetColor.B - startColor.B) * progress);

                ForeColor = Color.FromArgb(r, g, b);

                if (progress >= 1.0)
                {
                    colorTimer.Stop();
                    colorTimer.Dispose();
                }
            };

            colorTimer.Start();
        }

        /// <summary>
        /// Create a typewriter effect animation
        /// </summary>
        /// <param name="fullText">Complete text to display</param>
        /// <param name="speed">Characters per second</param>
        public void TypewriterEffect(string fullText, int speed = 10)
        {
            Text = "";
            int charIndex = 0;

            System.Windows.Forms.Timer typeTimer = new System.Windows.Forms.Timer();
            typeTimer.Interval = 1000 / speed;

            typeTimer.Tick += (s, e) =>
            {
                if (charIndex < fullText.Length)
                {
                    Text = fullText.Substring(0, charIndex + 1);
                    charIndex++;
                }
                else
                {
                    typeTimer.Stop();
                    typeTimer.Dispose();
                }
            };

            typeTimer.Start();
        }

        #endregion

        #region Override Methods

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _font?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    // Example usage form
    public partial class LabelExampleForm : Form
    {
        public LabelExampleForm()
        {
            InitializeComponent();
            CreateExampleLabels();
        }

        private void CreateExampleLabels()
        {
            // Title label
            ArthanLabel titleLabel = new ArthanLabel()
            {
                Text = "Custom Label Examples",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            titleLabel.SetTextStyle("title");
            Controls.Add(titleLabel);

            // Gradient text label
            ArthanLabel gradientLabel = new ArthanLabel()
            {
                Text = "Gradient Text Effect",
                Location = new Point(20, 60),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            gradientLabel.SetTextStyle("highlight");
            Controls.Add(gradientLabel);

            // Glow effect label
            ArthanLabel glowLabel = new ArthanLabel()
            {
                Text = "Glowing Text",
                Location = new Point(20, 100),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            glowLabel.SetTextStyle("glow");
            Controls.Add(glowLabel);

            // Outline text label
            ArthanLabel outlineLabel = new ArthanLabel()
            {
                Text = "Outlined Text",
                Location = new Point(20, 140),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            outlineLabel.SetTextStyle("outline");
            Controls.Add(outlineLabel);

            // Multi-line with spacing
            ArthanLabel multilineLabel = new ArthanLabel()
            {
                Text = "Line 1\nLine 2\nLine 3",
                Location = new Point(20, 180),
                Size = new Size(150, 80),
                TextAlign = ContentAlignment.TopLeft,
                LineSpacing = 5,
                WordWrap = true
            };
            multilineLabel.SetTextStyle("subtitle");
            Controls.Add(multilineLabel);

            // Transparent background demo
            ArthanLabel transparentLabel = new ArthanLabel()
            {
                Text = "Fully Transparent Background",
                Location = new Point(20, 280),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkBlue,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            transparentLabel.SetTransparency(0); // Fully transparent background
            Controls.Add(transparentLabel);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 350);
            Text = "Custom Label Demo";
            BackColor = Color.FromArgb(245, 245, 245);

            ResumeLayout(false);
        }
    }
}

