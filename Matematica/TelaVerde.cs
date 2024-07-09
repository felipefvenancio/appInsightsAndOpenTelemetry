using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matematica
{
    class TelaVerde : ITela
    {
        public void Exibir(string operacao ,decimal resultado)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{operacao}:  {resultado}");
        }
    }
}
