using System;

namespace LiteUI.Navigation
{
    public class ReturnEventArgs<TResult> : EventArgs
    {
        public TResult Result { get; set; }
    }
}
