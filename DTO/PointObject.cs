using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public struct City_Coordinate
    {
        double _x;
        double _y;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public City_Coordinate(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }
}
