using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_Agentes
{
    class Simulacion
    {
       public Queue<string> Acciones = new Queue<string>();

        Mundo mundo;
        int tiempo;
        int t;
        int time=0;
        public bool gano = false;
        public bool perdio = false;
        public double casillas_sucias;

        public Simulacion(int N, int M, int sucias, int obstaculos, int niños, int t, params string[] robots) 
        {
            int T = N*M-2*niños-robots.Length;
            int sucia=(sucias*T)/100;
            int obst=(obstaculos*T)/100;
            mundo = new Mundo(N, M, niños, sucia, obst, robots);
            this.t = t;
            foreach (var item in mundo.Acciones)
            {
              Acciones.Enqueue(item);
            }
            mundo.Acciones.Clear();
        }
        public void Comienza() 
        {         
         do
         {
             foreach (var robot in mundo.Robots)
             {
               robot.Juega(ref mundo);
               Acciones.Enqueue(robot.Accion);
             }

             mundo.CambioNatural();
             
             if (t == time) { time = 0; mundo.CambioAleatorio(); }
             tiempo++;
             time++;
             foreach (var item in mundo.Acciones)
             {
                 Acciones.Enqueue(item);
             }
             mundo.Acciones.Clear();
         } while(!EstadoFinal());

         casillas_sucias = (mundo.casillas_sucias*100)/(mundo.N*mundo.M);
        }
        public bool EstadoFinal() 
        {
            if (mundo.suciedad >= 60) 
            {
                Acciones.Enqueue("Se despide al robot."); 
                perdio = true;
                return true; 
            
            }
            else if (mundo.cantNiños == 0 && mundo.casillas_sucias == 0) { Acciones.Enqueue("El robot cumplio con su objetivo."); gano = true; return true; }
            else if (tiempo > 100 * t) return true;
            else return false;
        }
    }
}
