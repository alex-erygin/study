using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IHello: Orleans.IGrainWithIntegerKey
    {
        Task<String> SayHello(string greeting);
    }
}
