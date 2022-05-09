using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.Timings;

using System;
using System.Collections.Concurrent;

namespace BestHTTP.Core
{
    public enum RequestEvents
    {
        Upgraded,
        DownloadProgress,
        UploadProgress,
        StreamingData,
        StateChange,
        Resend,
        Headers
    }

    public
#if CSHARP_7_OR_LATER
        readonly
#endif
        struct RequestEventInfo
    {
        public readonly HTTPRequest SourceRequest;
        public readonly RequestEvents Event;

        public readonly HTTPRequestStates State;

        public readonly long Progress;
        public readonly long ProgressLength;

        public readonly byte[] Data;
        public readonly int DataLength;

        public RequestEventInfo(HTTPRequest request, RequestEvents @event)
        {
            this.SourceRequest = request;
            this.Event = @event;

            this.State = HTTPRequestStates.Initial;

            this.Progress = this.ProgressLength = 0;

            this.Data = null;
            this.DataLength = 0;
        }

        public RequestEventInfo(HTTPRequest request, HTTPRequestStates newState)
        {
            this.SourceRequest = request;
            this.Event = RequestEvents.StateChange;
            this.State = newState;

            this.Progress = this.ProgressLength = 0;
            this.Data = null;
            this.DataLength = 0;
        }

        public RequestEventInfo(HTTPRequest request, RequestEvents @event, long progress, long progressLength)
        {
            this.SourceRequest = request;
            this.Event = @event;
            this.State = HTTPRequestStates.Initial;

            this.Progress = progress;
            this.ProgressLength = progressLength;
            this.Data = null;
            this.DataLength = 0;
        }

        public RequestEventInfo(HTTPRequest request, byte[] data, int dataLength)
        {
            this.SourceRequest = request;
            this.Event = RequestEvents.StreamingData;
            this.State = HTTPRequestStates.Initial;

            this.Progress = this.ProgressLength = 0;
            this.Data = data;
            this.DataLength = dataLength;
        }

        public override string ToString()
        {
            return string.Format("[RequestEventInfo SourceRequest: {0}, Event: {1}, State: {2}, Progress: {3}, ProgressLength: {4}, Data: {5}]",
                this.SourceRequest.CurrentUri, this.Event, this.State, this.Progress, this.ProgressLength, this.DataLength);
        }
    }

    public static class RequestEventHelper
    {
        private static ConcurrentQueue<RequestEventInfo> requestEventQueue = new ConcurrentQueue<RequestEventInfo>();

#pragma warning disable 0649
        public static Action<RequestEventInfo> OnEvent;
#pragma warning restore

        public static void EnqueueRequestEvent(RequestEventInfo @event)
        {
            requestEventQueue.Enqueue(@event);
        }

        internal static void Clear()
        {
            requestEventQueue.Clear();
        }

        internal static void ProcessQueue()
        {
            RequestEventInfo requestEvent;
            while (requestEventQueue.TryDequeue(out requestEvent))
            {
                HTTPRequest source = requestEvent.SourceRequest;

                if (HTTPManager.Logger.Level == Loglevels.All)
                    HTTPManager.Logger.Information("RequestEventHelper", "Processing request event: " + requestEvent.ToString(), source.Context);

                if (OnEvent != null)
                {
                    try
                    {
                        OnEvent(requestEvent);
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("RequestEventHelper", "ProcessQueue", ex, source.Context);
                    }
                }
                
                switch (requestEvent.Event)
                {
                    case RequestEvents.StreamingData:
                        {
                            var response = source.Response;
                            if (response != null)
                                System.Threading.Interlocked.Decrement(ref response.UnprocessedFragments);

                            bool reuseBuffer = true;
                            try
                            {
                                if (source.OnStreamingData != null)
                                    reuseBuffer = source.OnStreamingData(source, response, requestEvent.Data, requestEvent.DataLength);
                            }
                            catch (Exception ex)
                            {
                                HTTPManager.Logger.Exception("RequestEventHelper", "Process RequestEventQueue - RequestEvents.StreamingData", ex, source.Context);
                            }

                            if (reuseBuffer)
                                BufferPool.Release(requestEvent.Data);
                            break;
                        }

                    case RequestEvents.DownloadProgress:
                        try
                        {
                            if (source.OnDownloadProgress != null)
                            {
                                source.OnDownloadProgress(source, requestEvent.Progress, requestEvent.ProgressLength);
                            }
                        }
                        catch (Exception ex)
                        {
                            HTTPManager.Logger.Exception("RequestEventHelper", "Process RequestEventQueue - RequestEvents.DownloadProgress", ex, source.Context);
                        }
                        break;

                    case RequestEvents.UploadProgress:
                        try
                        {
                            if (source.OnUploadProgress != null)
                                source.OnUploadProgress(source, requestEvent.Progress, requestEvent.ProgressLength);
                        }
                        catch (Exception ex)
                        {
                            HTTPManager.Logger.Exception("RequestEventHelper", "Process RequestEventQueue - RequestEvents.UploadProgress", ex, source.Context);
                        }
                        break;

                    case RequestEvents.Upgraded:
                        try
                        {
                            if (source.OnUpgraded != null)
                                source.OnUpgraded(source, source.Response);
                        }
                        catch (Exception ex)
                        {
                            HTTPManager.Logger.Exception("RequestEventHelper", "Process RequestEventQueue - RequestEvents.Upgraded", ex, source.Context);
                        }

                        IProtocol protocol = source.Response as IProtocol;
                        if (protocol != null)
                            ProtocolEventHelper.AddProtocol(protocol);
                        break;

                    case RequestEvents.Resend:
                        source.State = HTTPRequestStates.Initial;
                        
                        var host = HostManager.GetHost(source.CurrentUri.Host);

                        host.Send(source);

                        break;

                    case RequestEvents.Headers:
                        {
                            try
                            {
                                var response = source.Response;
                                if (source.OnHeadersReceived != null && response != null)
                                    source.OnHeadersReceived(source, response);
                            }
                            catch (Exception ex)
                            {
                                HTTPManager.Logger.Exception("RequestEventHelper", "Process RequestEventQueue - RequestEvents.Headers", ex, source.Context);
                            }
                            break;
                        }

                    case RequestEvents.StateChange:
                        try
                        {
                            RequestEventHelper.HandleRequestStateChange(requestEvent);
                        }
                        catch(Exception ex)
                        {
                            HTTPManager.Logger.Exception("RequestEventHelper", "HandleRequestStateChange", ex, source.Context);
                        }
                        break;
                }
            }
        }

        private static bool AbortRequestWhenTimedOut(DateTime now, object context)
        {
            HTTPRequest request = context as HTTPRequest;

            if (request.State != HTTPRequestStates.Processing)
                return false; // don't repeat

            // Protocols will shut down themselves
            if (request.Response is IProtocol)
                return false;

            if (request.IsTimedOut)
            {
                HTTPManager.Logger.Information("RequestEventHelper", "AbortRequestWhenTimedOut - Request timed out. CurrentUri: " + request.CurrentUri.ToString(), request.Context);
                request.Abort();

                return false; // don't repeat
            }

            return true;  // repeat
        }

        internal static void HandleRequestStateChange(RequestEventInfo @event)
        {
            HTTPRequest source = @event.SourceRequest;

            switch (@event.State)
            {
                case HTTPRequestStates.Processing:
                    if ((!source.UseStreaming && source.UploadStream == null) || source.EnableTimoutForStreaming)
                        BestHTTP.Extensions.Timer.Add(new BestHTTP.Extensions.TimerData(TimeSpan.FromSeconds(1), @event.SourceRequest, AbortRequestWhenTimedOut));
                    break;

                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                case HTTPRequestStates.Error:
                case HTTPRequestStates.Aborted:
                case HTTPRequestStates.Finished:

#if !BESTHTTP_DISABLE_CACHING
                    // Here we will try to load content for a failed load. Failed load is a request with ConnectionTimedOut, TimedOut or Error state.
                    // A request with Finished state but response with status code >= 500 also something that we will try to load from the cache.
                    // We have to set what we going to try to load here too (other place is inside IsCachedEntityExpiresInTheFuture) as we don't want to load a cached content for
                    // a request that just finished without any problem!

                    try
                    {
                        bool tryLoad = !source.DisableCache && source.State != HTTPRequestStates.Aborted && (source.State != HTTPRequestStates.Finished || source.Response == null || source.Response.StatusCode >= 500);
                        if (tryLoad && Caching.HTTPCacheService.IsCachedEntityExpiresInTheFuture(source))
                        {
                            HTTPManager.Logger.Information("RequestEventHelper", "IsCachedEntityExpiresInTheFuture check returned true! CurrentUri: " + source.CurrentUri.ToString(), source.Context);

                            PlatformSupport.Threading.ThreadedRunner.RunShortLiving<HTTPRequest>((req) =>
                            {
                                // Disable any other cache activity.
                                req.DisableCache = true;

                                var originalState = req.State;
                                if (Connections.ConnectionHelper.TryLoadAllFromCache("RequestEventHelper", req, req.Context))
                                {
                                    if (req.State != HTTPRequestStates.Finished)
                                        req.State = HTTPRequestStates.Finished;
                                    else
                                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(req, HTTPRequestStates.Finished));
                                }
                                else
                                {
                                    HTTPManager.Logger.Information("RequestEventHelper", "TryLoadAllFromCache failed to load! CurrentUri: " + req.CurrentUri.ToString(), source.Context);

                                    // If for some reason it couldn't load we place back the request to the queue.
                                    RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(req, originalState));
                                }
                            }, source);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("RequestEventHelper", string.Format("HandleRequestStateChange - Cache probe - CurrentUri: \"{0}\" State: {1} StatusCode: {2}", source.CurrentUri, source.State, source.Response != null ? source.Response.StatusCode : 0), ex, source.Context);
                    }
#endif

                    source.Timing.Add(TimingEventNames.Queued_For_Disptach);
                    source.Timing.Add(TimingEventNames.Finished, DateTime.Now - source.Timing.Start);

                    if (source.Callback != null)
                    {
                        try
                        {
                            source.Callback(source, source.Response);

                            source.Timing.Add(TimingEventNames.Callback);

                            if (HTTPManager.Logger.Level <= Loglevels.Information)
                                HTTPManager.Logger.Information("RequestEventHelper", "Finishing request. Timings: " + source.Timing.ToString(), source.Context);

                        }
                        catch (Exception ex)
                        {
                            HTTPManager.Logger.Exception("RequestEventHelper", "HandleRequestStateChange " + @event.State, ex, source.Context);
                        }
                    }

                    source.Dispose();

                    HostManager.GetHost(source.CurrentUri.Host)
                                .GetHostDefinition(HostDefinition.GetKeyForRequest(source))
                                .TryToSendQueuedRequests();
                    break;
            }
        }
    }
}
