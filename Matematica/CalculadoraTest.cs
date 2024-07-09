using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matematica
{
    class CalculadoraTest
    {
        public void DoisMais2Igual4EExbirNaTela()
        {
            var calculadora = new Calculadora(new MockTela());
            calculadora.Calcular();
        }

    }
    class MockTela : ITela
    {
        public void Exibir(string operacao, decimal resultado)
        {
           // "Exibir qualquer coisa";
        }
    }
}
