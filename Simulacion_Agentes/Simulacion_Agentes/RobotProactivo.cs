using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_Agentes
{
    class RobotProactivo : Robot
    {
        int[] dirx = { 0, 0, 1, 1, -1, -1 };
        int[] diry = { 1, -1, 0, 1, -1, 0 };
        public RobotProactivo(int x, int y) : base(x, y) { }
        public override void Juega(ref Mundo mundo)
        {
            if (cargaNiño)
            {
                if (mundo.Casillas[X, Y] == Elemento.RobotYCorral) DejaNiño(ref mundo);
                else
                {
                    Tuple<int, int> obj = MejorObjetivo(mundo, mundo.PosCorral);
                    Tuple<int, int> pos = ProximaJugada(mundo, obj);
                    EfectuaJugada(ref mundo, pos);
                }
            }
            else
            {
                Tuple<int, int> pos = new Tuple<int, int>(X, Y);
                if (mundo.Casillas[X, Y] == Elemento.SuciedadYRobot) Limpia(ref mundo);
                else EfectuaJugada(ref mundo, Proxima(mundo));
            }
        }
        public Tuple<int, int> Proxima(Mundo mundo)
        {
            Tuple<int, int> obj1 = null;
            Tuple<int, int> obj2 = null;
            double d1 = double.MaxValue;
            double d2 = double.MaxValue;

            if (mundo.PosSuciedad.Count != 0) obj1 = MejorObjetivo(mundo, mundo.PosSuciedad);
            if (mundo.PosNiños.Count != 0) obj2 = MejorObjetivo(mundo, mundo.PosNiños);
            if (obj1 != null && (obj1.Item1 != X || obj1.Item2 != Y)) d1 = Distancia(X, Y, obj1.Item1, obj1.Item2);
            if (obj2 != null && (obj2.Item1 != X || obj2.Item2 != Y)) d2 = Distancia(X, Y, obj2.Item1, obj2.Item2);

            if (d1 == double.MaxValue && d2 == double.MaxValue) return new Tuple<int, int>(X, Y);
            else if (d1 < d2) return ProximaJugada(mundo, obj1);
            else return ProximaJugada(mundo, obj2);
        }
    }
}
