using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;


namespace Combinacion_assignatras_creditos
{
    class Program
    {
        
        public class Assignatura
        {
            public string Siglas;
            public string Nombre;
            public double creditos;

            
        }
        public class Condiciones
        {
            public Assignatura Assignatura;
            public List<Assignatura> AssCorequisit=new List<Assignatura>();
        }
        public static Assignatura InsertarAss(string S, string N, double C)
        {
            Assignatura A = new Assignatura();
            A.Siglas = S;
            A.Nombre = N;
            A.creditos = C;
            return A;
        }
        public static Condiciones InsertarCond(Assignatura A, List<Assignatura> C)
        {
            Condiciones Cond = new Condiciones();
            Cond.Assignatura = A;
            Cond.AssCorequisit = C;
            return Cond;

        }
        public static List<List<Assignatura>> Combinaciones(List<Assignatura> ListAssF, int NumAssignatruas, int creditosMin,List<Condiciones> ListCond)
        {
            // El algoritmo de combinacion es
            // 1000      Primer bloque        
            // 0100
            // 0010
            // 0001

            // 1000     Segundo bloque
            // 1100     En todos los grupos anteriores (lineas anterories)
            // 1010       se añade el elemento de la primera columna
            // 1001
            // 1100

            // 0100     Segundo bloque
            // 0110     En todos los grupos anteriores (lineas anterories)
            // 0101       se añade el elemento de la segunda columna
            // 1100
            // 1100
            // 1110
            // 1101

            // 1010
            // 0110
            // 0010     Elemento repetido 3ra columna
            // 0011
            // 1010
            // 1110
            // 1010     Elemento repetido 3ra columna
            // 1011
            // 1110
            // 0110
            // 0110
            // 0111
            // 1110
            // 1110
            // 1110     Elemento repetido 3ra columna
            // 1111


            List<List<Assignatura>> CombinacionesAss = new List<List<Assignatura>>();
            List<List<Assignatura>> CombinacionesAssF = new List<List<Assignatura>>();
            
            //Se añade el primer bloque
            for (int i = 0; i < ListAssF.Count; i++)
            {
                List<Assignatura> L = new List<Assignatura>();
                L.Add(ListAssF[i]);
                CombinacionesAss.Add( L);
            }

            //Se hacen los siguientes bloques del algoritmo de combinacion
            foreach(Assignatura j in ListAssF)
            {
                int l = 0;
                int indice = CombinacionesAss.Count;
                while (l<indice)
                {
                    //En los bloques hay lineas en la que el elemento de la columna ya esta, entonces ese lo saltamos
                    //para ello miramos en cada fila si el elemento de la columna se encuentra en la fila
                    bool encontrado = false;
                    foreach(Assignatura k in CombinacionesAss[l])
                    {
                        if (k == j)
                        {
                            encontrado = true;
                        }
                    }

                    //Si no se encuentra el elmento se procede a añadir las nueva lineas de combinacion
                    if (!encontrado)
                    {
                        Assignatura[] B = new Assignatura[CombinacionesAss[l].Count+1];
                        int d = 0;
                        foreach(Assignatura m in CombinacionesAss[l])
                        {
                                B[d] = m;
                                
                            
                            d++;
                        }
                        
                        B[d] = j;
                        
                        double sum = 0;

                        //Para sumar el total de creditos en la combinacion
                        foreach (Assignatura x in B)
                        {
                            sum = sum + x.creditos;
                        }

                        //Agrega solo si cumple las dos condiciones
                        if ((B.Length == NumAssignatruas) && (sum >= creditosMin))
                        {
                            List<Assignatura> LAF = new List<Assignatura>();
                            foreach (Assignatura m in B)
                            {
                                LAF.Add(m);
                            }
                            //Aqui tenemos la agregacion a la lista que contienen las combinaciones que nos interesant
                            CombinacionesAssF.Add(LAF);
                        }
                        List<Assignatura> LA = new List<Assignatura>();
                        foreach (Assignatura m in B)
                        {
                            LA.Add(m);
                        }
                        //Aqui tenemos la agregacion a la lista que contienen TODAS las combinaciones
                        CombinacionesAss.Add(LA);

                    }
                    
                    l++;
                }
            }

            //Hay combinaciones que cotienen los mismos elementos pero en diferente orden
            //El WHILE  sirve para eliminar las combinaciones repetidas
            int u = 0;
            while (u < CombinacionesAssF.Count-1)
            {
                for(int q = u + 1; q < CombinacionesAssF.Count; q++)
                {
                    int e = 0;
                    foreach(Assignatura w in CombinacionesAssF[u])
                    {
                        if (CombinacionesAssF[q].Contains(w))
                        {
                            e++;
                        }
                    }
                    if (e == NumAssignatruas)
                    {
                        CombinacionesAssF.Remove(CombinacionesAssF[q]);
                    }
                    
                }
                
                    u++;
                
            }
            //FOR para eliminar las combinaciones que no cumplen los corequisitos
            foreach (Condiciones Cond in ListCond)
            {
                int i = 0;
                while (i < CombinacionesAssF.Count)
                {
                    if (CombinacionesAssF[i].Contains(Cond.Assignatura))
                    {
                        int eliminar = 0;
                        foreach(Assignatura Ass in Cond.AssCorequisit)
                        {
                            if (CombinacionesAssF[i].Contains(Ass))
                            {
                                eliminar++;
                            }
                        }
                        if (eliminar != Cond.AssCorequisit.Count)
                        {
                            CombinacionesAssF.Remove(CombinacionesAssF[i]);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return CombinacionesAssF;
            

        }
            

        static void Main(string[] args)
        {

            int creditosMin = 30;
            int NumAssignatruas = 6;
            Assignatura Unidad = new Assignatura();
            List<Assignatura> ListAss = new List<Assignatura>();
            Condiciones Cond1 = new Condiciones();
            Condiciones Cond2 = new Condiciones();

            //Añadir assignaturas que se tienen que combinar en una lista
            //Unidad = InsertarAss("DSA", "DISSENY DE SERVEIS I APLICACIONS (DSA)", 10);
            //ListAss.Add(Unidad);
            Unidad = InsertarAss("IX", "INTERCONNEXIÓ DE XARXES (IX)", 6);
            ListAss.Add(Unidad);
            //Unidad = Insertar("TIQ", "TECNOLOGIES D'INFORMACIÓ QUÀNTICA (TIQ)", 6);
            //ListAss.Add(Unidad);
            Unidad = InsertarAss("SX", "SEGURETAT EN XARXES (SX)", 4);
            ListAss.Add(Unidad);
            Unidad = InsertarAss("GEO - MP3", "GEOTÈCNIA (GEO-MP3)", 4.5);
            ListAss.Add(Unidad);
            Unidad = InsertarAss("PX", "PLANIFICACIÓ DE XARXES (PX)", 4);
            ListAss.Add(Unidad);
            Cond1.AssCorequisit.Add(Unidad);
            Unidad = InsertarAss("XT", "XARXES DE TRANSPORT (XT)", 4);
            ListAss.Add(Unidad);
            Cond1.AssCorequisit.Add(Unidad);
            Unidad = InsertarAss("ER", "EMISSORS I RECEPTORS (ER)", 4.5);
            ListAss.Add(Unidad);
            Cond2.AssCorequisit.Add(Unidad);
            Unidad = InsertarAss("IOT", "INFRAESTRUCTURES I OPERACIÓ DE TELECOMUNICACIONS (IOT-M)", 6);
            ListAss.Add(Unidad);
            Cond1.Assignatura = Unidad;
            Unidad = InsertarAss("MXS", "MOBILITAT, XARXES I SERVEIS (MXS)", 6);
            ListAss.Add(Unidad);
            Cond2.Assignatura = Unidad;
            Unidad = InsertarAss("SAI", "SERVEIS AUDIOVISUALS SOBRE INTERNET (SAI)", 4);
            ListAss.Add(Unidad);

            //Crear listas de las assignatruas que tienen requisitos y corequisitos
            List<Condiciones> ListCond = new List<Condiciones>();
            ListCond.Add(Cond1);
            ListCond.Add(Cond2);


            //Llamar la funcion que devuelve una lista de listas de assingaturas {[DSA,IX],[DSA,IOT],[IX,IOT]}
            List<List<Assignatura>> listaCombinaciones = Combinaciones(ListAss, NumAssignatruas,creditosMin,ListCond);

            //Escribir por consola
            StreamWriter sw = new StreamWriter(@"C:\Users\Carme Alcala\Downloads\CombinacionHorarios_creditos.txt");
            foreach (List<Assignatura> g in listaCombinaciones)
            {
                string S = "";
                foreach (Assignatura h in g)
                {
                    Console.Write(h.Siglas+"\t");
                    
                    //S=S+'"' + h.Nombre + '"' + ",";
                    S = S + "\t" + h.Nombre;
                }
                //S=S.Remove(S.Length-1,1);
                Console.WriteLine();
                sw.Write(S+"\n");
            }
            sw.Close();





        }
    }
}
