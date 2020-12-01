using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_Agentes
{
    class RobotReactivo : Robot
    {
        Tuple<int, int> obj;
        Tuple<int, int> mueve;
        public RobotReactivo(int x, int y) : base(x, y) { }
        public override void Juega(ref Mundo mundo)
        {
            if (mundo.cantNiños == 0) //el corral esta lleno o tengo un niño
            {
                if (cargaNiño)
                {
                    if (mundo.Casillas[X, Y] == Elemento.RobotYCorral) DejaNiño(ref mundo);
                    else
                    {
                        obj = MejorObjetivo(mundo, mundo.PosCorral); //busca el corral
                        mueve = ProximaJugada(mundo, obj);
                        EfectuaJugada(ref mundo, mueve);
                    }
                }
                else
                {
                    if (mundo.Casillas[X, Y] == Elemento.SuciedadYRobot) Limpia(ref mundo);
                    else
                    {
                        obj = MejorObjetivo(mundo, mundo.PosSuciedad); //busca la suciedad
                        mueve = ProximaJugada(mundo, obj);
                        EfectuaJugada(ref mundo, mueve);
                    }
                }

            }
            else //al menos hay dos niño suelto
            {
                if (mundo.suciedad < 45) //el objetivo principal es reducir los niños sueltos
                {
                    if (mundo.Casillas[X, Y] == Elemento.SuciedadYRobot && !cargaNiño)
                    {
                        //estoy en una casilla sucia
                        Limpia(ref mundo);
                    }
                    else
                    {
                        if (cargaNiño) //si ya lo cargo o busco el corral o lo dejo en el corral 
                        {
                            if (mundo.Casillas[X, Y] == Elemento.RobotYCorral)
                            {
                                DejaNiño(ref mundo); //estoy en el corral
                            }
                            else
                            {   //busco el corral
                                obj = MejorObjetivo(mundo, mundo.PosCorral);
                                mueve = ProximaJugada(mundo, obj);
                                EfectuaJugada(ref mundo, mueve);
                            }
                        }
                        else
                        {   //busco el niño mas cerca
                            obj = MejorObjetivo(mundo, mundo.PosNiños);
                            mueve = ProximaJugada(mundo, obj);
                            EfectuaJugada(ref mundo, mueve);
                        }
                    }
                }
                else //el objetivo principal es reducir la suciedad
                {
                    Tuple<int, int> objS = MejorObjetivo(mundo, mundo.PosSuciedad);
                    if (cargaNiño)
                    {
                        if (mundo.Casillas[X, Y] == Elemento.RobotYCorral) DejaNiño(ref mundo);
                        else
                        {
                            Tuple<int, int> objC = MejorObjetivo(mundo, mundo.PosCorral);
                            double d1 = Distancia(X, Y, objC.Item1, objC.Item2); //distancia a el corral
                            double d2 = Distancia(X, Y, objS.Item1, objS.Item2); //distancia a la suciedad

                            if (d1 / 2 > d2 && mundo.Casillas[X, Y] == Elemento.Robot)
                            {
                                DejaNiño(ref mundo);
                            }
                            else
                            {
                                mueve = ProximaJugada(mundo, objC);
                                EfectuaJugada(ref mundo, mueve);
                            }
                        }
                    }
                    else
                    {
                        if (mundo.Casillas[X, Y] == Elemento.SuciedadYRobot) Limpia(ref mundo);
                        else
                        {
                            mueve = ProximaJugada(mundo, objS);
                            EfectuaJugada(ref mundo, mueve);
                        }
                    }
                }
            }
        }
    }
    }

