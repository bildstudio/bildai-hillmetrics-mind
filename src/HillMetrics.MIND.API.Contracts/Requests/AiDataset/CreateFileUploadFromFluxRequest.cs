﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HillMetrics.Core.Financial;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;

namespace HillMetrics.MIND.API.Contracts.Requests.AiDataset
{
    public class CreateFileUploadFromFluxRequest
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        public required string FileName { get; set; } = null!;

        /// <summary>
        /// Content type of the file
        /// </summary>
        public required string ContentType { get; set; } = null!;

        /// <summary>
        /// ID of the associated Flux content
        /// </summary>
        public int FluxFetchingContentId { get; set; }

        /// <summary>
        /// Difficulty level of the file
        /// </summary>
        public required FileDifficulty Difficulty { get; set; }

        /// <summary>
        /// Type of financial instrument
        /// </summary>
        public required FinancialType FinancialType { get; set; }
    }
}
