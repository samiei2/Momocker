using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoker.src
{
    public class GestureState
    {
        internal System.Drawing.Point oldCursorPosition;
        long timeStamp = 0;

        public GestureState(long v)
        {
            this.TimeStamp = v;
        }

        public long TimeStamp
        {
            get
            {
                return timeStamp;
            }

            set
            {
                timeStamp = value;
            }
        }
    }

    public class ScrollGestureState : GestureState
    {
        public ScrollGestureState(long v):base(v)
        {
            
        }
    }
}
