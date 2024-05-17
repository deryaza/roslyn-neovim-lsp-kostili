// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Options;
using Roslyn.LanguageServer.Protocol;
using Roslyn.Utilities;
using LSP = Roslyn.LanguageServer.Protocol;

namespace Microsoft.CodeAnalysis.LanguageServer.Handler.SemanticTokens
{
    [Method(Methods.TextDocumentSemanticTokensFullName)]
    internal class SemanticTokensFullHandler : ILspServiceDocumentRequestHandler<SemanticTokensParams, LSP.SemanticTokens>
    {
        private readonly IGlobalOptionService _globalOptions;
        private readonly SemanticTokensRefreshQueue _semanticTokenRefreshQueue;

        public bool MutatesSolutionState => false;
        public bool RequiresLSPSolution => true;

        public SemanticTokensFullHandler(
            IGlobalOptionService globalOptions,
            SemanticTokensRefreshQueue semanticTokensRefreshQueue)
        {
            _globalOptions = globalOptions;
            _semanticTokenRefreshQueue = semanticTokensRefreshQueue;
        }

        public TextDocumentIdentifier GetTextDocumentIdentifier(LSP.SemanticTokensParams request)
        {
            Contract.ThrowIfNull(request.TextDocument);
            return request.TextDocument;
        }

        public async Task<LSP.SemanticTokens> HandleRequestAsync(
            SemanticTokensParams request,
            RequestContext context,
            CancellationToken cancellationToken)
        {
            Contract.ThrowIfNull(request.TextDocument, "TextDocument is null.");
            var tokensData = await SemanticTokensHelpers.HandleRequestHelperAsync(_globalOptions, _semanticTokenRefreshQueue, [], context, cancellationToken, true).ConfigureAwait(false);
            return new LSP.SemanticTokens { Data = tokensData };
        }
    }
}
