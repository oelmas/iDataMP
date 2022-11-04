using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MathHelper = MissionPlanner.Utilities.MathHelper;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Vector3 = MissionPlanner.Utilities.Vector3;

namespace MissionPlanner.Controls
{
    public class OpenGLtest : GLControl
    {
        public static OpenGLtest instance;

        private double _alt;

        private float _angle;

        // terrain image
        private Bitmap _terrain = new Bitmap(640, 480);

        private RectLatLng area = new RectLatLng(-35.04286, 117.84262, 0.1, 0.1);
        private double cameraX, cameraY, cameraZ; // camera coordinates

        private readonly GMap.NET.Internals.Core core = new GMap.NET.Internals.Core();
        private double lookX, lookY, lookZ; // camera look-at coordinates

        public Vector3 rpy = new Vector3();

        private readonly double step = 1 / 1200.0;
        private int texture;

        // image zoom level
        private int zoom = 14;

        public OpenGLtest()
        {
            instance = this;

            InitializeComponent();

            core.OnMapOpen();
        }

        public PointLatLngAlt LocationCenter
        {
            get => new PointLatLngAlt(area.LocationMiddle.Lat, area.LocationMiddle.Lng, _alt, "");
            set
            {
                if (area.LocationMiddle.Lat == value.Lat && area.LocationMiddle.Lng == value.Lng)
                    return;

                if (value.Lat == 0 && value.Lng == 0)
                    return;

                _alt = value.Alt;
                var size = 0.01;
                area = new RectLatLng(value.Lat + size, value.Lng - size, size * 2, size * 2);
                // Console.WriteLine(area.LocationMiddle + " " + value.ToString());
                Invalidate();
            }
        }

        private void getImage()
        {
            GMapProvider type = GoogleSatelliteMapProvider.Instance;
            var prj = type.Projection;

            //GMap.NET.GMaps.Instance.GetImageFrom();

            var startimage = DateTime.Now;

            if (!area.IsEmpty)
                try
                {
                    //string bigImage = zoom + "-" + type + "-vilnius.png";
                    //Console.WriteLine("Preparing: " + bigImage);
                    //Console.WriteLine("Zoom: " + zoom);
                    //Console.WriteLine("Type: " + type.ToString());
                    //Console.WriteLine("Area: " + area);
                    var types = type; // GMaps.Instance.GetAllLayersOfType(type);

                    // max zoom level
                    zoom = 20;

                    var topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                    var rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                    var pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);

                    // zoom based on pixel density
                    while (pxDelta.X > 2000)
                    {
                        zoom--;

                        // current area
                        topLeftPx = prj.FromLatLngToPixel(area.LocationTopLeft, zoom);
                        rightButtomPx = prj.FromLatLngToPixel(area.Bottom, area.Right, zoom);
                        pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
                    }

                    // get tiles - bg
                    core.Provider = type;
                    core.Position = LocationCenter;
                    core.Zoom = zoom;

                    // get type list at new zoom level
                    var tileArea = prj.GetAreaTileList(area, zoom, 0);

                    //this.Invalidate();
                    Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);

                    var padding = 0;
                    {
                        using (
                            var bmpDestination = new Bitmap((int)pxDelta.X + padding * 2, (int)pxDelta.Y + padding * 2)
                        )
                        {
                            Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                            using (var gfx = Graphics.FromImage(bmpDestination))
                            {
                                Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                gfx.CompositingMode = CompositingMode.SourceOver;
                                gfx.CompositingQuality = CompositingQuality.HighSpeed;
                                gfx.SmoothingMode = SmoothingMode.HighSpeed;

                                // get tiles & combine into one
                                foreach (var p in tileArea)
                                {
                                    Console.WriteLine("Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " +
                                                      tileArea.Count);

                                    foreach (var tp in type.Overlays)
                                    {
                                        Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                        var tile =
                                            ((PureImageCache)MyImageCache.Instance).GetImageFromCache(type.DbId, p,
                                                zoom) as GMapImage;

                                        //GMapImage tile = GMaps.Instance.GetImageFrom(tp, p, zoom, out ex) as GMapImage;
                                        //GMapImage tile = type.GetTileImage(p, zoom) as GMapImage;
                                        //tile.Img.Save(zoom + "-" + p.X + "-" + p.Y + ".bmp");
                                        if (tile != null)
                                            using (tile)
                                            {
                                                var x = p.X * prj.TileSize.Width - topLeftPx.X + padding;
                                                var y = p.Y * prj.TileSize.Width - topLeftPx.Y + padding;
                                                {
                                                    Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                                    gfx.DrawImage(tile.Img, x, y, prj.TileSize.Width,
                                                        prj.TileSize.Height);
                                                    Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                                                }
                                            }
                                    }
                                }
                            }

                            Console.WriteLine((startimage - DateTime.Now).TotalMilliseconds);
                            _terrain = new Bitmap(bmpDestination, 1024 * 2, 1024 * 2);


                            // _terrain.Save(zoom +"-map.bmp");
                            GL.BindTexture(TextureTarget.Texture2D, texture);

                            var data =
                                _terrain.LockBits(new Rectangle(0, 0, _terrain.Width, _terrain.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                            //Console.WriteLine("w {0} h {1}",data.Width, data.Height);
                            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height,
                                0,
                                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                            _terrain.UnlockBits(data);

                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                (int)TextureMinFilter.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                                (int)TextureMagFilter.Linear);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
        }

        public OpenTK.Vector3 Normal(OpenTK.Vector3 a, OpenTK.Vector3 b, OpenTK.Vector3 c)
        {
            var dir = OpenTK.Vector3.Cross(b - a, c - a);
            var norm = OpenTK.Vector3.Normalize(dir);
            return norm;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
                return;

            if (area.LocationMiddle.Lat == 0 && area.LocationMiddle.Lng == 0)
                return;

            _angle += 1f;

            // area.LocationTopLeft = new PointLatLng(area.LocationTopLeft.Lat + 0.0001,area.LocationTopLeft.Lng);

            //area.Size = new SizeLatLng(0.1, 0.1);

            try
            {
                base.OnPaint(e);
            }
            catch
            {
                return;
            }

            var heightscale = step / 90.0 * 1.3;

            var radians = (float)(Math.PI * (rpy.Z * -1) / 180.0f);

            //radians = 0;

            var mouseY = (float)0.1;

            cameraX = area.LocationMiddle.Lng; // multiplying by mouseY makes the
            cameraZ = area.LocationMiddle.Lat; // camera get closer/farther away with mouseY
            cameraY = LocationCenter.Alt < srtm.getAltitude(cameraZ, cameraX, 20).alt
                ? (srtm.getAltitude(cameraZ, cameraX, 20).alt + 0.2) * heightscale
                : LocationCenter.Alt * heightscale; // (srtm.getAltitude(lookZ, lookX, 20) + 100) * heighscale;


            lookX = area.LocationMiddle.Lng + Math.Sin(radians) * mouseY;
            ;
            lookY = cameraY;
            lookZ = area.LocationMiddle.Lat + Math.Cos(radians) * mouseY;
            ;


            MakeCurrent();


            GL.MatrixMode(MatrixMode.Projection);

            var projection = Matrix4.CreatePerspectiveFieldOfView((float)(100 * MathHelper.deg2rad), 1f, 0.00001f,
                (float)step * 50);
            GL.LoadMatrix(ref projection);

            var modelview = Matrix4.LookAt((float)cameraX, (float)cameraY, (float)cameraZ, (float)lookX,
                (float)lookY, (float)lookZ, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);

            // roll
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationZ((float)(rpy.X * MathHelper.deg2rad)));
            // pitch
            modelview = Matrix4.Mult(modelview, Matrix4.CreateRotationX((float)(rpy.Y * -MathHelper.deg2rad)));

            GL.LoadMatrix(ref modelview);

            GL.ClearColor(Color.LightBlue);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.LightModel(LightModelParameter.LightModelAmbient, new[] { 1f, 1f, 1f, 1f });

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            /*
            GL.Begin(BeginMode.LineStrip);

            GL.Color3(Color.White);
            GL.Vertex3(0, 0, 0);

            GL.Vertex3(area.Bottom, 0, area.Left);

            GL.Vertex3(lookX, lookY, lookZ);

            //GL.Vertex3(cameraX, cameraY, cameraZ);

            GL.End();
            */
            var sw = new Stopwatch();

            sw.Start();

            //zoom = 14;

            getImage();

            sw.Stop();

            Console.WriteLine("img " + sw.ElapsedMilliseconds);

            sw.Start();

            var increment = step * 1;

            var cleanup = area.Bottom % increment;
            var cleanup2 = area.Left % increment;

            for (var z = area.Bottom - cleanup; z < area.Top - step; z += increment)
            {
                //Makes OpenGL draw a triangle at every three consecutive vertices
                GL.Begin(PrimitiveType.TriangleStrip);
                for (var x = area.Left - cleanup2; x < area.Right - step; x += increment)
                {
                    var heightl = srtm.getAltitude(z, area.Right + area.Left - x, 20).alt;

                    //  Console.WriteLine(x + " " + z);

                    GL.Color3(Color.White);


                    //  int heightl = 0;

                    var scale2 = Math.Abs(x - area.Left) / area.WidthLng; // / (float)_terrain.Width;

                    var scale3 = Math.Abs(z - area.Bottom) / area.HeightLat; // / (float)_terrain.Height;

                    var imgx = 1 - scale2;
                    var imgy = 1 - scale3;
                    //GL.Color3(Color.Red);

                    //GL.Color3(_terrain.GetPixel(imgx, imgy));
                    GL.TexCoord2(imgx, imgy);
                    GL.Vertex3(x, heightl * heightscale, z); //  _terrain.GetPixel(x, z).R

                    try
                    {
                        heightl = srtm.getAltitude(z + increment, area.Right + area.Left - x, 20).alt;

                        //scale2 = (Math.Abs(x - area.Left) / area.WidthLng) * (float)_terrain.Width;

                        scale3 = Math.Abs(z + increment - area.Bottom) / area.HeightLat;
                        // / (float)_terrain.Height;

                        imgx = 1 - scale2;
                        imgy = 1 - scale3;
                        // GL.Color3(Color.Green);
                        //GL.Color3(_terrain.GetPixel(imgx, imgy));
                        GL.TexCoord2(imgx, imgy);
                        GL.Vertex3(x, heightl * heightscale, z + increment);

                        //  Console.WriteLine(x + " " + (z + step));
                    }
                    catch
                    {
                        break;
                    }
                }

                GL.End();
            }

            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            GL.Flush();

            sw.Stop();

            Console.WriteLine("GL  " + sw.ElapsedMilliseconds);

            try
            {
                SwapBuffers();

                Context.MakeCurrent(null);
            }
            catch
            {
            }

            //this.Invalidate();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // OpenGLtest
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            Name = "OpenGLtest";
            Load += test_Load;
            Resize += test_Resize;
            ResumeLayout(false);
        }

        private void test_Load(object sender, EventArgs e)
        {
            GL.GenTextures(1, out texture);

            GL.Enable(EnableCap.DepthTest);
            // GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);

            //GL.Enable(EnableCap.LineSmooth);
            //GL.Enable(EnableCap.PointSmooth);
            //GL.Enable(EnableCap.PolygonSmooth);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
        }

        private void test_Resize(object sender, EventArgs e)
        {
            MakeCurrent();

            GL.Viewport(0, 0, Width, Height);

            Invalidate();
        }
    }
}