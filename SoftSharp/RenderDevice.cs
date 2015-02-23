using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace SoftSharp
{
    struct InternalColor
    {
        public float R;
        public float G;
        public float B;
        
        public void Clamp()
        {
            if (R > 1.0f) R = 1.0f;
            if (R < 0.0f) R = 0.0f;
            if (G > 1.0f) G = 1.0f;
            if (G < 0.0f) G = 0.0f;
            if (B > 1.0f) B = 1.0f;
            if (B < 0.0f) B = 0.0f;
        }
    }
    /// <summary>
    /// This internal format is used for the triangle rasterizer
    /// </summary>
    struct InternalVertex
    {
        public Vector4 position; // position
        public Vector2 texcoord; // texture coordinates
        public InternalColor color; // vertex color
    }

    /// <summary>
    /// This type is used to hold our list of triangles for rendering
    /// </summary>
    struct Triangle
    {
        public InternalVertex v1, v2, v3;
        public bool visible;
    }

    public class RenderDevice
    {
        // private data
        BitmapData backBufferData = null;
        BitmapData textureData = null;
        Triangle[] triangleList = null;
        Bitmap backBuffer = null; // 24-bit color buffer
        ushort[,] depthBuffer = null; // 16-bit depth buffer
        Bitmap texture = null;
        Material material;
        bool lightEnabled = false;
        bool depthEnabled = false;
        bool textureEnabled = false;
        CullMode cullMode = CullMode.None;
        DepthFunction depthFunc = DepthFunction.Always;
        Color clearColor = Color.Black;
        float depthValue = 1.0f;
        Form parentForm = null;
        int width, height;
        bool blendVertexColor = false;
        bool cullEnabled = false;
        ShadeMode shadeMode = ShadeMode.Smooth;
        Viewport viewport;
        Matrix modelMatrix;
        Matrix viewMatrix;
        Matrix projMatrix;
        Graphics parentFormGfx = null;
        Color ambientLight = Color.Black;
        MaterialSource matSource = MaterialSource.Vertex;

        public RenderDevice(Form parent, int width, int height)
        {
            parentForm = parent;
            parentForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.width = width;
            this.height = height;
            parentForm.ClientSize = new Size(width, height);

            // create color and depth buffers
            backBuffer = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            depthBuffer = new ushort[width, height];

            // create graphics object for drawing on screen
            parentFormGfx = Graphics.FromHwnd(parent.Handle);
        }

        // properties

        public MaterialSource MaterialSource
        {
            get
            {
                return matSource;
            }

            set
            {
                matSource = value;
            }
        }

        public Color AmbientLight
        {
            get
            {
                return ambientLight;
            }
            set
            {
                ambientLight = value;
            }
        }

        public Matrix ModelMatrix
        {
            get
            {
                return modelMatrix;
            }

            set
            {
                modelMatrix = value;
            }
        }

        public Matrix ViewMatrix
        {
            get
            {
                return viewMatrix;
            }

            set
            {
                viewMatrix = value;
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                return projMatrix;
            }

            set
            {
                projMatrix = value;
            }
        }

        public Viewport Viewport
        {
            get
            {
                return viewport;
            }

            set
            {
                viewport = value;
            }
        }

        public ShadeMode ShadeMode
        {
            get
            {
                return shadeMode;
            }

            set
            {
                shadeMode = value;
            }
        }

        /// <summary>
        /// gets / sets current texture
        /// </summary>
        public Bitmap Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        /// <summary>
        /// toggles vertex lighting
        /// </summary>
        public bool LightEnabled
        {
            get
            {
                return lightEnabled;
            }

            set
            {
                lightEnabled = value;
            }
        }

        /// <summary>
        /// toggles texture mapping
        /// </summary>
        public bool TextureEnabled
        {
            get
            {
                return textureEnabled;
            }

            set
            {
                textureEnabled = value;
            }
        }

        /// <summary>
        /// toggles vertex color blending with textures
        /// </summary>
        public bool BlendVertexColorEnabled
        {
            get
            {
                return blendVertexColor;
            }

            set
            {
                blendVertexColor = true;
            }
        }

        /// <summary>
        /// toggle back face culling
        /// </summary>
        public bool CullModeEnabled
        {
            get
            {
                return cullEnabled;
            }

            set
            {
                cullEnabled = value;
            }
        }

        /// <summary>
        /// set cull mode
        /// </summary>
        public CullMode CullMode
        {
            get
            {
                return cullMode;
            }

            set
            {
                cullMode = value;
            }
        }

        /// <summary>
        /// set render target clear color
        /// </summary>
        public Color ClearColor
        {
            get
            {
                return clearColor;
            }

            set
            {
                clearColor = value;
            }
        }

        /// <summary>
        /// set depth buffer clear value
        /// </summary>
        public float DepthClearValue
        {
            get
            {
                return depthValue;
            }

            set
            {
                depthValue = value;
            }
        }

        /// <summary>
        /// toggle depth testing
        /// </summary>
        public bool DepthTestEnabled
        {
            get
            {
                return depthEnabled;
            }

            set
            {
                depthEnabled = value;
            }
        }

        /// <summary>
        /// set depth function
        /// </summary>
        public DepthFunction DepthFunction
        {
            get
            {
                return depthFunc;
            }

            set
            {
                depthFunc = value;
            }
        }

        /// <summary>
        /// array of vertex lights
        /// </summary>
        public Light[] Light = new Light[8];

        /// <summary>
        /// surface properties for light interaction
        /// </summary>
        public Material Material
        {
            get
            {
                return material;
            }

            set
            {
                material = value;
            }
        }

        /// <summary>
        /// Rendering area width
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// Rendering area height
        /// </summary>
        public int Height
        {
            get
            {
                return height;
            }
        }

        // public methods...

        /// <summary>
        /// used to clear the color buffer and depth buffer
        /// </summary>
        /// <param name="flags">Specifies which buffer(s) to clear</param>
        public void Clear(ClearFlags flags)
        {
            // clear the back buffer if that flag is set
            if ((flags & ClearFlags.Color) == ClearFlags.Color)
            {
                Graphics gfx = Graphics.FromImage(backBuffer);
                gfx.Clear(clearColor);
                gfx.Dispose();
            }

            // clear depth buffer if that flag is set
            if ((flags & ClearFlags.Depth) == ClearFlags.Depth)
            {
                for(int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        depthBuffer[i, j] = (ushort)(depthValue * ushort.MaxValue);
            }
        }

        /// <summary>
        /// Draw submitted geometric data
        /// </summary>
        /// <param name="type">Topology used for assembling triangles</param>
        /// <param name="start">Index to start at in vertex buffer</param>
        /// <param name="numTriangles">Number of triangles to render</param>
        public void DrawPrimitive(Vertex[] vertexBuffer, PrimitiveType type, int start, int numTriangles)
        {
            if (backBufferData == null)
                throw new Exception("Called DrawPrimitive outside of BeginScene/EndScene block.");

            if (vertexBuffer == null)
            {
                Console.WriteLine("DrawPrimitive failed because vertex buffer was not set.");
                return; // nothing to render (fail silently)
            }

            triangleList = new Triangle[numTriangles]; // allocate a new triangle list

            int[] indices = { 0, 0, 0 }; // indices to lookup in vertex buffer
            int curIndex = start; // index of current vertex

            // for every triangle...
            for (int i = 0; i < numTriangles; i++)
            {
                // fetch 3 vertices from vertex buffer based on primitive topology
                switch (type)
                {
                    case PrimitiveType.TriangleFan:
                        indices[0] = 0; // always first vertex
                        indices[1] = curIndex + 1;
                        indices[2] = curIndex + 2;
                        curIndex++;
                        break;
                    case PrimitiveType.TriangleList:
                        indices[0] = curIndex + 0;
                        indices[1] = curIndex + 1;
                        indices[2] = curIndex + 2;
                        curIndex += 3;
                        break;
                    case PrimitiveType.TriangleStrip:
                        // alternate vertex order for even and odd triangles (ensuring they're all in clockwise order)
                        if (curIndex % 2 == 0)
                        {
                            // even numbered triangle
                            indices[0] = curIndex + 0;
                            indices[1] = curIndex + 1;
                            indices[2] = curIndex + 2;
                        }
                        else
                        {
                            // odd numbered triangle
                            indices[0] = curIndex + 0;
                            indices[1] = curIndex + 2;
                            indices[2] = curIndex + 1;
                        }
                        curIndex++;
                        break;
                }

                // do transformation and lighting, store vertices temp triangle
                Triangle tempTri; InternalVertex[] tempVerts = new InternalVertex[3];
                for (int j = 0; j < 3; j++)
                {
                    // run vertex shader
                    tempVerts[j] = ProcessVertex(vertexBuffer[indices[j]]);
                }

                tempTri.v1 = tempVerts[0]; tempTri.v2 = tempVerts[1]; tempTri.v3 = tempVerts[2];
                tempTri.visible = true; // make it visible by default

                // check if backface culling is enabled
                if (cullEnabled)
                {
                    // set triangle visibility to true or false based on cull mode
                    if (cullMode == CullMode.FrontAndBack)
                    {
                        // in this case we're not rendering anything so bail
                        return;
                    }

                    if (cullMode == CullMode.Back)
                    {
                        // cull counterclockwise (back facing) triangles
                        if (CullTriangle(tempTri))
                            tempTri.visible = false;
                    }

                    if (cullMode == CullMode.Front)
                    {
                        // cull clockwise (front facing) triangles
                        if (!CullTriangle(tempTri))
                            tempTri.visible = false;
                    }
                }

                // set triangle visibility based on clipping planes
                if (ClipTriangle(tempTri))
                    tempTri.visible = false;

                // flat shading
                if (shadeMode == ShadeMode.Flat) tempTri.v3.color = tempTri.v2.color = tempTri.v1.color;

                // assign temp triangle to triangle list
                triangleList[i] = tempTri;
            }

            // -- at this point, we have a list of renderable triangles --

            // if we're using texture maps, go ahead and lock the texture image now
            if (textureEnabled && texture != null)
            {
                textureData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            }

            // for every triangle, render with appropriate rasterization function
            foreach (Triangle tri in triangleList)
            {
                if (!tri.visible)
                    continue;
                if (texture == null || !textureEnabled)
                    DrawTriangleSmooth(tri);
                if (texture != null && !blendVertexColor && textureEnabled)
                    DrawTriangleTextured(tri);
                if (texture != null && blendVertexColor && textureEnabled)
                    DrawTriangleSmoothTextured(tri);
            }

            if (textureEnabled && texture != null)
            {
                texture.UnlockBits(textureData);
            }
        }

        /// <summary>
        /// Draw submitted geometric data using index buffers
        /// </summary>
        /// <param name="type">Topology used for assembling triangles</param>
        /// <param name="start">Index to start at in index buffer</param>
        /// <param name="numTriangles">Number of triangles to render</param>
        public void DrawIndexedPrimitive(Vertex[] vertexBuffer, int[] indexBuffer, PrimitiveType type, int start, int numTriangles)
        {
            if (backBufferData == null)
                throw new Exception("Called DrawIndexedPrimitive outside of BeginScene/EndScene block.");

            if (indexBuffer == null || vertexBuffer == null)
            {
                Console.WriteLine("DrawIndexedPrimitive failed because vertex buffer or index buffer wasn't set.");
                return; // nothing to render (fail silently)
            }

            triangleList = new Triangle[numTriangles];

            int curIndex = start; // current vertex index (for fetching vertices)
            int[] indices = { 0, 0, 0 };

            // for every triangle...
            for (int i = 0; i < numTriangles; i++)
            {
                // fetch 3 indices from index buffer based on primitive topology
                switch (type)
                {
                    case PrimitiveType.TriangleFan:
                        indices[0] = 0; // always first vertex
                        indices[1] = curIndex + 1;
                        indices[2] = curIndex + 2;
                        curIndex++;
                        break;
                    case PrimitiveType.TriangleList:
                        indices[0] = curIndex + 0;
                        indices[1] = curIndex + 1;
                        indices[2] = curIndex + 2;
                        curIndex += 3;
                        break;
                    case PrimitiveType.TriangleStrip:
                        // alternate vertex order for even and odd triangles (ensuring they're all in clockwise order)
                        if (curIndex % 2 == 0)
                        {
                            // even numbered triangle
                            indices[0] = curIndex + 0;
                            indices[1] = curIndex + 1;
                            indices[2] = curIndex + 2;
                        }
                        else
                        {
                            // odd numbered triangle
                            indices[0] = curIndex + 0;
                            indices[1] = curIndex + 2;
                            indices[2] = curIndex + 1;
                        }
                        curIndex++;
                        break;
                }

                // use indices to get vertices from vertex buffer
                Triangle tempTri;
                int[] tempIndices = new int[3];
                InternalVertex[] tempVerts = new InternalVertex[3];

                for (int j = 0; j < 3; j++)
                    // do transformation and lighting
                    tempVerts[j] = ProcessVertex(vertexBuffer[indexBuffer[indices[j]]]);
                
                // store vertices temp triangle
                tempTri.v1 = tempVerts[0]; tempTri.v2 = tempVerts[1]; tempTri.v3 = tempVerts[2];
                tempTri.visible = true; // set to visible by default

                // check if backface culling is enabled
                if (cullEnabled)
                {
                    // set triangle visibility to true or false based on cull mode
                    if (cullMode == CullMode.FrontAndBack)
                    {
                        // in this case we're not rendering anything so bail
                        return;
                    }

                    else if (cullMode == CullMode.Back)
                    {
                        // cull counterclockwise (back facing) triangles
                        if (CullTriangle(tempTri))
                            tempTri.visible = false;
                    }

                    else if (cullMode == CullMode.Front)
                    {
                        // cull clockwise (front facing) triangles
                        if (!CullTriangle(tempTri))
                            tempTri.visible = false;
                    }
                }

                // set triangle visibility based on clipping planes
                if (ClipTriangle(tempTri))
                    tempTri.visible = false;

                // flat shading
                if (shadeMode == ShadeMode.Flat) tempTri.v3.color = tempTri.v2.color = tempTri.v1.color;

                // assign temp triangle to triangle list
                triangleList[i] = tempTri;
            }

            // -- at this point, we have a list of renderable triangles --

            // if we're using texture maps, go ahead and lock the texture image now
            if (textureEnabled && texture != null)
            {
                textureData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height), ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
            }

            // for every triangle, render with appropriate rasterization function
            foreach (Triangle tri in triangleList)
            {
                if (!tri.visible) continue;
                if (texture == null || !textureEnabled)
                    DrawTriangleSmooth(tri);
                if (texture != null && !blendVertexColor && textureEnabled)
                    DrawTriangleTextured(tri);
                if (texture != null && blendVertexColor && textureEnabled)
                    DrawTriangleSmoothTextured(tri);
            }

            // unlock texture image
            if (textureEnabled && texture != null)
            {
                texture.UnlockBits(textureData);
            }
        }

        /// <summary>
        /// Prints a string to the render target
        /// </summary>
        /// <param name="font">Font used on text</param>
        /// <param name="x">Horizontal position on screen (in pixels)</param>
        /// <param name="y">Vertical position on screen (in pixels)</param>
        /// <param name="color">Color of text to be drawn</param>
        /// <param name="str">String to be printed</param>
        public void DrawString(Font font, int x, int y, Color color, string str)
        {
            if (backBufferData != null) return; // fail silently if back buffer is locked
            var gfx = Graphics.FromImage(backBuffer);
            gfx.DrawString(str, font, new SolidBrush(color), x, y);
        }

        /// <summary>
        /// Update screen by blitting the back buffer to the window client area
        /// </summary>
        public void Present()
        {
            parentFormGfx.DrawImage(backBuffer, 0, 0);
        }

        /// <summary>
        /// Lock back buffer for rendering (so we don't need to do it every draw call)
        /// </summary>
        public void BeginScene()
        {
            if (backBufferData != null)
                throw new Exception("Did not call EndScene after previous BeginScene call.");

            backBufferData = backBuffer.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Unlock back buffer
        /// </summary>
        public void EndScene()
        {
            if (backBufferData == null)
                throw new Exception("EndScene called without matching BeginScene.");

            backBuffer.UnlockBits(backBufferData);
            backBufferData = null;
        }

        private InternalVertex ProcessVertex(Vertex input)
        {
            InternalVertex result;
            Matrix modelViewProj = modelMatrix * viewMatrix * projMatrix; // setup matrix to transform our vertex to clip space
            Vector4 inputPos = new Vector4(input.Position.X, input.Position.Y, input.Position.Z, 1);
            result.position = Vector4.Transform(inputPos, modelViewProj); // coordinates now in clip space

            if (lightEnabled)
            {
                // ambient light
                InternalColor globalAmbient, diff, amb, spec;
                globalAmbient.R = ambientLight.R / 255.0f;
                globalAmbient.G = ambientLight.G / 255.0f;
                globalAmbient.B = ambientLight.B / 255.0f;
                result.color = globalAmbient;

                // initialize all the color contribution values...
                diff.R = diff.G = diff.B = amb.R = amb.B = amb.G = spec.R = spec.G = spec.B = 0;

                Matrix wvMatrix = modelMatrix * viewMatrix; // camera space transform
                Vector3 V = Vector3.Transform(input.Position, wvMatrix);
                Vector3 N = Vector3.Transform(input.Normal, Matrix.Transpose(Matrix.Inverse(viewMatrix)));

                for (int i = 0; i < Light.Length; i++)
                {
                    if (!Light[i].Enabled) continue; // skip lights that aren't enabled

                    // camera space light position
                    Vector3 lightPos = Vector3.Transform(Light[i].Position, viewMatrix);

                    // light direction
                    Vector3 lightDir;

                    if (Light[i].Type == LightType.Directional)
                    {
                        lightDir = Light[i].Direction;
                        lightDir.Normalize();
                        lightDir.X = -lightDir.X;
                        lightDir.Y = -lightDir.Y;
                        lightDir.Z = -lightDir.Z;
                    }
                    else
                    {
                        lightDir = Vector3.CrossProduct(V, lightPos);
                        lightDir.Normalize();
                    }

                    // distance
                    float d = lightDir.Length;

                    // attenuation
                    float atten;
                    if (Light[i].Type != LightType.Directional)
                    {
                        atten = 1.0f / (Light[0].Attenuation0 + Light[0].Attenuation1 * d + Light[0].Attenuation2 * (d * d));
                    }
                    else
                        atten = 1.0f;

                    // spotlight
                    float spot;
                    if (Light[i].Type == LightType.Spot)
                    {
                        Vector3 ldcs = Vector3.Transform(Light[i].Direction, viewMatrix);
                        ldcs.X = -ldcs.X;
                        ldcs.Y = -ldcs.Y;
                        ldcs.Z = -ldcs.Z;
                        ldcs.Normalize();
                        lightDir.Normalize();
                        float rho = Vector3.DotProduct(ldcs, lightDir);

                        if (rho <= (float)Math.Cos(Light[i].Phi / 2))
                            spot = 0;
                        else
                        {
                            float spotNum = rho - (float)Math.Cos(Light[i].Phi / 2);
                            float spotDenom = (float)Math.Cos(Light[i].Theta / 2) - (float)Math.Cos(Light[i].Phi / 2);
                            spot = (float)Math.Pow((spotNum / spotDenom), Light[i].Falloff);
                        }
                    }
                    else
                        spot = 1.0f;

                    // current ambient value
                    amb.R += spot * atten * (Light[i].Ambient.R / 255.0f);
                    amb.G += spot * atten * (Light[i].Ambient.G / 255.0f);
                    amb.B += spot * atten * (Light[i].Ambient.B / 255.0f);

                    // current diffuse value
                    float nDotL = Vector3.DotProduct(N, lightDir);
                    if (matSource == MaterialSource.Material)
                    {
                        diff.R += (material.Diffuse.R / 255.0f) * (Light[i].Diffuse.R / 255.0f) * nDotL * atten * spot;
                        diff.G += (material.Diffuse.G / 255.0f) * (Light[i].Diffuse.G / 255.0f) * nDotL * atten * spot;
                        diff.B += (material.Diffuse.B / 255.0f) * (Light[i].Diffuse.B / 255.0f) * nDotL * atten * spot;
                    }
                    else
                    {
                        diff.R += (input.Diffuse.R / 255.0f) * (Light[i].Diffuse.R / 255.0f) * nDotL * atten * spot;
                        diff.G += (input.Diffuse.G / 255.0f) * (Light[i].Diffuse.G / 255.0f) * nDotL * atten * spot;
                        diff.B += (input.Diffuse.B / 255.0f) * (Light[i].Diffuse.B / 255.0f) * nDotL * atten * spot;
                    }

                    // current specular value
                    Vector3 camPos; // get camera position for calculating halfway vector
                    Matrix viewInv = Matrix.Inverse(viewMatrix); // inverse of view matrix (also needed for half vector calc)
                    camPos.X = viewInv.M41;
                    camPos.Y = viewInv.M42;
                    camPos.Z = viewInv.M43;

                    // calculate halfway vector
                    Vector3 vertexPos = Vector3.Transform(input.Position, modelMatrix);
                    Vector3 camToVertex = camPos - vertexPos;
                    camToVertex.Normalize();
                    Vector3 half = camToVertex + lightDir;
                    half.Normalize();

                    float specContrib = (float)Math.Pow(Vector3.DotProduct(N, half), material.Power);
                    spec.R += (Light[i].Specular.R / 255.0f) * specContrib * atten * spot;
                    spec.G += (Light[i].Specular.G / 255.0f) * specContrib * atten * spot;
                    spec.B += (Light[i].Specular.B / 255.0f) * specContrib * atten * spot;
                }

                // take sum of component terms and multiply by vertex / material values
                InternalColor emissive;
                if (matSource == MaterialSource.Material)
                {
                    emissive.R = (material.Emissive.R / 255.0f);
                    emissive.G = (material.Emissive.G / 255.0f);
                    emissive.B = (material.Emissive.B / 255.0f);

                    // ambient
                    amb.R = (material.Ambient.R / 255.0f) * (amb.R + globalAmbient.R);
                    amb.G = (material.Ambient.G / 255.0f) * (amb.G + globalAmbient.G);
                    amb.B = (material.Ambient.B / 255.0f) * (amb.B + globalAmbient.B);

                    //specular
                    spec.R *= (material.Specular.R / 255.0f);
                    spec.G *= (material.Specular.G / 255.0f);
                    spec.B *= (material.Specular.B / 255.0f);
                }
                else
                {
                    emissive.R = emissive.G = emissive.B = 0.0f;

                    // ambient
                    amb.R = (amb.R + globalAmbient.R);
                    amb.G = (amb.G + globalAmbient.G);
                    amb.B = (amb.B + globalAmbient.B);

                    //specular
                    spec.R *= (input.Specular.R / 255.0f);
                    spec.G *= (input.Specular.G / 255.0f);
                    spec.B *= (input.Specular.B / 255.0f);
                }

                // final lit vertex color
                amb.Clamp(); diff.Clamp(); spec.Clamp(); emissive.Clamp();
                result.color.R = amb.R + diff.R + spec.R + emissive.R;
                result.color.G = amb.G + diff.G + spec.G + emissive.G;
                result.color.B = amb.B + diff.B + spec.B + emissive.B;
            }
            else
            {
                // no lighting, just use diffuse color component for vertex
                result.color.R = input.Diffuse.R / 255.0f;
                result.color.G = input.Diffuse.G / 255.0f;
                result.color.B = input.Diffuse.B / 255.0f;
            }

            result.color.Clamp(); // make sure light/color values are in appropriate range

            // transform output vertex to screen space
            float num = result.position.X * viewport.Width;
            float denom = 2 * result.position.W;
            float center = viewport.Width / 2;

            result.position.X = (num / denom) + center;

            num = result.position.Y * viewport.Height;
            denom = 2 * result.position.W;
            center = viewport.Height / 2;

            result.position.Y = (-num / denom) + center;

            result.texcoord = input.TexCoord;
            result.texcoord = input.TexCoord;

            return result; // now we have a vertex that's ready for our triangle structure
        }

        /// <summary>
        /// Used to cull back facing triangles
        /// </summary>
        /// <param name="t">Transformed triangle ready for rasterization</param>
        /// <returns>True if back facing (vertices are counterclockwise), false otherwise</returns>
        private bool CullTriangle(Triangle t)
        {
            if (t.v1.position.Y <= t.v2.position.Y && t.v3.position.Y <= t.v2.position.Y) // triangle is clockwise
                return false;
            return true; // triangle is counterclockwise
        }

        /// <summary>
        /// Checks to see if we need to clip a triangle that's outside the view volume.
        /// </summary>
        /// <param name="t">Triangle to be clipped</param>
        /// <returns>True if triangle is outside view volume.</returns>
        private bool ClipTriangle(Triangle t)
        {
            bool result = false;
            if (!VertexInVolume(t.v1) && !VertexInVolume(t.v2) && !VertexInVolume(t.v3))
                result = true;
            return result;
        }

        bool VertexInVolume(InternalVertex v)
        {
            if (v.position.X >= viewport.X && v.position.X < viewport.Width &&
                v.position.Y >= viewport.Y && v.position.Y < viewport.Height &&
                (v.position.Z / v.position.W) >= viewport.Near &&
                (v.position.Z / v.position.W) < viewport.Far)
                return true;
            return false;
        }

        /// <summary>
        /// Draw a smooth shaded triangle
        /// </summary>
        /// <param name="t"></param>
        private void DrawTriangleSmooth(Triangle t)
        {
            // vertices should be such that a.position.Y <= b.position.Y <= c.position.Y
            InternalVertex[] vertices = { t.v1, t.v2, t.v3 };
            IEnumerable<InternalVertex> sortedVerts = from vertex in vertices
                                                     orderby vertex.position.Y ascending
                                                     select vertex;
            var a = sortedVerts.ElementAt(0);
            var b = sortedVerts.ElementAt(1);
            var c = sortedVerts.ElementAt(2);

            // transform Z coordinates to clip space (for Z buffering)
            a.position.Z /= a.position.W;
            b.position.Z /= b.position.W;
            c.position.Z /= c.position.W;

            float dx1, dx2, dx3; // interpolation values for x coordinates
            float dz1, dz2, dz3;
            float dz;
            float dr, dg, db;
            float dr1, dr2, dr3, dg1, dg2, dg3, db1, db2, db3; // delta values for color interp

            int width = viewport.Width;
            int height = viewport.Height;
            int x = viewport.X, y = viewport.Y;

            if (b.position.Y - a.position.Y > 0)
            {
                dx1 = (b.position.X - a.position.X) / (b.position.Y - a.position.Y);
                dz1 = (b.position.Z - a.position.Z) / (b.position.Y - a.position.Y);
                dr1 = (b.color.R - a.color.R) / (b.position.Y - a.position.Y);
                dg1 = (b.color.G - a.color.G) / (b.position.Y - a.position.Y);
                db1 = (b.color.B - a.color.B) / (b.position.Y - a.position.Y);
            }
            else
                dx1 = dz1 = dr1 = dg1 = db1 = 0;

            if (c.position.Y - a.position.Y > 0)
            {
                dx2 = (c.position.X - a.position.X) / (c.position.Y - a.position.Y);
                dz2 = (c.position.Z - a.position.Z) / (c.position.Y - a.position.Y);
                dr2 = (c.color.R - a.color.R) / (c.position.Y - a.position.Y);
                dg2 = (c.color.G - a.color.G) / (c.position.Y - a.position.Y);
                db2 = (c.color.B - a.color.B) / (c.position.Y - a.position.Y);
            }
            else
                dx2 = dz2 = dr2 = dg2 = db2 = 0;

            if (c.position.Y - b.position.Y > 0)
            {
                dx3 = (c.position.X - b.position.X) / (c.position.Y - b.position.Y);
                dz3 = (c.position.Z - b.position.Z) / (c.position.Y - b.position.Y);
                dr3 = (c.color.R - b.color.R) / (c.position.Y - b.position.Y);
                dg3 = (c.color.G - b.color.G) / (c.position.Y - b.position.Y);
                db3 = (c.color.B - b.color.B) / (c.position.Y - b.position.Y);
            }
            else
                dx3 = dz3 = dr3 = dg3 = db3 = 0;

            InternalVertex s, e;
            s = e = a;

            if (dx1 > dx2)
            {
                for (; s.position.Y <= Math.Min(height - 1, b.position.Y); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = dr = dg = db = 0;

                    InternalVertex p = s;
                    p.position.X = Math.Max(p.position.X, x);
                    for (; p.position.X < Math.Min(width, e.position.X); p.position.X++)
                    {
                        if (p.position.Y < y) continue; // prevent drawing out of upper vertical range
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, Color.FromArgb(255, (int)(p.color.R * 255), (int)(p.color.G * 255),
                            (int)(p.color.B * 255)));
                        p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }

                    s.position.X += dx2; s.color.R += dr2; s.color.G += dg2; s.color.B += db2; s.position.Z += dz2;
                    e.position.X += dx1; e.color.R += dr1; e.color.G += dg1; e.color.B += db1; e.position.Z += dz1;
                }

                e = b;
                for (; s.position.Y <= Math.Min(c.position.Y, height - 1); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = dr = dg = db = 0;
                    InternalVertex p = s;
                    p.position.X = Math.Max(p.position.X, x);
                    for (; p.position.X < Math.Min(width, e.position.X); p.position.X++)
                    {

                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, Color.FromArgb(255, (int)(p.color.R * 255), (int)(p.color.G * 255),
                            (int)(p.color.B * 255)));
                        p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }

                    s.position.X += dx2; s.color.R += dr2; s.color.G += dg2; s.color.B += db2; s.position.Z += dz2;
                    e.position.X += dx3; e.color.R += dr3; e.color.G += dg3; e.color.B += db3; e.position.Z += dz3;
                }
            }
            else
            {
                for (; s.position.Y <= Math.Min(b.position.Y, height); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = dr = dg = db = 0;

                    InternalVertex p = s;
                    p.position.X = Math.Max(x, p.position.X);
                    for (; p.position.X < Math.Min(e.position.X, width); p.position.X++)
                    {
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, Color.FromArgb(255, (int)(p.color.R * 255), (int)(p.color.G * 255),
                            (int)(p.color.B * 255)));
                        p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }

                    s.position.X += dx1; s.color.R += dr1; s.color.G += dg1; s.color.B += db1; s.position.Z += dz1;
                    e.position.X += dx2; e.color.R += dr2; e.color.G += dg2; e.color.B += db2; s.position.Z += dz2;
                }
                s = b;

                for (; s.position.Y <= Math.Min(c.position.Y, height); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = dr = dg = db = 0;
                    InternalVertex p = s;
                    p.position.X = Math.Max(x, p.position.X);
                    for (; p.position.X < Math.Min(e.position.X, width); p.position.X++)
                    {
                        p.color.Clamp();
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, Color.FromArgb(255, (int)(p.color.R * 255), (int)(p.color.G * 255),
                            (int)(p.color.B * 255)));
                        p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }
                    s.position.X += dx3; s.color.R += dr3; s.color.G += dg3; s.color.B += db3; s.position.Z += dz3;
                    e.position.X += dx2; e.color.R += dr2; e.color.G += dg2; e.color.B += db2; e.position.Z += dz3;
                }
            }
        }

        void SetPixel(int x, int y, float z, Color color)
        {
            if (!DepthPass(x, y, z)) return;

            unsafe
            {
                byte* data = (byte*)backBufferData.Scan0;
                data[(y * backBufferData.Stride) + (x * 3)] = color.B;
                data[(y * backBufferData.Stride) + (x * 3) + 1] = color.G;
                data[(y * backBufferData.Stride) + (x * 3) + 2] = color.R;
            }
        }

        bool DepthPass(int x, int y, float z)
        {
            if (!depthEnabled) return true; // always pass in this case
            if (x < viewport.X || x >= viewport.Width || y < viewport.Y || y >= viewport.Height)
                return false; // prevent out of bounds memory read

            ushort pixel = depthBuffer[x, y];
            ushort value = (ushort)(z * ushort.MaxValue);

            bool pass = false;

            switch (depthFunc)
            {
                case DepthFunction.Always:
                    pass = true;
                    break;
                case DepthFunction.Equal:
                    if (value == pixel) pass = true;
                    break;
                case DepthFunction.Greater:
                    if (value > pixel) pass = true;
                    break;
                case DepthFunction.GreaterEqual:
                    if (value >= pixel) pass = true;
                    break;
                case DepthFunction.Less:
                    if (value < pixel) pass = true;
                    break;
                case DepthFunction.LessEqual:
                    if (value <= pixel) pass = true;
                    break;
                case DepthFunction.Never:
                    pass = false;
                    break;
                case DepthFunction.NotEqual:
                    if (value != pixel) pass = true;
                    break;
            }

            if (pass) // write new depth buffer value
                depthBuffer[x, y] = value;

            return pass;
        }

        private Color GetTexel(int x, int y)
        {
            Color result;
            unsafe
            {
                byte* data = (byte*)textureData.Scan0;
                result = Color.FromArgb(data[(y * textureData.Stride) + (x * 3) + 2],
                data[(y * textureData.Stride) + (x * 3) + 1],
                data[(y * textureData.Stride) + (x * 3)]);
            }
            return result;
        }

        /// <summary>
        /// Draw a textured triangle with no shading
        /// </summary>
        /// <param name="t"></param>
        private void DrawTriangleTextured(Triangle t)
        {
            InternalVertex[] vertices = { t.v1, t.v2, t.v3 };

            // vertices should be such that a.position.Y <= b.position.Y <= c.position.Y
            IEnumerable<InternalVertex> sortedVerts = from vertex in vertices
                                                      orderby vertex.position.Y ascending
                                                      select vertex;
            var a = sortedVerts.ElementAt(0);
            var b = sortedVerts.ElementAt(1);
            var c = sortedVerts.ElementAt(2);

            // transform Z coordinates to clip space (for Z buffering)
            a.position.Z /= a.position.W;
            b.position.Z /= b.position.W;
            c.position.Z /= c.position.W;

            float dx1, dx2, dx3; // interpolation values for x coordinates
            float du, dv;
            float dz1, dz2, dz3;
            float dz;
            float du1, du2, du3, dv1, dv2, dv3; // delta values for texture coord interpolation

            int width = viewport.Width;
            int height = viewport.Height;
            int x = viewport.X, y = viewport.Y;

            int texwidth = texture.Width - 1;
            int texheight = texture.Height - 1;

            if (b.position.Y - a.position.Y > 0)
            {
                dx1 = (b.position.X - a.position.X) / (b.position.Y - a.position.Y);
                dz1 = (b.position.Z - a.position.Z) / (b.position.Y - a.position.Y);
                du1 = (b.texcoord.X - a.texcoord.X) / (b.position.Y - a.position.Y);
                dv1 = (b.texcoord.Y - a.texcoord.Y) / (b.position.Y - a.position.Y);
            }
            else
                dx1 = dz1 = du1 = dv1 = 0;

            if (c.position.Y - a.position.Y > 0)
            {
                dx2 = (c.position.X - a.position.X) / (c.position.Y - a.position.Y);
                dz2 = (c.position.Z - a.position.Z) / (c.position.Y - a.position.Y);
                du2 = (c.texcoord.X - a.texcoord.X) / (c.position.Y - a.position.Y);
                dv2 = (c.texcoord.Y - a.texcoord.Y) / (c.position.Y - a.position.Y);
            }
            else
                dx2 = dz2 = du2 = dv2 = 0;

            if (c.position.Y - b.position.Y > 0)
            {
                dx3 = (c.position.X - b.position.X) / (c.position.Y - b.position.Y);
                dz3 = (c.position.Z - b.position.Z) / (c.position.Y - b.position.Y);
                du3 = (c.texcoord.X - b.texcoord.X) / (c.position.Y - b.position.Y);
                dv3 = (c.texcoord.Y - b.texcoord.Y) / (c.position.Y - b.position.Y);
            }
            else
                dx3 = dz3 = du3 = dv3 = 0;

            InternalVertex s, e;
            s = e = a;

            if (dx1 > dx2)
            {
                for (; s.position.Y <= Math.Min(height - 1, b.position.Y); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                    }
                    else
                        du = dv = dz = 0;

                    var p = s;
                    p.position.X = Math.Max(p.position.X, x);
                    for (; p.position.X < Math.Min(width, e.position.X); p.position.X++)
                    {
                        if (p.position.Y < y) continue; // prevent drawing out of upper vertical range
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, GetTexel((int)(p.texcoord.X * texwidth),
                            (int)(p.texcoord.Y * texheight)));
                        p.texcoord.X += du; p.texcoord.Y += dv; p.position.Z += dz;
                    }

                    s.position.X += dx2; s.texcoord.X += du2; s.texcoord.Y += dv2; s.position.Z += dz2;
                    e.position.X += dx1; e.texcoord.X += du1; e.texcoord.Y += dv1; e.position.Z += dz1;
                }

                e = b;
                for (; s.position.Y <= Math.Min(c.position.Y, height - 1); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = 0;
                    var p = s;
                    p.position.X = Math.Max(p.position.X, x);
                    for (; p.position.X < Math.Min(width, e.position.X); p.position.X++)
                    {
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, GetTexel((int)(p.texcoord.X * texwidth),
                            (int)(p.texcoord.Y * texheight)));
                        p.texcoord.X += du; p.texcoord.Y += dv; p.position.Z += dz;
                    }

                    s.position.X += dx2; s.texcoord.X += du2; s.texcoord.Y += dv2; s.position.Z += dz2;
                    e.position.X += dx3; e.texcoord.X += du3; e.texcoord.Y += dv3; e.position.Z += dz3;
                }
            }
            else
            {
                for (; s.position.Y <= Math.Min(b.position.Y, height); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = 0;

                    var p = s;
                    p.position.X = Math.Max(x, p.position.X);
                    for (; p.position.X < Math.Min(e.position.X, width); p.position.X++)
                    {
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, GetTexel((int)(p.texcoord.X * texwidth),
                            (int)(p.texcoord.Y * texheight)));
                        p.texcoord.X += du; p.texcoord.Y += dv; p.position.Z += dz;
                    }

                    s.position.X += dx1; s.texcoord.X += du1; s.texcoord.Y += dv1; s.position.Z += dz1;
                    e.position.X += dx2; e.texcoord.X += du2; e.texcoord.Y += dv2; e.position.Z += dz2;
                }
                s = b;

                for (; s.position.Y <= Math.Min(c.position.Y, height); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = 0;
                    var p = s;
                    p.position.X = Math.Max(x, p.position.X);
                    for (; p.position.X < Math.Min(e.position.X, width); p.position.X++)
                    {
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, GetTexel((int)(p.texcoord.X * texwidth),
                            (int)(p.texcoord.Y * texheight)));
                        p.texcoord.X += du; p.texcoord.Y += dv; p.position.Z += dz;
                    }
                    s.position.X += dx3; s.texcoord.X += du3; s.texcoord.Y += dv3; s.position.Z += dz3;
                    e.position.X += dx2; e.texcoord.X += du2; e.texcoord.Y += dv2; s.position.Z += dz2;
                }
            }
        }

        /// <summary>
        /// Draw a smooth shaded textured triangle
        /// </summary>
        /// <param name="t"></param>
        private void DrawTriangleSmoothTextured(Triangle t)
        {
            InternalVertex[] vertices = { t.v1, t.v2, t.v3 };

            // vertices should be such that a.y <= b.y <= c.y
            IEnumerable<InternalVertex> sortedVerts = from vertex in vertices
                                                             orderby vertex.position.Y ascending
                                                             select vertex;
            var a = sortedVerts.ElementAt(0);
            var b = sortedVerts.ElementAt(1);
            var c = sortedVerts.ElementAt(2);

            // transform Z coordinates to clip space (for Z buffering)
            a.position.Z /= a.position.W;
            b.position.Z /= b.position.W;
            c.position.Z /= c.position.W;

            float dx1, dx2, dx3; // interpolation values for x coordinates
            float dz1, dz2, dz3;
            float dz;
            float du, dv, dr, dg, db;
            float du1, du2, du3, dv1, dv2, dv3,
                dr1, dr2, dr3, dg1, dg2, dg3, db1, db2, db3; // delta values for texture coord interpolation

            int width = viewport.Width;
            int height = viewport.Height;
            int x = viewport.X, y = viewport.Y;
            int texwidth = texture.Width - 1;
            int texheight = texture.Height - 1;

            if (b.position.Y - a.position.Y > 0)
            {
                dz1 = (b.position.Z - a.position.Z) / (b.position.Y - a.position.Y);
                dx1 = (b.position.X - a.position.X) / (b.position.Y - a.position.Y);
                du1 = (b.texcoord.X - a.texcoord.X) / (b.position.Y - a.position.Y);
                dv1 = (b.texcoord.Y - a.texcoord.Y) / (b.position.Y - a.position.Y);
                dr1 = (b.color.R - a.color.R) / (b.position.Y - a.position.Y);
                dg1 = (b.color.G - a.color.G) / (b.position.Y - a.position.Y);
                db1 = (b.color.B - a.color.B) / (b.position.Y - a.position.Y);
            }
            else
                dz1 = dx1 = du1 = dv1 = dr1 = dg1 = db1 = 0;

            if (c.position.Y - a.position.Y > 0)
            {
                dz2 = (c.position.Z - a.position.Z) / (c.position.Y - a.position.Y);
                dx2 = (c.position.X - a.position.X) / (c.position.Y - a.position.Y);
                du2 = (c.texcoord.X - a.texcoord.X) / (c.position.Y - a.position.Y);
                dv2 = (c.texcoord.Y - a.texcoord.Y) / (c.position.Y - a.position.Y);
                dr2 = (c.color.R - a.color.R) / (c.position.Y - a.position.Y);
                dg2 = (c.color.G - a.color.G) / (c.position.Y - a.position.Y);
                db2 = (c.color.B - a.color.B) / (c.position.Y - a.position.Y);
            }
            else
                dz2 = dx2 = du2 = dv2 = dr2 = dg2 = db2 = 0;

            if (c.position.Y - b.position.Y > 0)
            {
                dz3 = (c.position.Z - b.position.Z) / (c.position.Y - b.position.Y);
                dx3 = (c.position.X - b.position.X) / (c.position.Y - b.position.Y);
                du3 = (c.texcoord.X - b.texcoord.X) / (c.position.Y - b.position.Y);
                dv3 = (c.texcoord.Y - b.texcoord.Y) / (c.position.Y - b.position.Y);
                dr3 = (c.color.R - b.color.R) / (c.position.Y - b.position.Y);
                dg3 = (c.color.G - b.color.G) / (c.position.Y - b.position.Y);
                db3 = (c.color.B - b.color.B) / (c.position.Y - b.position.Y);
            }
            else
                dz3 = dx3 = du3 = dv3 = dr3 = dg3 = db3 = 0;

            InternalVertex s, e;
            s = e = a;

            if (dx1 > dx2)
            {
                for (; s.position.Y <= Math.Min(height - 1, b.position.Y); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = dr = dg = db = 0;

                    var p = s;
                    p.position.X = Math.Max(p.position.X, x);
                    for (; p.position.X < Math.Min(width, e.position.X); p.position.X++)
                    {
                        if (p.position.Y < y) continue; // prevent drawing out of upper vertical range
                        Color texcolor = GetTexel((int)(p.texcoord.X * texwidth), (int)(p.texcoord.Y * texheight));
                        Color interp = Color.FromArgb(255, (int)(texcolor.R * p.color.R), (int)(texcolor.G * p.color.G),
                            (int)(texcolor.B * p.color.B));
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, interp);
                        p.texcoord.X += du; p.texcoord.Y += dv; p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }

                    s.position.X += dx2; s.texcoord.X += du2; s.texcoord.Y += dv2; s.color.R += dr2; s.color.G += dg2; s.color.B += db2; s.position.Z += dz2;
                    e.position.X += dx1; e.texcoord.X += du1; e.texcoord.Y += dv1; e.color.R += dr1; e.color.G += dg1; e.color.B += db1; e.position.Z += dz1;
                }

                e = b;
                for (; s.position.Y <= Math.Min(c.position.Y, height - 1); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = dr = dg = db = 0;
                    var p = s;
                    p.position.X = Math.Max(p.position.X, x);
                    for (; p.position.X < Math.Min(width, e.position.X); p.position.X++)
                    {
                        Color texcolor = GetTexel((int)(p.texcoord.X * texwidth), (int)(p.texcoord.Y * texheight));
                        Color interp = Color.FromArgb(255, (int)(texcolor.R * p.color.R), (int)(texcolor.G * p.color.G),
                            (int)(texcolor.B * p.color.B));
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, interp);
                        p.texcoord.X += du; p.texcoord.Y += dv; p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }

                    s.position.X += dx2; s.texcoord.X += du2; s.texcoord.Y += dv2; s.color.R += dr2; s.color.G += dg2; s.color.B += db2; s.position.Z += dz2;
                    e.position.X += dx3; e.texcoord.X += du3; e.texcoord.Y += dv3; e.color.R += dr3; e.color.G += dg3; e.color.B += db3; e.position.Z += dz3;
                }
            }
            else
            {
                for (; s.position.Y <= Math.Min(b.position.Y, height); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = dr = dg = db = 0;

                    var p = s;
                    p.position.X = Math.Max(x, p.position.X);
                    for (; p.position.X < Math.Min(e.position.X, width); p.position.X++)
                    {
                        Color texcolor = GetTexel((int)(p.texcoord.X * texwidth), (int)(p.texcoord.Y * texheight));
                        Color interp = Color.FromArgb(255, (int)(texcolor.R * p.color.R), (int)(texcolor.G * p.color.G),
                            (int)(texcolor.B * p.color.B));
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, interp);
                        p.texcoord.X += du; p.texcoord.Y += dv; p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }

                    s.position.X += dx1; s.texcoord.X += du1; s.texcoord.Y += dv1; s.color.R += dr1; s.color.G += dg1; s.color.B += db1; s.position.Z += dz1;
                    e.position.X += dx2; e.texcoord.X += du2; e.texcoord.Y += dv2; e.color.R += dr2; e.color.G += dg2; e.color.B += db2; e.position.Z += dz2;
                }
                s = b;

                for (; s.position.Y <= Math.Min(c.position.Y, height); s.position.Y++, e.position.Y++)
                {
                    if (e.position.X - s.position.X > 0)
                    {
                        dz = (e.position.Z - s.position.Z) / (e.position.X - s.position.X);
                        du = (e.texcoord.X - s.texcoord.X) / (e.position.X - s.position.X);
                        dv = (e.texcoord.Y - s.texcoord.Y) / (e.position.X - s.position.X);
                        dr = (e.color.R - s.color.R) / (e.position.X - s.position.X);
                        dg = (e.color.G - s.color.G) / (e.position.X - s.position.X);
                        db = (e.color.B - s.color.B) / (e.position.X - s.position.X);
                    }
                    else
                        dz = du = dv = dr = dg = db = 0;
                    var p = s;
                    p.position.X = Math.Max(x, p.position.X);
                    for (; p.position.X < Math.Min(e.position.X, width); p.position.X++)
                    {
                        Color texcolor = GetTexel((int)(p.texcoord.X * texwidth), (int)(p.texcoord.Y * texheight));
                        Color interp = Color.FromArgb(255, (int)(texcolor.R * p.color.R), (int)(texcolor.G * p.color.G),
                            (int)(texcolor.B * p.color.B));
                        SetPixel((int)p.position.X, (int)p.position.Y, p.position.Z, interp);
                        p.texcoord.X += du; p.texcoord.Y += dv; p.color.R += dr; p.color.G += dg; p.color.B += db; p.position.Z += dz;
                    }
                    s.position.X += dx3; s.texcoord.X += du3; s.texcoord.Y += dv3; s.color.R += dr3; s.color.G += dg3; s.color.B += db3; s.position.Z += dz3;
                    e.position.X += dx2; e.texcoord.X += du2; e.texcoord.Y += dv2; e.color.R += dr2; e.color.G += dg2; e.color.B += db2; e.position.Z += dz2;
                }
            }
        }
    }
}
