using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

#nullable disable

namespace LibraryCGC.Components
{
    [ToolboxItem(true)]
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public class ArthanButton : UserControl
    {
        // Fields - same approach as ArthanPanel
        private Color _gradientStartColor = Color.FromArgb(70, 130, 180);
        private Color _gradientEndColor = Color.FromArgb(135, 206, 250);
        private LinearGradientMode _gradientDirection = LinearGradientMode.Vertical;
        private int _cornerRadius = 15;

        // Individual corner radius properties
        private int _topLeftRadius = 15;
        private int _topRightRadius = 15;
        private int _bottomLeftRadius = 15;
        private int _bottomRightRadius = 15;
        private bool _useIndividualCorners = false;

        private bool _enableDropShadow = false;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private int _shadowOffset = 5;
        private int _shadowBlur = 10;

        private int _borderSize = 0;
        private Color _borderColor = Color.White;

        // Button specific fields
        private string _text = "Button";
        private Font _font = new Font("Segoe UI", 9F);
        private Color _textColor = Color.White;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;

        // Mouse state tracking
        private bool _isMouseOver = false;
        private bool _isMouseDown = false;
        private Color _hoverStartColor = Color.Empty;
        private Color _hoverEndColor = Color.Empty;

        // Missing properties for designer compatibility
        private FlatStyle _flatStyle = FlatStyle.Standard;
        private bool _useVisualStyleBackColor = true;
        private Color _backgroundColor = Color.Empty;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleCenter;
        private Image _image = null;

        // FlatButtonAppearance inner class
        public class FlatButtonAppearance
        {
            private ArthanButton _parent;
            private Color _borderColor = Color.Empty;
            private int _borderSize = 0;
            private Color _mouseDownBackColor = Color.Empty;
            private Color _mouseOverBackColor = Color.Empty;

            internal FlatButtonAppearance(ArthanButton parent)
            {
                _parent = parent;
            }

            [Category("Appearance")]
            public Color BorderColor
            {
                get { return _borderColor; }
                set { _borderColor = value; if (_parent != null) _parent.Invalidate(); }
            }

            [Category("Appearance")]
            public int BorderSize
            {
                get { return _borderSize; }
                set { _borderSize = Math.Max(0, value); if (_parent != null) _parent.Invalidate(); }
            }

            [Category("Appearance")]
            public Color MouseDownBackColor
            {
                get { return _mouseDownBackColor; }
                set { _mouseDownBackColor = value; if (_parent != null) _parent.Invalidate(); }
            }

            [Category("Appearance")]
            public Color MouseOverBackColor
            {
                get { return _mouseOverBackColor; }
                set { _mouseOverBackColor = value; if (_parent != null) _parent.Invalidate(); }
            }
        }

        private FlatButtonAppearance _flatAppearance;

        public ArthanButton()
        {
            _flatAppearance = new FlatButtonAppearance(this);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // ArthanButton
            // 
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Name = "ArthanButton";
            Size = new Size(150, 40);
            Load += ArthanButton_Load;
            ResumeLayout(false);
        }

        private void UpdateHoverColors()
        {
            if (_hoverStartColor == Color.Empty)
                _hoverStartColor = ControlPaint.Light(_gradientStartColor, 0.2f);
            if (_hoverEndColor == Color.Empty)
                _hoverEndColor = ControlPaint.Light(_gradientEndColor, 0.2f);
        }

        private bool ShouldSerializeGradientStartColor()
        {
            return _gradientStartColor != Color.FromArgb(70, 130, 180);
        }

        private bool ShouldSerializeGradientEndColor()
        {
            return _gradientEndColor != Color.FromArgb(135, 206, 250);
        }

        private bool ShouldSerializeCornerRadius()
        {
            return _cornerRadius != 15;
        }

        private bool ShouldSerializeFont()
        {
            return !_font.Equals(new Font("Segoe UI", 9F));
        }

        private bool ShouldSerializeTextColor()
        {
            return _textColor != Color.White;
        }

        #region Properties

        [Category("Appearance")]
        [Description("Start color of the gradient background")]
        [DefaultValue(typeof(Color), "70, 130, 180")]
        public Color GradientStartColor
        {
            get { return _gradientStartColor; }
            set
            {
                if (_gradientStartColor != value)
                {
                    _gradientStartColor = value;
                    UpdateHoverColors();
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("End color of the gradient background")]
        [DefaultValue(typeof(Color), "135, 206, 250")]
        public Color GradientEndColor
        {
            get { return _gradientEndColor; }
            set
            {
                if (_gradientEndColor != value)
                {
                    _gradientEndColor = value;
                    UpdateHoverColors();
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Direction of the gradient")]
        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientDirection
        {
            get { return _gradientDirection; }
            set
            {
                if (_gradientDirection != value)
                {
                    _gradientDirection = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Radius of all corners (when UseIndividualCorners is false)")]
        [DefaultValue(15)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_cornerRadius != newValue)
                {
                    _cornerRadius = newValue;
                    if (!_useIndividualCorners)
                    {
                        _topLeftRadius = _topRightRadius = _bottomLeftRadius = _bottomRightRadius = _cornerRadius;
                    }
                    Invalidate();
                }
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Enable individual corner radius settings")]
        [DefaultValue(false)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseIndividualCorners
        {
            get { return _useIndividualCorners; }
            set
            {
                if (_useIndividualCorners != value)
                {
                    _useIndividualCorners = value;
                    if (!value)
                    {
                        _topLeftRadius = _topRightRadius = _bottomLeftRadius = _bottomRightRadius = _cornerRadius;
                    }
                    Invalidate();
                }
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the top-left corner")]
        [DefaultValue(15)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TopLeftRadius
        {
            get { return _topLeftRadius; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_topLeftRadius != newValue)
                {
                    _topLeftRadius = newValue;
                    if (_useIndividualCorners) Invalidate();
                }
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the top-right corner")]
        [DefaultValue(15)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TopRightRadius
        {
            get { return _topRightRadius; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_topRightRadius != newValue)
                {
                    _topRightRadius = newValue;
                    if (_useIndividualCorners) Invalidate();
                }
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the bottom-left corner")]
        [DefaultValue(15)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BottomLeftRadius
        {
            get { return _bottomLeftRadius; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_bottomLeftRadius != newValue)
                {
                    _bottomLeftRadius = newValue;
                    if (_useIndividualCorners) Invalidate();
                }
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the bottom-right corner")]
        [DefaultValue(15)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BottomRightRadius
        {
            get { return _bottomRightRadius; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_bottomRightRadius != newValue)
                {
                    _bottomRightRadius = newValue;
                    if (_useIndividualCorners) Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Enable or disable drop shadow")]
        [DefaultValue(false)]
        public bool EnableDropShadow
        {
            get { return _enableDropShadow; }
            set
            {
                if (_enableDropShadow != value)
                {
                    _enableDropShadow = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Color of the drop shadow")]
        [DefaultValue(typeof(Color), "50, 0, 0, 0")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                if (_shadowColor != value)
                {
                    _shadowColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Offset distance of the drop shadow")]
        [DefaultValue(5)]
        public int ShadowOffset
        {
            get { return _shadowOffset; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_shadowOffset != newValue)
                {
                    _shadowOffset = newValue;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Blur radius of the drop shadow")]
        [DefaultValue(10)]
        public int ShadowBlur
        {
            get { return _shadowBlur; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_shadowBlur != newValue)
                {
                    _shadowBlur = newValue;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Size of the border")]
        [DefaultValue(0)]
        public int BorderSize
        {
            get { return _borderSize; }
            set
            {
                int newValue = Math.Max(0, value);
                if (_borderSize != newValue)
                {
                    _borderSize = newValue;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Color of the border")]
        [DefaultValue(typeof(Color), "White")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Button text")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    _text = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Button font")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font
        {
            get { return _font; }
            set
            {
                if (_font != value && value != null)
                {
                    if (_font != null)
                    {
                        _font.Dispose();
                    }
                    _font = value;
                    base.Font = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Text color")]
        [DefaultValue(typeof(Color), "White")]
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Text alignment")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get { return _textAlign; }
            set
            {
                if (_textAlign != value)
                {
                    _textAlign = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Hover start color (leave empty for auto)")]
        public Color HoverStartColor
        {
            get { return _hoverStartColor; }
            set
            {
                if (_hoverStartColor != value)
                {
                    _hoverStartColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Hover end color (leave empty for auto)")]
        public Color HoverEndColor
        {
            get { return _hoverEndColor; }
            set
            {
                if (_hoverEndColor != value)
                {
                    _hoverEndColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Flat appearance of the button")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlatButtonAppearance FlatAppearance
        {
            get { return _flatAppearance; }
        }

        [Category("Appearance")]
        [Description("Flat style appearance of the button")]
        [DefaultValue(FlatStyle.Standard)]
        public FlatStyle FlatStyle
        {
            get { return _flatStyle; }
            set
            {
                if (_flatStyle != value)
                {
                    _flatStyle = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Use visual style back color")]
        [DefaultValue(true)]
        public bool UseVisualStyleBackColor
        {
            get { return _useVisualStyleBackColor; }
            set
            {
                if (_useVisualStyleBackColor != value)
                {
                    _useVisualStyleBackColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Background color of the button")]
        public new Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Layout")]
        [Description("Size of the button")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Size Size
        {
            get { return base.Size; }
            set
            {
                if (base.Size != value)
                {
                    base.Size = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Border radius of the button (same as CornerRadius)")]
        [DefaultValue(15)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderRadius
        {
            get { return _cornerRadius; }
            set { CornerRadius = value; }
        }

        [Category("Appearance")]
        [Description("Image alignment")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment ImageAlign
        {
            get { return _imageAlign; }
            set
            {
                if (_imageAlign != value)
                {
                    _imageAlign = value;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Image displayed on the button")]
        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    Invalidate();
                }
            }
        }

        #endregion

        private GraphicsPath CreateRoundedRectangleWithIndividualCorners(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            // Get the actual corner radii to use
            int tlRadius = _useIndividualCorners ? _topLeftRadius : _cornerRadius;
            int trRadius = _useIndividualCorners ? _topRightRadius : _cornerRadius;
            int blRadius = _useIndividualCorners ? _bottomLeftRadius : _cornerRadius;
            int brRadius = _useIndividualCorners ? _bottomRightRadius : _cornerRadius;

            // Ensure radii don't exceed bounds
            int maxRadius = Math.Min(bounds.Width / 2, bounds.Height / 2);
            tlRadius = Math.Min(tlRadius, maxRadius);
            trRadius = Math.Min(trRadius, maxRadius);
            blRadius = Math.Min(blRadius, maxRadius);
            brRadius = Math.Min(brRadius, maxRadius);

            // If all radii are 0, just add a rectangle
            if (tlRadius <= 0 && trRadius <= 0 && blRadius <= 0 && brRadius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Start from top-left corner
            int x = bounds.X;
            int y = bounds.Y;
            int width = bounds.Width;
            int height = bounds.Height;

            // Top-left corner
            if (tlRadius > 0)
            {
                path.AddArc(x, y, tlRadius * 2, tlRadius * 2, 180, 90);
                path.AddLine(x + tlRadius, y, x + width - trRadius, y);
            }
            else
            {
                path.AddLine(x, y, x + width - trRadius, y);
            }

            // Top-right corner
            if (trRadius > 0)
            {
                path.AddArc(x + width - trRadius * 2, y, trRadius * 2, trRadius * 2, 270, 90);
                path.AddLine(x + width, y + trRadius, x + width, y + height - brRadius);
            }
            else
            {
                path.AddLine(x + width, y, x + width, y + height - brRadius);
            }

            // Bottom-right corner
            if (brRadius > 0)
            {
                path.AddArc(x + width - brRadius * 2, y + height - brRadius * 2, brRadius * 2, brRadius * 2, 0, 90);
                path.AddLine(x + width - brRadius, y + height, x + blRadius, y + height);
            }
            else
            {
                path.AddLine(x + width, y + height, x + blRadius, y + height);
            }

            // Bottom-left corner
            if (blRadius > 0)
            {
                path.AddArc(x, y + height - blRadius * 2, blRadius * 2, blRadius * 2, 90, 90);
                path.AddLine(x, y + height - blRadius, x, y + tlRadius);
            }
            else
            {
                path.AddLine(x, y + height, x, y + tlRadius);
            }

            // Close the path if top-left has no radius
            if (tlRadius <= 0)
            {
                path.AddLine(x, y + tlRadius, x, y);
            }

            path.CloseFigure();
            return path;
        }

        private void DrawShadow(Graphics g, Rectangle bounds)
        {
            for (int i = _shadowBlur; i >= 1; i--)
            {
                int alpha = Math.Max(1, _shadowColor.A / _shadowBlur * (i / 2));
                Color shadowColor = Color.FromArgb(alpha, _shadowColor.R, _shadowColor.G, _shadowColor.B);

                Rectangle shadowBounds = new Rectangle(
                    bounds.X + _shadowOffset + (i - 1),
                    bounds.Y + _shadowOffset + (i - 1),
                    bounds.Width,
                    bounds.Height);

                using (GraphicsPath shadowPath = CreateRoundedRectangleWithIndividualCorners(shadowBounds))
                using (SolidBrush shadowBrush = new SolidBrush(shadowColor))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }
        }

        private StringFormat GetStringFormat(ContentAlignment alignment)
        {
            StringFormat sf = new StringFormat();

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
            }

            return sf;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Rectangle bounds = ClientRectangle;

            // Adjust bounds for shadow if enabled
            if (_enableDropShadow)
            {
                bounds.Width -= _shadowOffset + _shadowBlur;
                bounds.Height -= _shadowOffset + _shadowBlur;
            }

            // Draw shadow
            if (_enableDropShadow)
            {
                DrawShadow(g, bounds);
            }

            // Create rounded rectangle path with individual corners
            using (GraphicsPath path = CreateRoundedRectangleWithIndividualCorners(bounds))
            {
                // Choose colors based on state
                Color startColor = (_isMouseOver && !_isMouseDown) ? _hoverStartColor : _gradientStartColor;
                Color endColor = (_isMouseOver && !_isMouseDown) ? _hoverEndColor : _gradientEndColor;

                // Darken colors when pressed
                if (_isMouseDown)
                {
                    startColor = ControlPaint.Dark(startColor, 0.1f);
                    endColor = ControlPaint.Dark(endColor, 0.1f);
                }

                // Fill with gradient
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    bounds, startColor, endColor, _gradientDirection))
                {
                    g.FillPath(brush, path);
                }

                // Draw border if specified
                if (_borderSize > 0)
                {
                    using (Pen borderPen = new Pen(_borderColor, _borderSize))
                    {
                        g.DrawPath(borderPen, path);
                    }
                }
            }

            // Draw image if specified
            if (_image != null)
            {
                Rectangle imageBounds = GetImageBounds(bounds);
                g.DrawImage(_image, imageBounds);
            }

            // Draw text
            if (!string.IsNullOrEmpty(base.Text))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                using (StringFormat sf = GetStringFormat(_textAlign))
                using (SolidBrush textBrush = new SolidBrush(_textColor))
                {
                    Rectangle textBounds = bounds;
                    if (_borderSize > 0)
                    {
                        textBounds = new Rectangle(
                            bounds.X + _borderSize,
                            bounds.Y + _borderSize,
                            bounds.Width - _borderSize * 2,
                            bounds.Height - _borderSize * 2);
                    }

                    g.DrawString(base.Text, _font, textBrush, textBounds, sf);
                }
            }
        }

        private Rectangle GetImageBounds(Rectangle controlBounds)
        {
            if (_image == null) return Rectangle.Empty;

            int imageWidth = _image.Width;
            int imageHeight = _image.Height;
            Rectangle imageBounds = new Rectangle();

            switch (_imageAlign)
            {
                case ContentAlignment.TopLeft:
                    imageBounds = new Rectangle(controlBounds.X, controlBounds.Y, imageWidth, imageHeight);
                    break;
                case ContentAlignment.TopCenter:
                    imageBounds = new Rectangle(controlBounds.X + (controlBounds.Width - imageWidth) / 2, controlBounds.Y, imageWidth, imageHeight);
                    break;
                case ContentAlignment.TopRight:
                    imageBounds = new Rectangle(controlBounds.Right - imageWidth, controlBounds.Y, imageWidth, imageHeight);
                    break;
                case ContentAlignment.MiddleLeft:
                    imageBounds = new Rectangle(controlBounds.X, controlBounds.Y + (controlBounds.Height - imageHeight) / 2, imageWidth, imageHeight);
                    break;
                case ContentAlignment.MiddleCenter:
                    imageBounds = new Rectangle(controlBounds.X + (controlBounds.Width - imageWidth) / 2, controlBounds.Y + (controlBounds.Height - imageHeight) / 2, imageWidth, imageHeight);
                    break;
                case ContentAlignment.MiddleRight:
                    imageBounds = new Rectangle(controlBounds.Right - imageWidth, controlBounds.Y + (controlBounds.Height - imageHeight) / 2, imageWidth, imageHeight);
                    break;
                case ContentAlignment.BottomLeft:
                    imageBounds = new Rectangle(controlBounds.X, controlBounds.Bottom - imageHeight, imageWidth, imageHeight);
                    break;
                case ContentAlignment.BottomCenter:
                    imageBounds = new Rectangle(controlBounds.X + (controlBounds.Width - imageWidth) / 2, controlBounds.Bottom - imageHeight, imageWidth, imageHeight);
                    break;
                case ContentAlignment.BottomRight:
                    imageBounds = new Rectangle(controlBounds.Right - imageWidth, controlBounds.Bottom - imageHeight, imageWidth, imageHeight);
                    break;
            }

            return imageBounds;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            _text = base.Text;
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            _font = base.Font;
            Invalidate();
            base.OnFontChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isMouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseOver = false;
            _isMouseDown = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = true;
                this.Focus();
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = false;
                Invalidate();

                // Fire Click event if mouse is still over the button
                if (_isMouseOver)
                {
                    OnClick(EventArgs.Empty);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_font != null)
                {
                    _font.Dispose();
                    _font = null;
                }
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Helper Methods

        public void SetGradientTheme(string theme)
        {
            switch (theme.ToLower())
            {
                case "ocean":
                    GradientStartColor = Color.FromArgb(70, 130, 180);
                    GradientEndColor = Color.FromArgb(135, 206, 250);
                    break;
                case "sunset":
                    GradientStartColor = Color.FromArgb(255, 94, 77);
                    GradientEndColor = Color.FromArgb(255, 154, 0);
                    break;
                case "forest":
                    GradientStartColor = Color.FromArgb(76, 175, 80);
                    GradientEndColor = Color.FromArgb(129, 199, 132);
                    break;
                case "purple":
                    GradientStartColor = Color.FromArgb(156, 39, 176);
                    GradientEndColor = Color.FromArgb(186, 104, 200);
                    break;
                case "dark":
                    GradientStartColor = Color.FromArgb(45, 45, 45);
                    GradientEndColor = Color.FromArgb(80, 80, 80);
                    break;
                default:
                    GradientStartColor = Color.FromArgb(70, 130, 180);
                    GradientEndColor = Color.FromArgb(135, 206, 250);
                    break;
            }
        }

        public void SetCornerRadii(int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            _useIndividualCorners = true;
            _topLeftRadius = Math.Max(0, topLeft);
            _topRightRadius = Math.Max(0, topRight);
            _bottomRightRadius = Math.Max(0, bottomRight);
            _bottomLeftRadius = Math.Max(0, bottomLeft);
            Invalidate();
        }

        public void SetUniformCornerRadius(int radius)
        {
            _useIndividualCorners = false;
            CornerRadius = radius;
        }

        #endregion

        private void ArthanButton_Load(object sender, EventArgs e)
        {

        }
    }
}