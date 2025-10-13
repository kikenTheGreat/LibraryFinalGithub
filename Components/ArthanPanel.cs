using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibraryCGC.Components
{
    public class ArthanPanel : Panel
    {

        private bool _isDragging = false;
        private Point _lastCursor;
        private Point _lastForm;

        // Custom properties
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

        private bool _enableDropShadow = true;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private int _shadowOffset = 5;
        private int _shadowBlur = 10;
        private bool _enableDragging = true;

        public ArthanPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
        }

        private void InitializeComponent()
        {
            // Event handlers for dragging
            MouseDown += DraggablePanel_MouseDown;
            MouseMove += DraggablePanel_MouseMove;
            MouseUp += DraggablePanel_MouseUp;

            // Change cursor when hovering
            MouseEnter += (s, e) => { if (_enableDragging) Cursor = Cursors.SizeAll; };
            MouseLeave += (s, e) => { Cursor = Cursors.Default; };
        }

        #region Custom Properties

        [Category("Appearance")]
        [Description("Start color of the gradient background")]
        public Color GradientStartColor
        {
            get { return _gradientStartColor; }
            set { _gradientStartColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("End color of the gradient background")]
        public Color GradientEndColor
        {
            get { return _gradientEndColor; }
            set { _gradientEndColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Direction of the gradient")]
        public LinearGradientMode GradientDirection
        {
            get { return _gradientDirection; }
            set { _gradientDirection = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Radius of all corners (when UseIndividualCorners is false)")]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = Math.Max(0, value);
                if (!_useIndividualCorners)
                {
                    _topLeftRadius = _topRightRadius = _bottomLeftRadius = _bottomRightRadius = _cornerRadius;
                }
                Invalidate();
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Enable individual corner radius settings")]
        public bool UseIndividualCorners
        {
            get { return _useIndividualCorners; }
            set
            {
                _useIndividualCorners = value;
                if (!value)
                {
                    // Sync all corners to the main CornerRadius value
                    _topLeftRadius = _topRightRadius = _bottomLeftRadius = _bottomRightRadius = _cornerRadius;
                }
                Invalidate();
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the top-left corner")]
        public int TopLeftRadius
        {
            get { return _topLeftRadius; }
            set
            {
                _topLeftRadius = Math.Max(0, value);
                if (_useIndividualCorners) Invalidate();
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the top-right corner")]
        public int TopRightRadius
        {
            get { return _topRightRadius; }
            set
            {
                _topRightRadius = Math.Max(0, value);
                if (_useIndividualCorners) Invalidate();
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the bottom-left corner")]
        public int BottomLeftRadius
        {
            get { return _bottomLeftRadius; }
            set
            {
                _bottomLeftRadius = Math.Max(0, value);
                if (_useIndividualCorners) Invalidate();
            }
        }

        [Category("Appearance - Individual Corners")]
        [Description("Radius of the bottom-right corner")]
        public int BottomRightRadius
        {
            get { return _bottomRightRadius; }
            set
            {
                _bottomRightRadius = Math.Max(0, value);
                if (_useIndividualCorners) Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Enable or disable drop shadow")]
        public bool EnableDropShadow
        {
            get { return _enableDropShadow; }
            set { _enableDropShadow = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color of the drop shadow")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Offset distance of the drop shadow")]
        public int ShadowOffset
        {
            get { return _shadowOffset; }
            set { _shadowOffset = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Blur radius of the drop shadow")]
        public int ShadowBlur
        {
            get { return _shadowBlur; }
            set { _shadowBlur = Math.Max(0, value); Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Enable or disable dragging functionality")]
        public bool EnableDragging
        {
            get { return _enableDragging; }
            set { _enableDragging = value; }
        }

        #endregion

        #region Dragging Logic

        private void DraggablePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_enableDragging || e.Button != MouseButtons.Left) return;

            _isDragging = true;
            _lastCursor = Cursor.Position;
            _lastForm = Location;
        }

        private void DraggablePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !_enableDragging) return;

            Point currentCursor = Cursor.Position;
            Point offset = new Point(currentCursor.X - _lastCursor.X, currentCursor.Y - _lastCursor.Y);
            Location = new Point(_lastForm.X + offset.X, _lastForm.Y + offset.Y);
        }

        private void DraggablePanel_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        #endregion

        #region Custom Painting

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
                // Fill with gradient
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    bounds, _gradientStartColor, _gradientEndColor, _gradientDirection))
                {
                    g.FillPath(brush, path);
                }

                // Optional: Add a subtle border
                using (Pen borderPen = new Pen(Color.FromArgb(100, Color.White), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Don't call base.OnPaint to avoid default panel painting
        }

        private void DrawShadow(Graphics g, Rectangle bounds)
        {
            // Create multiple shadow layers for blur effect
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

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Additional Methods

        /// <summary>
        /// Set a predefined gradient theme
        /// </summary>
        /// <param name="theme">Theme name</param>
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
                    // Default ocean theme
                    GradientStartColor = Color.FromArgb(70, 130, 180);
                    GradientEndColor = Color.FromArgb(135, 206, 250);
                    break;
            }
        }

        /// <summary>
        /// Set individual corner radii in one call
        /// </summary>
        /// <param name="topLeft">Top-left corner radius</param>
        /// <param name="topRight">Top-right corner radius</param>
        /// <param name="bottomRight">Bottom-right corner radius</param>
        /// <param name="bottomLeft">Bottom-left corner radius</param>
        public void SetCornerRadii(int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            _useIndividualCorners = true;
            _topLeftRadius = Math.Max(0, topLeft);
            _topRightRadius = Math.Max(0, topRight);
            _bottomRightRadius = Math.Max(0, bottomRight);
            _bottomLeftRadius = Math.Max(0, bottomLeft);
            Invalidate();
        }

        /// <summary>
        /// Reset all corners to the same radius
        /// </summary>
        /// <param name="radius">Radius for all corners</param>
        public void SetUniformCornerRadius(int radius)
        {
            _useIndividualCorners = false;
            CornerRadius = radius;
        }

        /// <summary>
        /// Animate the panel to a new location
        /// </summary>
        /// <param name="targetLocation">Target location</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public void AnimateToLocation(Point targetLocation, int duration = 300)
        {
            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            DateTime startTime = DateTime.Now;
            Point startLocation = Location;

            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += (s, e) =>
            {
                double elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                double progress = Math.Min(elapsed / duration, 1.0);

                // Easing function (ease out)
                progress = 1 - Math.Pow(1 - progress, 3);

                int newX = (int)(startLocation.X + (targetLocation.X - startLocation.X) * progress);
                int newY = (int)(startLocation.Y + (targetLocation.Y - startLocation.Y) * progress);

                Location = new Point(newX, newY);

                if (progress >= 1.0)
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();
                }
            };

            animationTimer.Start();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up event handlers
                MouseDown -= DraggablePanel_MouseDown;
                MouseMove -= DraggablePanel_MouseMove;
                MouseUp -= DraggablePanel_MouseUp;
            }
            base.Dispose(disposing);
        }

        internal void Load(string thumbnail)
        {
            throw new NotImplementedException();
        }
    }

    // Example usage class
    public partial class ExampleForm : Form
    {
        public ExampleForm()
        {
            InitializeComponent();
            CreateExamplePanels();
        }

        private void CreateExamplePanels()
        {
            // Example 1: Ocean theme panel with uniform corners
            ArthanPanel oceanPanel = new ArthanPanel()
            {
                Size = new Size(200, 150),
                Location = new Point(50, 50),
                CornerRadius = 20,
                EnableDropShadow = true,
                ShadowOffset = 8,
                ShadowBlur = 15
            };
            oceanPanel.SetGradientTheme("ocean");

            Label oceanLabel = new Label()
            {
                Text = "Ocean Theme\nUniform Corners",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            oceanPanel.Controls.Add(oceanLabel);
            Controls.Add(oceanPanel);

            // Example 2: Sunset theme panel with individual corners
            ArthanPanel sunsetPanel = new ArthanPanel()
            {
                Size = new Size(180, 120),
                Location = new Point(300, 80),
                EnableDropShadow = true,
                GradientDirection = LinearGradientMode.Horizontal
            };
            sunsetPanel.SetGradientTheme("sunset");
            // Set individual corner radii: top corners rounded, bottom corners sharp
            sunsetPanel.SetCornerRadii(25, 25, 0, 0);

            Label sunsetLabel = new Label()
            {
                Text = "Sunset Theme\nTop Rounded Only",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            sunsetPanel.Controls.Add(sunsetLabel);
            Controls.Add(sunsetPanel);

            // Example 3: Custom gradient panel with asymmetric corners
            ArthanPanel customPanel = new ArthanPanel()
            {
                Size = new Size(160, 160),
                Location = new Point(150, 250),
                GradientStartColor = Color.FromArgb(255, 0, 150),
                GradientEndColor = Color.FromArgb(0, 204, 255),
                GradientDirection = LinearGradientMode.ForwardDiagonal,
                EnableDropShadow = true,
                ShadowColor = Color.FromArgb(80, 255, 0, 150),
                ShadowOffset = 6,
                ShadowBlur = 12
            };
            // Set individual corner radii: asymmetric design
            customPanel.SetCornerRadii(5, 30, 5, 30);

            Label customLabel = new Label()
            {
                Text = "Custom\nAsymmetric\nCorners",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            customPanel.Controls.Add(customLabel);
            Controls.Add(customPanel);

            // Example 4: Forest theme with mixed corners
            ArthanPanel forestPanel = new ArthanPanel()
            {
                Size = new Size(140, 100),
                Location = new Point(400, 280),
                EnableDropShadow = true,
                ShadowOffset = 4,
                ShadowBlur = 8
            };
            forestPanel.SetGradientTheme("forest");
            // Set individual corner radii: left side rounded, right side sharp
            forestPanel.SetCornerRadii(20, 0, 0, 20);

            Label forestLabel = new Label()
            {
                Text = "Forest Theme\nLeft Rounded",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            forestPanel.Controls.Add(forestLabel);
            Controls.Add(forestPanel);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 450);
            Text = "Enhanced Draggable Panel Demo - Individual Corners";
            BackColor = Color.FromArgb(240, 240, 240);

            ResumeLayout(false);
        }
    }
}