using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Simulacion_Agentes
{
    class Program
    {
        static StreamWriter write;
        static void Main(string[] args)
        {
            //Reactivo
            //Proactivo
            //Simulacion(int N, int M, int sucias, int obstaculos, int niños, int t, params string[] robots);
            //Simula(1, 6, 5, 10, 10, 1, 10, "Proactivo");
            //Simula(2, 6, 5, 10, 10, 2, 10, "Proactivo");
            //Simula(3, 15, 15, 25, 30, 10, 10, "Proactivo");
            //Simula(4, 7, 5, 10, 20, 3, 10, "Proactivo", "Proactivo");
            //Simula(5, 7, 5, 10, 20, 5, 10, "Proactivo", "Proactivo", "Proactivo");
            //Simula(6, 6, 5, 10, 20, 2, 10, "Proactivo");
            //Simula(7, 5, 3, 5, 10, 1, 10, "Reactivo");
            //Simula(8, 5, 3, 5, 2, 2, 15, "Reactivo");
            //Simula(9, 5, 3, 5, 2, 3, 15, "Reactivo", "Reactivo");
            //Simula(10, 5, 6, 5, 10, 2, 15, "Reactivo");

            Console.ReadLine();
        }

        static void Simula(int numero, int N, int M, int sucias, int obstaculos, int niños, int t, params string[] robots) 
        {
            string name;
            int gano = 0;
            int despedido = 0;
            double cantsucias = 0;
            Simulacion sim;

            for (int vez = 1; vez <= 30; vez++)
            {
                sim = new Simulacion(N, M, sucias, obstaculos, niños, t, robots);
                name = "Ambiente " + numero +" Simulacion " + vez+".txt";
                write = new StreamWriter(name);
                
                sim.Comienza();

                Queue<string> acciones = sim.Acciones;

                foreach (var item in acciones)
                {
                  write.WriteLine(item);
                }

                if (sim.gano) gano++;
                if (sim.perdio) despedido++;
                cantsucias += sim.casillas_sucias;

                write.WriteLine("Termina Simulacion");
                write.Close();
            }

            Console.WriteLine("El porciento medio de casillas sucias fue: " + cantsucias / 30);
            Console.WriteLine("El robot fue despedido " + despedido + " veces");
            Console.WriteLine("El robot limpio la casa y coloco los niños " + gano + " veces");

            name = "Resumen Ambiente " + numero + ".txt";
            write = new StreamWriter(name);
            write.WriteLine("El porciento medio de casillas sucias fue: " + cantsucias / 30);
            write.WriteLine("El robot fue despedido " + despedido + " veces");
            write.WriteLine("El robot limpio la casa y coloco los niños " + gano + " veces");
            write.Close();
        }
    }
}
