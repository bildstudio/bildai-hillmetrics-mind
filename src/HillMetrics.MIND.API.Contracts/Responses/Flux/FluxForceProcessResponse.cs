using HillMetrics.Core.Financial.DataPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class FluxForceProcessResponse
    {
        public bool HasProcessSomething { get; set; }

        public Dictionary<int, ProcessFluxCommandResultItemDto> ContentResult { get; set; } = new();

        public record ProcessFluxCommandResultItemDto(Status Status, Dictionary<FinancialDataPoint, bool> ExecutorsStatus, long processMs);

        public enum Status
        {
            NoRawId,
            ContentNull,
            Success,
            NoValidatedFinancialDataPoint,
            PartiallySuccess,
            Fail
        }
    }
}
