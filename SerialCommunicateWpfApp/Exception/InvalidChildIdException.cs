using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class InvalidChildIdException : Exception {
    public InvalidChildIdException() : base("無効な子機IDを参照しました") { }
}
