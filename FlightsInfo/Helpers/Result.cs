using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightsInfo.Helpers
{
    /// <summary>
    /// Defines a Result object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public Result(T Status)
        {
            this.Status = Status;
        }

        public Result(T Status, string Info)
        {
            this.Status = Status;
            this.Info = Info;
        }

        /// <summary>
        /// Result Status
        /// </summary>
        public T Status { get; private set; }

        /// <summary>
        /// Result extra info (can be null or empty in case of Success)
        /// </summary>
        public string Info { get; private set; } = "";
    }
}