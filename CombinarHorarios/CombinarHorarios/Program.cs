using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace CombinarHorarios
{
    class Program
    {
        public class Assignatura
        {
            public string Nombre;
            public string id;
            public List<string> subgrupo;

            public Assignatura(string n, string i, List<string> d)
            {
                Nombre = n;
                id = i;
                subgrupo = d;
            }

        }
        public static List<Assignatura> ListaAssignaturas = new List<Assignatura>();
        
        public static List<string> AObligatorias = new List<string> {  };
        public static List<string> AOpcionales = new List<string> { };
        public static int numAssignaturas = 5;




        public static void Main(string[] args)
        {

            int f = 0;
            foreach (string line in System.IO.File.ReadLines(@"C:\Users\Carme Alcala\Downloads\CombinacionHorarios_creditos.txt"))
            {


                string path = @"C:\Users\Carme Alcala\Downloads\Horarios" + f.ToString();
                Directory.CreateDirectory(path);
               
                string[] listAss=line.Split("\t");
                AObligatorias.Clear();
                foreach(string ass in listAss)
                {
                    AObligatorias.Add(ass);
                }
                AObligatorias.RemoveAt(0);
                StreamWriter sw = new StreamWriter(path+"/CombinacionHorarios.txt");

                List<string> list = new List<string>();
                HtmlWeb oWeb = new HtmlWeb();
                HtmlDocument doc = oWeb.Load("https://sia.upc.edu/SIA/INFOWEB_HORARIS.FILTRE01?v_curs_quad=2022-1&w_codi_ue=300");
                HtmlNode Body = doc.DocumentNode.CssSelect(".dadaTaula2").First();
                foreach (var Nodo in Body.CssSelect("option"))
                {
                    List<string> ListSubgrupo = new List<string>();
                    string subgrupo;
                    string compare = "nada";
                    string Nombre = Nodo.InnerHtml;
                    if (AObligatorias.Contains(Nombre) || AOpcionales.Contains(Nombre))
                    {
                        var id = Nodo.OuterHtml.Split('"')[1];
                        doc = oWeb.Load("https://sia.upc.edu/SIA/INFOWEB_HORARIS.FILTRE02?v_curs_quad=2022-1&w_codi_ue=300&w_codi_programa_p_simula=1162&v_per=A&w_codi_ud_p=&w_codi_ud_p=" + id + "&v_franja=X");
                        Body = doc.DocumentNode.CssSelect(".dadaTaula0").First();
                        foreach (var BNodo in Body.CssSelect("option"))
                        {
                            subgrupo = BNodo.OuterHtml.Split('"')[1];
                            if (subgrupo != compare)
                            {
                                ListSubgrupo.Add(subgrupo);
                                compare = subgrupo;
                            }

                        }
                        var clase = new Assignatura(Nombre, id, ListSubgrupo);
                        ListaAssignaturas.Add(clase);
                    }
                }

                List<string> listaCodigosObli = CombinarNAsignturas(AObligatorias);
                List<string> listaCodigosOpci = CombinarAOpcionales(AObligatorias, AOpcionales);
                List<string> listaCodigosT = new List<string>();
                if (listaCodigosObli.Count == 0)
                {
                    listaCodigosT = listaCodigosOpci;
                }
                else if (listaCodigosOpci.Count != 0)
                {
                    foreach (var j in listaCodigosObli)
                    {
                        foreach (var k in listaCodigosOpci)
                        {
                            listaCodigosT.Add(j + k);
                        }
                    }
                }

                else
                    listaCodigosT = listaCodigosObli;



                WebClient mywebClient = new WebClient();
                for (int i = 0; i < listaCodigosT.Count; i++)
                {
                    mywebClient.DownloadFile("https://sia.upc.edu/SIA/INFOWEB.horariAssigsGrups_CSV?a_assigs_grups=" + listaCodigosT[i] + "@&v_curs_quad=2022-1&a_codi_programes=@@", @"C:\Users\Carme Alcala\Downloads\Horarios"+f+"/Horario_" + i + ".csv");

                    string ubicacionArchivo = @"C:\Users\Carme Alcala\Downloads\Horarios"+f+"/Horario_" + i + ".csv";
                    System.IO.StreamReader archivo = new System.IO.StreamReader(ubicacionArchivo);

                    List<List<string>> LineasT = new List<List<string>>();
                    // Si el archivo no tiene encabezado, elimina la siguiente línea
                    archivo.ReadLine(); // Leer la primera línea pero descartarla porque es el encabezado
                    string linea = archivo.ReadLine();
                    while (linea != null)
                    {
                        string[] LineasF = linea.Split('"');
                        LineasT.Add(LineasF.ToList<string>());
                        linea = archivo.ReadLine();
                    }
                    int combinacion = 1;
                    int j = 0;
                    while ((j < LineasT.Count - 1) && (combinacion == 1))
                    {

                        int k = j + 1;
                        while ((k < LineasT.Count) && (combinacion == 1))
                        {
                            if (LineasT[j][3] == LineasT[k][3])
                            {
                                int Hinicioj = Convert.ToInt32(LineasT[j][5].Split(":")[0]);
                                int Minicioj = Convert.ToInt32(LineasT[j][5].Split(":")[1]);
                                int IJ = Hinicioj * 100 + Minicioj;
                                int Hfinalj = Convert.ToInt32(LineasT[j][9].Split(":")[0]);
                                int Mfinalj = Convert.ToInt32(LineasT[j][9].Split(":")[1]);
                                int FJ = Hfinalj * 100 + Mfinalj;

                                int Hiniciok = Convert.ToInt32(LineasT[k][5].Split(":")[0]);
                                int Miniciok = Convert.ToInt32(LineasT[k][5].Split(":")[1]);
                                int IK = Hiniciok * 100 + Miniciok;
                                int Hfinalk = Convert.ToInt32(LineasT[k][9].Split(":")[0]);
                                int Mfinalk = Convert.ToInt32(LineasT[k][9].Split(":")[1]);
                                int FK = Hfinalk * 100 + Mfinalk;

                                if (FJ != 0)
                                {
                                    if ((IJ > IK) && (IJ < FK))
                                        combinacion = 0;

                                    else if (IJ == IK)
                                        combinacion = 0;
                                    else if ((IJ < IK) && (FJ > IK))
                                        combinacion = 0;
                                    else
                                        k++;
                                }
                                else
                                    k++;
                            }
                            else
                                k++;
                        }
                        j++;
                    }
                    if (combinacion == 1)
                    {
                        mywebClient.DownloadFile("https://sia.upc.edu/SIA/INFOWEB.horariAssigsGrups?a_assigs_grups=" + listaCodigosT[i] + "@&n_hora_ini=&n_hora_fi=&v_curs_quad=2022-1&a_codi_programes=@@", @"C:\Users\Carme Alcala\Downloads\Horarios"+f+"/Horario_" + i + ".pdf");

                        sw.WriteLine("Horario " + Convert.ToString(i) + "\n");
                        string[] AssSubGrupo = listaCodigosT[i].Split("@");

                        for (int m = 1; m < AssSubGrupo.Length; m++)
                        {
                            string subgrupoTxt = "";
                            string assignatrua = AssSubGrupo[m].Split("-")[0];
                            string subgrupo = AssSubGrupo[m].Split("-")[1] + "-" + AssSubGrupo[m].Split("-")[2];
                            doc = oWeb.Load("https://sia.upc.edu/SIA/INFOWEB_HORARIS.FILTRE02?v_curs_quad=2022-1&w_codi_ue=300&w_codi_programa_p_simula=1162&v_per=A&w_codi_ud_p=&w_codi_ud_p=" + assignatrua + "&v_franja=X");
                            Body = doc.DocumentNode.CssSelect("body").First();
                            var BNodo = Body.CssSelect(".dadaTaula0").First();
                            //string subgrpo = BNodo.CssSelect(assignatrua);
                            string ass = BNodo.CssSelect("a").First().InnerHtml;
                            foreach (var variable in BNodo.CssSelect("option"))
                            {
                                string sub = variable.OuterHtml.Split('"')[1];
                                if (sub == subgrupo)
                                {
                                    subgrupoTxt = variable.InnerText;
                                }

                            }
                            //Pass the filepath and filename to the StreamWriter Constructor

                            //Write a line of text

                            //Write a second line of text
                            sw.WriteLine(ass + "_" + subgrupoTxt + "\n");

                        }
                         sw.WriteLine("\n");

                    }





                }










                sw.Close();
                f++;
            }
        }

        public static List<string> CombinarNAsignturas(List<string> ListaA)
        {
            List<string> CodiogsExcel = new List<string>();
            for (int i = 0; i < ListaA.Count; i++)
            {
                int encontrado = 0;
                int ind;
                int z = 0;
                while (encontrado == 0)
                {
                    if (ListaAssignaturas[z].Nombre == ListaA[i])
                    {
                        encontrado = 1;
                        ind = z;
                    }
                    else
                        z++;
                }
                int indice = CodiogsExcel.Count;
                if (CodiogsExcel.Count != 0)
                {
                    foreach (var j in ListaAssignaturas[z].subgrupo)
                    {

                        for (int k = 0; k < indice; k++)
                        {
                            CodiogsExcel.Add(CodiogsExcel[k] + "@" + ListaAssignaturas[z].id + "-" + j);
                        }
                    }
                }
                else
                {
                    foreach (var j in ListaAssignaturas[z].subgrupo)
                    {
                        CodiogsExcel.Add("@" + ListaAssignaturas[z].id + "-" + j);

                    }
                }
                
                


            }
            int l = 0;
            while (l < CodiogsExcel.Count())
            {
                if (CodiogsExcel[l].Count(f => f == '@') != ListaA.Count)
                    CodiogsExcel.RemoveAt(l);
                else
                    l++;
            }
            return CodiogsExcel;
        }

        public static List<string> CombinarAOpcionales(List<string> AObligatorias, List<string> AOpcionales)
        {
            List<string> ListaT = new List<string>();
            List<string> Lista = new List<string>();
            List<int> NumOpc = new List<int>();
            int length = numAssignaturas - AObligatorias.Count;
            for (int i = 0; i < length; i++)
                NumOpc.Add(i);
            List<List<string>> ListaCombinaciones = produceList(AOpcionales);
            int l = 0;
          
            while (l < ListaCombinaciones.Count())
            {
                if (ListaCombinaciones[l].Count!=numAssignaturas-AObligatorias.Count)
                    ListaCombinaciones.RemoveAt(l);
                else
                    l++;
            }
            for(int i=0;i<ListaCombinaciones.Count;i++)
            {
                ListaT.AddRange(CombinarNAsignturas(ListaCombinaciones[i]));
            }
            return ListaT;
        }
        private static IEnumerable<int> constructSetFromBits(int i)
        {
            for (int n = 0; i != 0; i /= 2, n++)
            {
                if ((i & 1) != 0)
                    yield return n;
            }
        }

        

        private static IEnumerable<List<string>> produceEnumeration(List<string> allValues)
        {
            for (int i = 0; i < (1 << allValues.Count); i++)
            {
                yield return
                    constructSetFromBits(i).Select(n => allValues[n]).ToList();
            }
        }

        public static List<List<string>> produceList(List<string> allValues)
        {
            return produceEnumeration(allValues).ToList();
        }




    }
}
    

