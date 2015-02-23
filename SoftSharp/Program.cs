using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SoftSharp
{
    static class Program
    {
        static Vertex[] vertices; // vertex buffer data
        static int polyNum; // number of polygons

        static bool ReadMesh()
        {
            Stream In = File.OpenRead("Model.txt");
            if (In == null) return false;

            BinaryReader reader = new BinaryReader(In);

            polyNum = reader.ReadInt32();
            vertices = new Vertex[polyNum * 3];

            // read in all the polygon data
            for (int i = 0; i < polyNum; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // read in normal
                    vertices[i * 3 + j].Normal.X = reader.ReadSingle();
                    vertices[i * 3 + j].Normal.Y = reader.ReadSingle();
                    vertices[i * 3 + j].Normal.Z = reader.ReadSingle();
                    // read in position
                    vertices[i * 3 + j].Position.X = reader.ReadSingle();
                    vertices[i * 3 + j].Position.Y = reader.ReadSingle();
                    vertices[i * 3 + j].Position.Z = reader.ReadSingle();
                }
            }

            reader.Close();
            In.Close();
            return true;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            int width = 300, height = 300;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 gameForm = new Form1();
            RenderDevice device = new RenderDevice(gameForm, width, height);
            gameForm.Show();

            Viewport vp;
            vp.X = 0;
            vp.Y = 0;
            vp.Near = 0.0f;
            vp.Far = 1.0f;
            vp.Width = width;
            vp.Height = height;

            device.Viewport = vp;

            device.ShadeMode = ShadeMode.Smooth;
            device.DepthTestEnabled = true;
            device.DepthFunction = DepthFunction.LessEqual;
            device.ClearColor = Color.Black;
            device.DepthClearValue = 1.0f;

            ReadMesh();

            while (gameForm.Created)
            {
                device.Clear(ClearFlags.Color | ClearFlags.Depth);

                // setup transforms

                device.ModelMatrix = Matrix.Scale(3, 3, 3) * Matrix.RotateY(Environment.TickCount / 180.0f);
                device.ViewMatrix = Matrix.LookAt(new Vector3(0, 3, 5),
                                                  new Vector3(0, 0, 0),
                                                  new Vector3(0, 1, 0));
                device.ProjectionMatrix = Matrix.PerspectiveFOV((float)Math.PI / 4, width / height, 1.0f, 100.0f);

                // setup material
                Material mtrl = new Material();
                mtrl.Diffuse = mtrl.Ambient = Color.White;
                device.Material = mtrl;

                // setup light
                Vector3 vecDir;
                Light light = new Light();
                light.Type = LightType.Directional;
                light.Diffuse = Color.White;
                vecDir = new Vector3(0, 0, -1);
                vecDir.Normalize();
                light.Direction = vecDir;
                light.Range = 1000.0f;
                light.Enabled = true;
                device.Light[0] = light;
                device.LightEnabled = true;
                device.AmbientLight = Color.FromArgb(0x00202020);
                device.MaterialSource = MaterialSource.Material;

                device.BeginScene();
                device.DrawPrimitive(vertices, PrimitiveType.TriangleList, 0, polyNum);
                device.EndScene();
                
                device.Present();

                Application.DoEvents();
            }
        }
    }
}
