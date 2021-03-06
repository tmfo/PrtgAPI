﻿using System.Diagnostics.CodeAnalysis;

namespace PrtgAPI.Parameters
{
    [ExcludeFromCodeCoverage]
    class PauseParametersBase : BaseActionParameters
    {
        protected PauseParametersBase(int objectId) : base(objectId)
        {
        }

        public string PauseMessage
        {
            get { return (string) this[Parameter.PauseMessage]; }
            set { this[Parameter.PauseMessage] = value; }
        }
    }
}
