﻿using System.Diagnostics.CodeAnalysis;

namespace PrtgAPI.Parameters
{
    [ExcludeFromCodeCoverage]
    internal class PauseRequestParameters
    {
        internal PauseRequestParameters(int objectId, int? durationMinutes, string pauseMessage)
        {
            if (durationMinutes == null)
            {
                Parameters = new PauseParameters(objectId);
                Function = CommandFunction.Pause;
            }
            else
            {
                Parameters = new PauseForDurationParameters(objectId, (int)durationMinutes);
                Function = CommandFunction.PauseObjectFor;
            }

            if (pauseMessage != null)
                Parameters.PauseMessage = pauseMessage;
        }

        internal PauseParametersBase Parameters { get; }

        internal CommandFunction Function { get; }
    }
}
