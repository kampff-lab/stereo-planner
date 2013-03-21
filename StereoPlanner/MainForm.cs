using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using StereoPlanner.Model;
using System.Xml;
using System.Xml.Serialization;
using StereoPlanner.Graphics;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;

namespace StereoPlanner
{
    public partial class MainForm : Form
    {
        const float DefaultMarkerSize = 3;
        const float SelectionThreshold = 10;
        const float DepthWindowWidth = 7.5f;
        const float DepthWindowHeight = 5;
        static readonly Vector2 DepthWindowPosition = new Vector2(2.5f, 9);

        StereotacticProtocol protocol;
        int version;
        int saveVersion;

        bool loaded;
        SpriteFont font;
        Camera2D camera;
        SpriteBatch spriteBatch;
        StereotacticPoint selectedPoint;

        public MainForm()
        {
            InitializeComponent();
            protocol = new StereotacticProtocol();
            propertyGrid.SelectedObject = protocol;
        }

        bool CheckUnsavedChanges()
        {
            if (protocol != null && saveVersion != version)
            {
                var result = MessageBox.Show("Protocol has unsaved changes. Save protocol file?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.OK)
                {
                    saveToolStripMenuItem_Click(this, EventArgs.Empty);
                }
                else return result == DialogResult.No;
            }

            return true;
        }

        void OpenProject(string fileName)
        {
            saveProjectDialog.FileName = null;
            using (var reader = XmlReader.Create(fileName))
            {
                var serializer = new XmlSerializer(typeof(StereotacticProtocol));
                try { protocol = (StereotacticProtocol)serializer.Deserialize(reader); }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Unrecognized file format. Please check if project file is compatible with this version of StereoPlanner.", "Invalid file format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            propertyGrid.SelectedObject = protocol;
        }

        Vector2 PickModelPoint()
        {
            var viewportPosition = glControl.PointToClient(Form.MousePosition);
            var position = new Vector3(
                viewportPosition.X - glControl.Width * 0.5f,
                glControl.Height * 0.5f - viewportPosition.Y, 0);
            position /= spriteBatch.PixelsPerMeter;

            var view = camera.GetViewMatrix();
            view.Invert();
            Vector3.Transform(ref position, ref view, out position);

            return new Vector2(position.X, position.Y);
        }

        StereotacticPoint ClosestPoint(Vector2 measurement, StereotacticPoint reference)
        {
            var closest = (from point in protocol.Points
                           where !string.IsNullOrEmpty(point.Name)
                           let renderPosition = Utilities.GetRenderPosition(point, reference)
                           let cursorDistance = (renderPosition - measurement).LengthSquared
                           where cursorDistance < SelectionThreshold
                           select new { point, cursorDistance })
                           .ArgMin(xs => xs.cursorDistance);
            return closest != null ? closest.point : null;
        }

        StereotacticPoint GetReferencePoint()
        {
            StereotacticPoint reference = null;
            if (!string.IsNullOrEmpty(protocol.ReferencePoint) && protocol.Points.Contains(protocol.ReferencePoint))
            {
                reference = protocol.Points[protocol.ReferencePoint];
            }

            return reference;
        }

        void RenderModel()
        {
            GL.PointSize(DefaultMarkerSize);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            spriteBatch.Begin(camera.GetViewMatrix());

            var reference = GetReferencePoint();
            spriteBatch.DrawVertices(from point in protocol.Points.Do(xs => GL.Color3(xs.Color))
                                     select Utilities.GetRenderPosition(point, reference),
                                     BeginMode.Points,
                                     Color4.White);

            var cursor = PickModelPoint();
            var closest = (from point in protocol.Points
                           where !string.IsNullOrEmpty(point.Name)
                           let position = Utilities.GetPointPosition(point, reference)
                           let renderPosition = Utilities.GetRenderPosition(point, reference)
                           let cursorDistance = (renderPosition - cursor).LengthSquared
                           where cursorDistance < SelectionThreshold
                           select new { point, position, renderPosition, cursorDistance })
                           .ArgMin(xs => xs.cursorDistance);

            if (closest != null)
            {
                var text = string.Format(
                    "{0:f2}\nAP:{1:f2} ({2:+0.00;-0.00})\nML:{3:f2} ({4:+0.00;-0.00})",
                    closest.point.Name,
                    closest.position.Y,
                    closest.point.Name == protocol.ReferencePoint ? 0 : closest.point.AnteriorPosterior,
                    closest.position.X,
                    closest.point.Name == protocol.ReferencePoint ? 0 : closest.point.MedioLateral);
                spriteBatch.DrawString(font, text, closest.renderPosition, 0, Vector2.One, closest.point.Color);
            }

            foreach (var shape in protocol.Shapes)
            {
                shape.Draw(spriteBatch, protocol.Points);
            }

            if (selectedPoint != null)
            {
                spriteBatch.DrawVertices(new[]
                {
                    DepthWindowPosition,
                    DepthWindowPosition + DepthWindowWidth * Vector2.UnitX,
                    DepthWindowPosition + new Vector2(DepthWindowWidth, -DepthWindowHeight),
                    DepthWindowPosition - DepthWindowHeight * Vector2.UnitY,
                    DepthWindowPosition
                }, BeginMode.LineStrip, Color.White);

                var depthWindowLabel = string.Format("{0}\nDepth View", selectedPoint.Name);
                spriteBatch.DrawString(font, depthWindowLabel, DepthWindowPosition, 0, Vector2.One, Color.White);

                var targetPositions = from target in selectedPoint.DorsoVentralTargets
                                      let depth = target.DorsoVentral + selectedPoint.DorsoVentralReference
                                      let position = new Vector2(DepthWindowPosition.X + DepthWindowWidth * 0.1f, DepthWindowPosition.Y - DepthWindowHeight * 0.2f + (float)target.DorsoVentral)
                                      select new { name = target.Name, depth, position, target.DorsoVentral };
                spriteBatch.DrawVertices(targetPositions.Select(ts => ts.position), BeginMode.Points, Color.Red);

                var closestDepth = (from target in targetPositions
                                    let cursorDistance = (target.position - cursor).LengthSquared
                                    where cursorDistance < SelectionThreshold
                                    select new { target, cursorDistance })
                                    .ArgMin(xs => xs.cursorDistance);

                if (closestDepth != null)
                {
                    var text = string.Format("{0:f2}\nDV:{1:f2} ({2:+0.00;-0.00})", closestDepth.target.name, closestDepth.target.depth, closestDepth.target.DorsoVentral);
                    spriteBatch.DrawString(font, text, closestDepth.target.position, 0, Vector2.One, Color.Red);
                }
            }

            spriteBatch.End();
            glControl.SwapBuffers();
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            glControl.VSync = true;
            GL.ClearColor(Color.Black);

            camera = new Camera2D();
            spriteBatch = new SpriteBatch(glControl.Width, glControl.Height);
            spriteBatch.PixelsPerMeter = 20;

            var fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StereoPlanner.Resources.LiberationMono-Regular.spf");
            font = SpriteFont.FromStream(fontStream);

            Application.Idle += new EventHandler(Application_Idle);
            loaded = true;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl.IsIdle)
            {
                RenderModel();
            }
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;

            spriteBatch.SetDimensions(glControl.Width, glControl.Height);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;
            RenderModel();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckUnsavedChanges()) return;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckUnsavedChanges()) return;

            if (openProjectDialog.ShowDialog() == DialogResult.OK)
            {
                OpenProject(openProjectDialog.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveProjectDialog.FileName)) saveAsToolStripMenuItem_Click(this, e);
            else
            {
                using (var writer = XmlWriter.Create(saveProjectDialog.FileName, new XmlWriterSettings { Indent = true }))
                {
                    var serializer = new XmlSerializer(typeof(StereotacticProtocol));
                    serializer.Serialize(writer, protocol);
                    saveVersion = version;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveProjectDialog.ShowDialog() == DialogResult.OK)
            {
                saveToolStripMenuItem_Click(this, e);
            }
        }

        private void glControl_MouseClick(object sender, MouseEventArgs e)
        {
            var cursor = PickModelPoint();
            var reference = GetReferencePoint();
            selectedPoint = ClosestPoint(cursor, reference);
            if (selectedPoint != null)
            {
                propertyGrid.SelectedObject = selectedPoint;
            }
            else propertyGrid.SelectedObject = protocol;
        }
    }
}
