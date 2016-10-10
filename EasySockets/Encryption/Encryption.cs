using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySockets.Encryption
{
    public abstract class Encryption
    {
        internal string _key;
        public string Key
        {
            get { return _key; }
        }

        public Encryption(string key)
        {
            _key = key;
        }

        public abstract byte[] Encrypt(byte[] data);
        public abstract byte[] Decrypt(byte[] data);
    }
}
