using System;
using System.Collections.Generic;

namespace AsmodatStandard.Types
{
    public class PreetyException
    {
        public PreetyException(Exception ex, int maxDepth = 1000)
        {
            if (ex == null)
                throw new ArgumentNullException($"{nameof(PreetyException)} doesn't allow for {nameof(ex)} parameter to be null.");

            this.Message = ex.Message;
            this.Type = ex.GetType().FullName;

            if (maxDepth > 0)
            {
                if (ex.InnerException != null)
                    this.InnerException = new PreetyException(ex.InnerException, (maxDepth - 1));

                if (ex is AggregateException)
                {
                    var aex = ex as AggregateException;

                    if (aex?.InnerExceptions != null)
                    {
                        var list = new List<PreetyException>();

                        foreach (var ae in aex.InnerExceptions)
                            if(ae != null)
                                list.Add(new PreetyException(ae, (maxDepth - 1)));

                        this.InnerExceptions = list.ToArray();
                    }
                }
            }
        }

        public string Type { get; set; }
        public string Message { get; set; }
        public PreetyException InnerException { get; set; }
        public PreetyException[] InnerExceptions { get; set; }
    }
}