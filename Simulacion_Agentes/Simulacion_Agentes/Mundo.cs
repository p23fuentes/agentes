using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_Agentes
{
    class Mundo
    {
        Random Random = new Random();
        public int N { get; set; }
        public int M { get; set; }
        public Queue<string> Acciones = new Queue<string>();
        int totalSucias;
        int totalNiños;
        int obstaculos;
        public int cantNiños { get { return PosNiños.Count(); } } //Cantidad de niños que no estan ni en el corral ni en un robot.
        public double casillas_sucias { get { return PosSuciedad.Count(); }}
        public double casillas_vacias { get { return ((N * M) - PosNiños.Count() - obstaculos - totalNiños - Robots.Count()); } }
        public double suciedad { get { return ((casillas_sucias / casillas_vacias)* 100);}}
        public Elemento[,] Casillas { get; set; }
        public bool[,] Ocupadas { get; set; }
        public List<Tuple<int, int>> PosNiños { get; set; }
        public List<Tuple<int, int>> PosSuciedad { get; set; }
        public List<Tuple<int, int>> PosObstaculo { get; set; }
        public List<Tuple<int, int>> PosCorral { get; set; }
        public List<Robot> Robots { get; set; }
        public Mundo(int dimensionX, int dimensionY, int niños, int sucias, int obstaculos, params string[] robots)
        {
            N = dimensionX;
            M = dimensionY;
            this.obstaculos = obstaculos;
            totalNiños = niños;
            Casillas = new Elemento[N, M];
            Ocupadas = new bool[N, M];
            PosNiños = new List<Tuple<int, int>>();
            PosSuciedad = new List<Tuple<int, int>>();
            PosObstaculo = new List<Tuple<int, int>>();
            PosCorral = new List<Tuple<int, int>>();
            Robots = new List<Robot>();
            totalSucias = sucias;
            AmbienteInicial(sucias, obstaculos, robots);
        }

        #region AmbienteInicial
        protected void AmbienteInicial(int sucias, int obstaculos, params string[] robots)
        {
              //poner Corral de primero
              Acciones.Enqueue("Creando corral");
              PonerCorral(totalNiños,0);
              //poner niños
              Acciones.Enqueue("Creando ninos");
              PonerElemento(totalNiños, Elemento.Niño);
              //poner suciedad
              Acciones.Enqueue("Creando suciedad inicial");
              PonerElemento(sucias, Elemento.Suciedad);
              //poner obstaculos
              Acciones.Enqueue("Creando obstaculos");
              PonerElemento(obstaculos, Elemento.Obstaculo);
              //poner robots
              Acciones.Enqueue("Ubicando Robots");
              PonerRobot(robots);
        }
        protected void PonerRobot(params string[] robots) 
        {
            int robot = robots.Length;
            int p = 0;

            int x;
            int y;
            do
            {
                x = Random.Next(0, N);
                y = Random.Next(0, M);

                if (!Ocupadas[x, y])
                {
                    Ocupadas[x, y] = true;
                    Casillas[x, y] = Elemento.Robot;

                    switch (robots[p]) //crear Robot
                    {
                        case "Reactivo": Robots.Add(new RobotReactivo(x, y)); break;
                        case "Proactivo": Robots.Add(new RobotProactivo(x, y)); break;
                    }
                    
                    p++;
                    robot--;
                    Acciones.Enqueue("Se ubica el Robot en la posicion ("+x+","+y+")");
                }
            } while (robot>0);
        
        }

        #region Poner Corral
        protected void PonerCorral(int cantS, int cantD)
        {
            int x;
            int y;
            do
            {
              x = Random.Next(0, N);
              y = Random.Next(0, M);
            } while (Ocupadas[x, y]);

            PonerCorral(x, y, cantS-1, cantD-1);
        }
        protected void PonerCorral(int x, int y, int cantS, int cantD)
        {
            int a = x;
            int b = y;
            bool vacio = ProximoCorral(x, y, ref a, ref b);

            if (cantS >= 0 && vacio)
            {
                Casillas[a, b] = Elemento.Corral;
                Ocupadas[a, b] = true;
                PosCorral.Add(new Tuple<int, int>(a, b));
                Acciones.Enqueue("Se ubica corral vacio en la posicion (" + a + "," + b + ")");
                int c = cantS;
                c--;
                PonerCorral(a, b, c, cantD);
            }
            else if (cantD >= 0 && vacio)
            {
                Casillas[a, b] = Elemento.CorralOcupado;
                Ocupadas[a, b] = true;
                Acciones.Enqueue("Se ubica corral ocupado en la posicion (" + a + "," + b + ")");
                int c = cantD;
                c--;
                PonerCorral(a, b, cantS, c);
            }
            else if (!vacio && (cantS > -1 || cantD > -1))
            {
                PonerCorral(a, b, cantS, cantD);
            }
        }

        int[] dirx = { 0, 0, 1, 1, -1, -1 };
        int[] diry = { 1, -1, 0, 1, -1, 0 };
        public bool ProximoCorral(int x, int y, ref int a, ref int b)
        {
            int d=x;
            int e=y; 
            for (int i = 0; i < 6; i++)
            {
                int A = x + dirx[i];
                int B = y + diry[i];

                if(Valida(A, B))
                {
                  if (Ocupadas[A, B] == false)
                  {
                    a = A; b = B;
                    return true;
                  }
                  else if (Casillas[A, B] == Elemento.Corral || Casillas[A, B] == Elemento.CorralOcupado) 
                  {
                      d = A; e = B;
                  }
                }
            }

            a = d; b = e;
            return false;
        }

        #endregion

        protected void PonerElemento(int cant, Elemento element)
        {
            int x;
            int y;
            do
            {
                x = Random.Next(0, N);
                y = Random.Next(0, M);

                if (!Ocupadas[x, y])
                {
                    Ocupadas[x, y] = true;
                    Casillas[x, y] = element;

                    switch (element)
                    {
                        case Elemento.Niño:
                            {
                                PosNiños.Add(new Tuple<int, int>(x, y));
                                Acciones.Enqueue("Se ubica niño en la posicion (" + x + "," + y + ")");
                            } break;
                        case Elemento.Suciedad:
                            {
                                PosSuciedad.Add(new Tuple<int, int>(x, y));
                                Acciones.Enqueue("Se crea suciedad en la posicion (" + x + "," + y + ")");
                            }
                            break;
                        case Elemento.Obstaculo:
                            {
                                PosObstaculo.Add(new Tuple<int, int>(x, y));
                                Acciones.Enqueue("Se ubica obstaculo en la posicion (" + x + "," + y + ")");
                            }
                            break;
                        default:
                            break;
                    }
                    cant--;
                }

            } while (cant > 0 && HayEspacio());
        }

        bool HayEspacio() 
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (Ocupadas[i, j] == false) return true;
                }
            }
            return false;
        }

        #endregion
        public bool Valida(int x, int y){ return (0<=x && x<N && 0<=y && y<M);}
        public void CambioNatural() 
        {
            Acciones.Enqueue("Ocurre un cambio Natural De Ambiente");
            //Mando a mover a los niños
            MueveNiños();
            //Creo la suciedad
            if (cantNiños == 1) CreaSuciedad(1);
            else if (cantNiños == 2) CreaSuciedad(3);
            else if (cantNiños >= 3) CreaSuciedad(6);
        }

        #region Mover Niños
        protected void MueveNiños()
        {
            List<Tuple<int, int>> posnew = new List<Tuple<int, int>>();
            Tuple<int, int> dir;

            foreach (var niño in PosNiños)
            {
                dir = new Tuple<int, int>(Random.Next(-1, 2), Random.Next(-1, 2));
                int x = niño.Item1 + dir.Item1;
                int y = niño.Item2 + dir.Item2;

                if (Se_Mueve(niño, dir) && (dir.Item1 != 0 || dir.Item2 != 0))
                {
                     if (Casillas[x, y] == Elemento.Obstaculo)
                    {
                        int a = x;
                        int b = y;

                        do
                        {
                            a += dir.Item1;
                            b += dir.Item2;

                        } while (Ocupadas[a, b]);

                        Casillas[a, b] = Elemento.Obstaculo;
                        PosObstaculo.Add(new Tuple<int, int>(a, b));
                        PosObstaculo.Remove(new Tuple<int, int>(x, y));
                        Ocupadas[a, b] = true;
                    }

                    int X = niño.Item1;
                    int Y = niño.Item2;

                    Casillas[x, y] = Elemento.Niño;
                    Ocupadas[x, y] = true;

                    //Puede que en el turno anterior el robot lo halla dejado
                    //y por tanto estan juntos
                    if (Casillas[X, Y] != Elemento.NiñoYRobot)
                    {
                        Casillas[X, Y] = Elemento.Vacia;
                        Ocupadas[X, Y] = false;
                    }
                    else Casillas[X, Y] = Elemento.Robot;

                    Acciones.Enqueue("Se mueve niño de la posicion (" + niño.Item1 + "," + niño.Item2 + ") a la posicion (" + x + "," + y + ")");
                    posnew.Add(new Tuple<int, int>(x, y));
                }
                else
                {
                    Acciones.Enqueue("El niño de la posicion (" + niño.Item1 + "," + niño.Item2 + ") no se mueve");
                    posnew.Add(niño);
                }
            }

            PosNiños = posnew;
        }
        protected bool Se_Mueve(Tuple<int, int> posicion, Tuple<int, int> dir)
        {
            int x = posicion.Item1 + dir.Item1;
            int y = posicion.Item2 + dir.Item2;

            if (Valida(x, y))
            {
                if (Ocupadas[x, y] == false) return true;
                else if (Casillas[x, y] == Elemento.Obstaculo) return Se_Mueve(new Tuple<int, int>(x, y), dir); //puede seguir moviendo obstaculos?
                else return false;
            }
            else return false;
        }

        #endregion
        protected void CreaSuciedad(int cant)
        {
           int x = Random.Next(0,cant+1);
           totalSucias += x;
           PonerElemento(x, Elemento.Suciedad);
        }
        public void CambioAleatorio() 
        {
            Acciones.Enqueue("Ocurre un Cambio Aleatorio De Ambiente");
                    
            Ocupadas = new bool[N,M];
            Casillas = new Elemento[N,M];
            int corral = PosCorral.Count;
            int sucio = PosSuciedad.Count;
            int cantN = PosNiños.Count;
            PosCorral.Clear();
            PosNiños.Clear();
            PosObstaculo.Clear();
            PosSuciedad.Clear();

            foreach (var item in Robots)
	       {
		     Ocupadas[item.X,item.Y]=true;
             Casillas[item.X,item.Y]=Elemento.Robot;
	       }
      
              Acciones.Enqueue("Reubicando elementos"); 
              PonerCorral(corral, totalNiños - corral);
              PonerElemento(obstaculos, Elemento.Obstaculo);
              PonerElemento(sucio, Elemento.Suciedad);
              PonerElemento(cantN, Elemento.Niño);
        }
    }
}
