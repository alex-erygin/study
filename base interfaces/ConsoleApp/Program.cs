using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileIo;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ISigner signer;
            SignerFactory.CreateSigner(new File(), new File(), "Haba", "Haba", out signer);
        }
    }
}
