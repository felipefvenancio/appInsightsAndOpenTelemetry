using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matematica
{
    class Tela : ITela
    {
        public void Exibir(string operacao ,decimal resultado)
        {
            //vou no bd para obter a cor
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.WriteLine($"{operacao}:  {resultado}");

        }
    }
}
