using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Создаем объект-окно
            Form1 mainForm = new Form1();
            // Cвязываем метод OnIdle с событием Application.Idle
            Application.Idle += new EventHandler(mainForm.OnIdle);
            // Показываем окно и запускаем цикл обработки сообщений
            Application.Run(mainForm);
        }
    }
}
