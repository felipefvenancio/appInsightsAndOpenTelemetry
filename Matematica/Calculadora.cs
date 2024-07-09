using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matematica
{
    class Calculadora
    {
        readonly ITela _tela;
        public Calculadora(ITela tela)
        {
            _tela = tela;
        }
        public decimal N1 { get; set; }
        public decimal N2 { get; set; }
        public string Operacao { get; set; }
        public void Calcular()
        {
            switch (Operacao)
            {
                case "soma":
                    _tela.Exibir(Operacao , N1 + N2);
                    break;
                case "subtracao":
                    _tela.Exibir(Operacao, N1 - N2);
                    break;
                case "multiplicacao":
                    _tela.Exibir(Operacao, N1 * N2);
                    break;
                case "divisao":
                    _tela.Exibir(Operacao, N1 / N2);
                    break;
            }
        }
    }
}
