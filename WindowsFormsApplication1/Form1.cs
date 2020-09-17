 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Material chainikMaterial;
        Device d3d;
        Mesh chainik;
        float koor;
        float t;
        float angle;
        Mesh[] objects = new Mesh[10];                // Модели вращающихся объектов
        Material[] objectMaterials = new Material[3]; // Материал объектов
        const double OrbitRadius = 1.5;    // Радиус орбиты вращения вокруг чайника
        const double RotationFreq = 0.4; // Частота вращения по орбите…

        public Form1()
        {
            InitializeComponent();
            d3d = null;
            chainik = null;
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            koor = 5.0f;
            t = 0;
            angle = 0;
            
        }

 
        public void OnIdle(object sender, EventArgs e)
        {
            Invalidate(); // Помечаем главное окно (this) как требующее перерисовки
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                PresentParameters d3dpp = new PresentParameters();
                d3dpp.BackBufferCount = 1;
                d3dpp.SwapEffect = SwapEffect.Discard;
                d3dpp.Windowed = true; // Выводим графику в окно
                d3dpp.MultiSample = MultiSampleType.None; // Выключаем антиалиасинг
                d3dpp.EnableAutoDepthStencil = true; // Разрешаем создание z-буфера
                d3dpp.AutoDepthStencilFormat = DepthFormat.D16; // Z-буфер в 16 бит
                d3d = new Device(0, // D3D_ADAPTER_DEFAULT - видеоадаптер по 
                    // умолчанию
                      DeviceType.Hardware, // Тип устройства - аппаратный ускоритель
                      this, // Окно для вывода графики
                      CreateFlags.SoftwareVertexProcessing,
                      d3dpp);
            }
            catch (Exception exc)
            {
                MessageBox.Show(this, exc.Message, "Ошибка инициализации");
                Close(); // Закрываем окно            }        
            }
            chainik = Mesh.Teapot(d3d);
            chainikMaterial = new Material();
            chainikMaterial.Diffuse = Color.Yellow;
            chainikMaterial.Specular = Color.White;
            // Создаем модели объектов, вращающихся вокруг чайника
            // и задаем свойства материалов
            for (int i = 0; i < objects.Length; i++)
                objects[i] = Mesh.Sphere(d3d,
                                         0.05f, // Радиус сферы
                                         10,     // Количество параллелей
                                         10);    // Количество меридианов
            objectMaterials[0].Diffuse = Color.Red;
            objectMaterials[1].Diffuse = Color.LightGreen;
            objectMaterials[2].Diffuse = Color.LightBlue;

        }

        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
               
            // Очищаем буфер глубины и дублирующий буфер
            d3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Green, 1.0f, 0);
            d3d.BeginScene();
            SetupCamera();

            // Включаем отсечение нелицевых граней
            d3d.RenderState.CullMode = Cull.CounterClockwise;

            // В цикле рисуем все объекты
            int numObjects = objects.Length;
            for (int i = 0; i < numObjects; i++)
            {
                // Задаем следующее преобразование координат:
                // Сдвигаем объект на радиус орбиты,
                // вращаем объект на зависящий от времени угол,
                // поворачиваем плоскость вращения на угол, зависящий от номера
                // объекта и совмещаем центр орбит объектов с центром чайника
                double a = i / (double)numObjects;
                d3d.Transform.World = Matrix.Translation(0, 0, (float)OrbitRadius) *
                Matrix.RotationY((float)(2 * Math.PI * (a + t * RotationFreq))) *
                Matrix.RotationZ((float)(Math.PI * a)) * Matrix.Translation(0, 0,
 3.5f);
                // Задаем материал и рисуем i-й объект
                d3d.Material = objectMaterials[i % objectMaterials.Length];
                objects[i].DrawSubset(0);
            }


            // Отключаем отсечение нелицевых граней
            d3d.RenderState.CullMode = Cull.None;
            d3d.Transform.World = Matrix.Translation(0, 0, 5f);
            d3d.Material = chainikMaterial;  
          

            
            
            chainik.DrawSubset(0);
            d3d.EndScene();
            d3d.Present();
            t += 0.01f;
            koor = 5.0f+ (float)Math.Cos(t);
            angle += 0.05f;
            //this.Invalidate();
        }

        private void SetupCamera()
        {
           d3d.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1.0f, 50.0f);
           d3d.Lights[0].Enabled = true;   // Включаем нулевой источник освещения
           d3d.Lights[0].Diffuse = Color.White;// Цвет источника освещения
           d3d.Lights[0].Position = new Vector3(0, 0, 0); // Задаем координаты
        }
    }
}