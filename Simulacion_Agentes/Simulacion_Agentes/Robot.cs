using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_Agentes
{
   abstract class Robot
    {
        public string Accion { get; set;}
        public int X;
        public int Y;
        protected bool cargaNiño;
        public Robot(int x, int y)
        {
            X = x;
            Y = y;
        }
        public virtual void Juega(ref Mundo mundo) 
        {
        }
        protected void DejaNiño(ref Mundo mundo)
        {
            if (mundo.Casillas[X, Y] == Elemento.RobotYCorral) 
            {
              mundo.Casillas[X, Y] = Elemento.CorralOcupado;
              mundo.PosCorral.Remove(new Tuple<int, int>(X, Y));
              Accion = "El Robot deja al niño en el corral de la posicion (" + X + "," + Y + ")";
            }
            else 
            {
              mundo.Casillas[X, Y] = Elemento.NiñoYRobot;
              mundo.PosNiños.Add(new Tuple<int, int>(X, Y));
              Accion = "El Robot deja al niño en la posicion (" + X + "," + Y + ")";
            }
            cargaNiño = false;
        }
        protected void Limpia(ref Mundo mundo) 
        {
            mundo.PosSuciedad.Remove(new Tuple<int, int>(X,Y));
            mundo.Casillas[X, Y] = Elemento.Robot;
            Accion = "El Robot limpia la suciedad de la posicion (" + X + "," + Y + ")";
        }
        protected void EfectuaJugada(ref Mundo mundo, Tuple<int, int> pos)
        {
            int a = pos.Item1;
            int b = pos.Item2;

            if (a != X || b != Y)
            {
                Elemento hay = mundo.Casillas[a, b];

                //A donde llego
                switch (hay)
                {
                    case Elemento.Vacia:
                        {
                            mundo.Casillas[a, b] = Elemento.Robot;
                            mundo.Ocupadas[a, b] = true;
                            Accion = "El Robot se mueve a la posicion (" + X + "," + Y + ")";
                        }
                        break;
                    case Elemento.Niño:
                        {
                            mundo.Casillas[a, b] = Elemento.Robot;
                            cargaNiño = true; //lo carga automaticamente
                            mundo.PosNiños.Remove(new Tuple<int, int>(a, b));
                            Accion = "El Robot se mueve y carga al niño de la posicion (" + a + "," + b + ")";
                        }
                        break;
                    case Elemento.Suciedad: 
                        { 
                            mundo.Casillas[a, b] = Elemento.SuciedadYRobot; 
                            Accion = "El Robot se mueve a la posicion (" + a + "," + b + ")"+" donde hay suciedad"; 
                        }
                        break;
                    case Elemento.Corral: 
                        {
                            mundo.Casillas[a, b] = Elemento.RobotYCorral;
                            Accion = "El Robot llega al corral de la posicion (" + a + "," + b + ")";
                        }
                        break;
                    case Elemento.CorralOcupado:
                        {
                            mundo.Casillas[a, b] = Elemento.RobotYCorral;
                            cargaNiño = true;
                            mundo.PosCorral.Add(new Tuple<int, int>(a, b));
                            Accion = "El Robot llega al corral de la posicion (" + a + "," + b + ")" + " y carga al niño";
                        }
                        break;

                    default:
                        break;
                }
                //donde estaba

                switch (mundo.Casillas[X, Y])
                {
                    case Elemento.Robot:
                        {
                            mundo.Casillas[X, Y] = Elemento.Vacia;
                            mundo.Ocupadas[X, Y] = false;
                        }
                        break;
                    case Elemento.SuciedadYRobot: mundo.Casillas[X, Y] = Elemento.Suciedad;
                        break;
                    case Elemento.NiñoYRobot: mundo.Casillas[X, Y] = Elemento.Niño;
                        break;
                    case Elemento.RobotYCorral: mundo.Casillas[X, Y] = Elemento.Corral;
                        break;
                    default:
                        break;
                }
                X = a;
                Y = b;
            }
            else
            {
                Accion = "El Robot no se mueve";
            }
        }
        protected Tuple<int, int> MejorObjetivo(Mundo mundo, List<Tuple<int, int>> lista) 
        {
            Tuple<int, int> resp=new Tuple<int,int>(X,Y);
            double d;
            double mejor = int.MaxValue;

            foreach (var item in lista)
            {
                d = Distancia(X, Y, item.Item1, item.Item2);
                if (mejor > d) 
                {
                    mejor = d;
                    resp = item;
                }
            }
            return resp;
        }
        protected Tuple<int, int> ProximaJugada(Mundo mundo, Tuple<int, int> obj) 
        {
            List<Tuple<int, int>> posibles = PosiblesMovimientos(mundo);

            if (cargaNiño)
            {
                int t = posibles.Count;
                List<Tuple<int, int>> aux;
                for (int i = 0; i < t; i++)
                {
                    aux = PosiblesMovimientos(mundo, posibles[i]);
                    foreach (var item in aux)
                    {
                        if (!posibles.Contains(item)) posibles.Add(item);
                    }
                }
            }

            Tuple<int,int> proximo = new Tuple<int,int>(X,Y);

            double mejor = Double.MaxValue;

            foreach (var item in posibles)
            {
                double d=Distancia(item.Item1,item.Item2,obj.Item1,obj.Item2);
                if (d < mejor) 
                {
                    mejor = d;
                    proximo = item;
                }
            }
            return proximo;
        }
        protected List<Tuple<int, int>> PosiblesMovimientos(Mundo mundo) 
        { 
            return PosiblesMovimientos(mundo, new Tuple<int, int>(X,Y));
        }
        protected List<Tuple<int, int>> PosiblesMovimientos(Mundo mundo, Tuple<int, int> P) 
        {
            List<Tuple<int, int>> posibles = new List<Tuple<int, int>>();
            int a = P.Item1;
            int b = P.Item2;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (mundo.Valida(a + i, b + j))
                    {
                        if (mundo.Casillas[a + i, b + j] == Elemento.Vacia) posibles.Add(new Tuple<int, int>(a + i, b + j));
                        else if (mundo.Casillas[a + i, b + j] == Elemento.Suciedad) posibles.Add(new Tuple<int, int>(a + i, b + j));
                        else if (mundo.Casillas[a + i, b + j] == Elemento.Corral) posibles.Add(new Tuple<int, int>(a + i, b + j));
                        else if (!cargaNiño) 
                        {
                           if (mundo.Casillas[a + i, b + j] == Elemento.Niño) posibles.Add(new Tuple<int, int>(a + i, b + j));
                        }
                    }
                }
            }
            return posibles;
        }
        protected double Distancia(int x, int y, int a, int b)
        {
            return Math.Sqrt((Math.Pow(x - a, 2) + Math.Pow(y - b, 2)));
        }
   }
}
