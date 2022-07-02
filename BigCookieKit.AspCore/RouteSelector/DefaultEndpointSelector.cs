﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.RouteSelector
{
    internal sealed class DefaultEndpointSelector : EndpointSelector
    {
        public override Task SelectAsync(
            HttpContext httpContext,
            CandidateSet candidateSet)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (candidateSet == null)
            {
                throw new ArgumentNullException(nameof(candidateSet));
            }

            Select(httpContext, candidateSet.Candidates);
            return Task.CompletedTask;
        }

        internal static void Select(HttpContext httpContext, CandidateState[] candidateState)
        {
            // Fast path: We can specialize for trivial numbers of candidates since there can
            // be no ambiguities
            switch (candidateState.Length)
            {
                case 0:
                    {
                        // Do nothing
                        break;
                    }

                case 1:
                    {
                        ref var state = ref candidateState[0];
                        if (CandidateSet.IsValidCandidate(ref state))
                        {
                            httpContext.SetEndpoint(state.Endpoint);
                            httpContext.Request.RouteValues = state.Values!;
                        }

                        break;
                    }

                default:
                    {
                        // Slow path: There's more than one candidate (to say nothing of validity) so we
                        // have to process for ambiguities.
                        ProcessFinalCandidates(httpContext, candidateState);
                        break;
                    }
            }
        }

        public static Endpoint ProcessFinalCandidates(CandidateState[] candidateState)
        {
            Endpoint? endpoint = null;
            RouteValueDictionary? values = null;
            int? foundScore = null;
            for (var i = 0; i < candidateState.Length; i++)
            {
                ref var state = ref candidateState[i];
                if (!CandidateSet.IsValidCandidate(ref state))
                {
                    continue;
                }

                if (foundScore == null)
                {
                    // This is the first match we've seen - speculatively assign it.
                    endpoint = state.Endpoint;
                    values = state.Values;
                    foundScore = state.Score;
                }
                else if (foundScore < state.Score)
                {
                    // This candidate is lower priority than the one we've seen
                    // so far, we can stop.
                    //
                    // Don't worry about the 'null < state.Score' case, it returns false.
                    break;
                }
                else if (foundScore == state.Score)
                {
                    // This is the second match we've found of the same score, so there
                    // must be an ambiguity.
                    //
                    // Don't worry about the 'null == state.Score' case, it returns false.

                    ReportAmbiguity(candidateState);

                    // Unreachable, ReportAmbiguity always throws.
                    throw new NotSupportedException();
                }
            }

            return endpoint;
        }

        private static void ProcessFinalCandidates(
            HttpContext httpContext,
            CandidateState[] candidateState)
        {
            Endpoint? endpoint = null;
            RouteValueDictionary? values = null;
            int? foundScore = null;
            for (var i = 0; i < candidateState.Length; i++)
            {
                ref var state = ref candidateState[i];
                if (!CandidateSet.IsValidCandidate(ref state))
                {
                    continue;
                }

                if (foundScore == null)
                {
                    // This is the first match we've seen - speculatively assign it.
                    endpoint = state.Endpoint;
                    values = state.Values;
                    foundScore = state.Score;
                }
                else if (foundScore < state.Score)
                {
                    // This candidate is lower priority than the one we've seen
                    // so far, we can stop.
                    //
                    // Don't worry about the 'null < state.Score' case, it returns false.
                    break;
                }
                else if (foundScore == state.Score)
                {
                    // This is the second match we've found of the same score, so there
                    // must be an ambiguity.
                    //
                    // Don't worry about the 'null == state.Score' case, it returns false.

                    ReportAmbiguity(candidateState);

                    // Unreachable, ReportAmbiguity always throws.
                    throw new NotSupportedException();
                }
            }

            if (endpoint != null)
            {
                httpContext.SetEndpoint(endpoint);
                httpContext.Request.RouteValues = values!;
            }
        }

        private static void ReportAmbiguity(CandidateState[] candidateState)
        {
            // If we get here it's the result of an ambiguity - we're OK with this
            // being a littler slower and more allocatey.
            var matches = new List<Endpoint>();
            for (var i = 0; i < candidateState.Length; i++)
            {
                ref var state = ref candidateState[i];
                if (CandidateSet.IsValidCandidate(ref state))
                {
                    matches.Add(state.Endpoint);
                }
            }

            var message = "";
            throw new AmbiguousMatchException(message);
        }
    }
}
